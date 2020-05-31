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
using System.Globalization;
using crcPdf;

namespace SharpReport {
	/// <summary>
	/// Render a label
	/// </summary>
	internal class RenderLabel : RenderElement
	{
		/// <summary>
		/// Text renderer
		/// </summary>
		readonly string m_text;

		/// <summary>
		/// Font used
		/// </summary>
		readonly Font m_font;
        		
		/// <summary>
		/// The degrees to radiant constant
		/// </summary>
		const float degreesToRadiant = 0.01745329252f;

		/// <summary>
		/// Render a new label
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="z">The z coordinate.</param>
		/// <param name="text">text to render</param>
		/// <param name="font">Font used</param>
		internal RenderLabel(float x, float y, float z, string text, Font font) : base(x,y,z) {
			m_text = text;
			m_font = font;
		}

		/// <summary>
		/// Render the label to a PDF
		/// </summary>
		/// <param name="pdf">pdf</param>
		/// <param name="pageSize">Page size.</param>
		internal override void RenderPDF(SimplePdf pdf, PageSize pageSize) {
			float xPos = x * pageSize.GetDPI;
			float yPos = (pageSize.GetHeightInCM - y - m_font.GetHeight()) * pageSize.GetDPI;
			
			pdf.SetFont(m_font.GetPDFFont(), (int)(m_font.Size * pageSize.GetDPI));
			pdf.SetColor(m_font.GetPDFColor.R_Normalized, m_font.GetPDFColor.G_Normalized, m_font.GetPDFColor.B_Normalized);

			if (Math.Abs(m_font.Angle) > 0.0001) {		// se compara con un epsilon por ser un float
				float sinus = (float)Math.Sin(m_font.Angle *degreesToRadiant);
				float cosinus = (float)Math.Cos(m_font.Angle *degreesToRadiant);
				float fontHeight = (-m_font.GetDescentPoint() - m_font.GetHeight())  * pageSize.GetDPI;
				xPos = xPos - sinus * fontHeight;
				yPos = yPos + cosinus * fontHeight - fontHeight;
			}

			pdf.TextAngle(m_font.Angle);

			pdf.DrawText(m_text, (int)xPos, (int)yPos);
		}

		/// <summary>
		/// Renders the label to a TIFF
		/// </summary>
		/// <param name="graphics">Graphics.</param>
		/// <param name="pageSize">page size</param>
		internal override void RenderTIFF(Graphics graphics, PageSize pageSize) {
			PointF pt = new PointF(0, 0);

			if (Math.Abs(m_font.Angle) > 0.001) 
				graphics.RotateTransform(-m_font.Angle, System.Drawing.Drawing2D.MatrixOrder.Append);
			
			graphics.TranslateTransform(x * graphics.DpiX, y * graphics.DpiY, System.Drawing.Drawing2D.MatrixOrder.Append);
    		SolidBrush drawBrush = new SolidBrush(m_font.GetSystemDrawingColor_InverseOrder);			
			graphics.DrawString(m_text, m_font.GetSystemFont(pageSize.GetDPI), drawBrush, pt);			 
			graphics.ResetTransform();
		}

		public override string ToString()	
		{
			return "Text [" + x.ToString(CultureInfo.InvariantCulture) + "," + y.ToString(CultureInfo.InvariantCulture) + "] Z:" + z.ToString(CultureInfo.InvariantCulture) + " " + m_text + " Font: " + m_font.ToString();
		}
	}
}
