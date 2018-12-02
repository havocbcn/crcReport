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
using System.Diagnostics;
using System.Drawing;
using System.IO;
using SharpReport.PDF;

namespace SharpReport
{
    /// <summary>
    /// Render an image to PDF or TIFF
    /// </summary>
	internal class RenderImage : RenderElement
	{
		private readonly float m_w;
		private readonly float m_h;
		private readonly byte[] m_image;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:reporting.RenderBox"/> class.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="w">The width.</param>
		/// <param name="h">The height.</param>
		/// <param name="z">The z coordinate.</param>
		/// <param name="image"></param>
		public RenderImage(float x, float y, float z, float w, float h, byte[] image) : base(x, y, z) {
			m_w = w;
			m_h = h;
			m_image = image;

			Debug();
		}

        /// <summary>
        /// Renders the image to PDF
        /// </summary>
        /// <param name="pdf">Cb.</param>
        /// <param name="pageSize">Page size.</param>
        internal override void RenderPDF(SharpPdf pdf, PageSize pageSize) {
            XrefImage img = XrefImageFactory.GetImageJpeg(m_image);
            pdf.DrawImage(x * pageSize.GetDPI, 
                          (pageSize.GetHeightInCM - y - m_h) * pageSize.GetDPI, 
                          m_w * pageSize.GetDPI, 
                          m_h * pageSize.GetDPI, 
                          img);
		}

		/// <summary>
		/// Renders the TIF.
		/// </summary>
		/// <param name="graphics">Graphics.</param>
		internal override void RenderTIFF(Graphics graphics, PageSize pageSize)
		{
			System.Drawing.Rectangle rect = new System.Drawing.Rectangle(
				(int)(x * graphics.DpiX), (int)(y * graphics.DpiY),
				(int)(m_w * graphics.DpiX), (int)(m_h * graphics.DpiY));
			
			System.Drawing.Image img;
			using (var ms = new MemoryStream(m_image))
			{
				img = System.Drawing.Image.FromStream(ms);
			}
			graphics.DrawImage(img, rect);
		}

		[Conditional("DEBUG")]
		private void Debug()
		{
			if (x < 0 || x + m_w > 21 || y < 0 || y + m_h > 29.7f)
			{
				Console.WriteLine("WARNING: box out of limits (" + x + "|" + y + ")");
			}

			if (m_image == null)
			{
				throw new ImageException("null image");
			}

		}

	}

}
