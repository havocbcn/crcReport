// This file is part of SharpReport.
// 
// SharpReport is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// SharpReport is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with SharpReport.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Drawing;
using System.Diagnostics;
using crcPdf;
using System.Globalization;

namespace SharpReport
{
	/// <summary>
	/// Render a box to PDF or TIFF
	/// </summary>
	internal class RenderBox : RenderElement {
		private readonly float m_w;
		private readonly float m_h;
		private readonly Color m_color;
		private readonly bool m_IsFull;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:reporting.RenderBox"/> class.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="w">The width.</param>
		/// <param name="h">The height.</param>
		/// <param name="z">The z coordinate.</param>
		/// <param name="IsFull">If set to <c>true</c> is full.</param>
		/// <param name="color">Color used.</param>
		internal RenderBox(float x, float y, float w, float h, float z, bool IsFull, Color color) : base(x, y, z) {
			m_w = w;
			m_h = h;
			m_color = color;
			m_IsFull = IsFull;

			Debug();
		}

		/// <summary>
		/// Render a box in PDF format
		/// </summary>
		/// <param name="pdf">pdf<param>
		/// <param name="pageSize">Page size.</param>
		internal override void RenderPDF(SimplePdf pdf, PageSize pageSize) {
			float fx = x * pageSize.GetDPI;
			float fy = (pageSize.GetHeightInCM - y) * pageSize.GetDPI;
			float fwidth =  m_w * pageSize.GetDPI;
			float fheight = -m_h * pageSize.GetDPI;

			pdf.SetColor(m_color.R/255f, m_color.G/255f, m_color.B/255f);

			if (m_IsFull)
				pdf.DrawRectangleFull((int)fx, (int)fy, (int)fwidth, (int)fheight);
			else
				pdf.DrawRectangle((int)fx, (int)fy, (int)fwidth, (int)fheight);            
		}

		/// <summary>
		/// Render a box in TIFF format
		/// </summary>
		/// <param name="graphics">Graphics.</param>
		internal override void RenderTIFF(Graphics graphics, PageSize pageSize) {
			// Create rectangle.
			System.Drawing.Rectangle rect = new System.Drawing.Rectangle(
				(int)(x * graphics.DpiX), (int)(y * graphics.DpiY),
				(int)(m_w * graphics.DpiX), (int)(m_h * graphics.DpiY));

			if (m_IsFull) {
				// TIFF COLORS ARE IN INVERSE ORDER!
				SolidBrush brush = new SolidBrush(m_color.GetSystemInverseColor());
				graphics.FillRectangle(brush, rect);
			}
			else {
				// TIFF COLORS ARE IN INVERSE ORDER!
				Pen pen = new Pen(m_color.GetSystemInverseColor(), 1);
				graphics.DrawRectangle(pen, rect);
			}
		}

		[Conditional("DEBUG")]
		private void Debug() {			
			if (x < 0 || x+m_w > 21 || y < 0 || y+m_h > 29.7f) {
				Console.WriteLine("WARNING: box out of limits (" + x + "|" + y + ")");
			}			
		}

		public override string ToString()	
		{
			return "Box " + (m_IsFull ? " full" : "") + 
			"[" + x.ToString(CultureInfo.InvariantCulture) + "," + y.ToString(CultureInfo.InvariantCulture) + "," + m_w.ToString(CultureInfo.InvariantCulture) + "," + m_h.ToString(CultureInfo.InvariantCulture) + "] Z:" + 
			z.ToString(CultureInfo.InvariantCulture) + " " + m_color.ToString();
		}

	}

}
