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

using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Globalization;
using crcPdf;

namespace SharpReport {
    /// <summary>
    /// A SharpReport font.
    /// Is based in our PDF TTF implementation
    /// </summary>
	public class Font {
		private readonly string m_name;
		private readonly float m_size;
		private readonly bool m_bold;
		private readonly bool m_italic;		
        private EFontAlign m_align;
        private FontPlacement m_embedded;
        private Color m_color;
		private float m_lineSpacing = 1.1f;
		private float m_angle;
		

		/// <summary>
		/// Font for SharpReport
		/// </summary>
		/// <param name="name">font name</param>
		/// <param name="size">font size in pixels</param>
		/// <param name="fontEmphasis">bold, italic, etc</param>
        public Font (string name, float size, FontEmphasis fontEmphasis) {
            m_name = name;
			m_size = size / 100;
            m_bold = ((fontEmphasis & FontEmphasis.Bold) == FontEmphasis.Bold);
            m_italic = ((fontEmphasis & FontEmphasis.Italic) == FontEmphasis.Italic);
		}

		/// <summary>
		/// Font for SharpReport
		/// </summary>
		/// <param name="name">font name</param>
		/// <param name="fontEmphasis">bold, italic, etc</param>
        public Font (string name, float size) {
            m_name = name;
			m_size = size / 100;
		}

		public void SetEmbedded(FontPlacement fontPlacement) {
			m_embedded = fontPlacement;
		}

		public void SetColor(Color color) {
			m_color = color;
		}

		public void SetLineSpacing(float lineSpacing) {
			m_lineSpacing = lineSpacing;
		}

		public void SetAlignment(EFontAlign alignment) {
			m_align = alignment;
		}

		public void SetAngle(float angle) {
			m_angle = angle;
		}

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:SharpReport.Font"/>.
        /// </summary>
        /// <returns>A <see cref="T:System.String"/> that represents the current <see cref="T:SharpReport.Font"/>.</returns>
		public override string ToString() {						
			StringBuilder sb = new StringBuilder();
			sb.Append(m_name);
			sb.Append(m_size);
			sb.Append((m_bold ? "0" : "1"));
			sb.Append((m_italic ? "0" : "1"));
			return "Font: " + m_name + "," + m_size.ToString(CultureInfo.InvariantCulture) + " " + (m_bold ? "bold" : "no bold") + " " + (m_italic ? "italic" : "no italic") + " Align:" + m_align.ToString() + " Angle:" + m_angle.ToString(CultureInfo.InvariantCulture) + " Color: " + m_color.ToString();
		}

		private int RegisterDirectory(string baseDir) 		{
			int filesLoaded = 0;

			if (Directory.Exists(baseDir)) 			{
                foreach (string directory in Directory.GetDirectories(baseDir)) {
					filesLoaded += RegisterDirectory(directory);
				}
			}

			return filesLoaded;
		}
       	
        /// <summary>
        /// Get a PDF font
        /// </summary>
        /// <returns>The PDF Font.</returns>
		public DocumentFont GetPDFFont() {
            return crcPdf.Fonts.FontFactory.GetFont(m_name, m_bold, m_italic, (m_embedded == FontPlacement.Embedded ? crcPdf.Fonts.Embedded.Yes : crcPdf.Fonts.Embedded.No));
		}

		internal virtual bool GetUseBase64()
		{
			return false;
		}

		public System.Drawing.Font GetSystemFont(float dpi) {
			System.Drawing.FontStyle fs = 0;
			if (m_italic) {
				fs |= System.Drawing.FontStyle.Italic;
			}
			if (m_bold) {
				fs |= System.Drawing.FontStyle.Bold;
			}

			if (m_name.Contains("/") || m_name.Contains("\\")) {
				System.Drawing.Text.PrivateFontCollection foo = new System.Drawing.Text.PrivateFontCollection();
				foo.AddFontFile(m_name);
				return new System.Drawing.Font(foo.Families[0], m_size * dpi * 0.76f, fs);
			} else {
				return new System.Drawing.Font(m_name, m_size * dpi * 0.76f, fs);				
			}
		
		}

		public float GetHeight() {
			return (GetPDFFont().GetAscendent(m_size)) - (GetPDFFont().GetDescendent(m_size)) ;
		}

		public float GetDescentPoint() {
			return GetPDFFont().GetDescendent(m_size);
		}

		public float GetWidth(string text) 	{									
			return GetPDFFont().GetWidthPointKerned (text, m_size);
		}

		public SharpReport.Color GetPDFColor{
			get { 
                return m_color;
			}
		}

		public float Size {
			get { return m_size; }
		}

        public EFontAlign Align {
            get { return m_align; }
        }

		public float Angle
		{
			get { return m_angle; }
		}

		/// <summary>
		/// Get the color in System.Drawing.Color form
		/// </summary>
		/// <value>The system drawing color inverse order.</value>
        public System.Drawing.Color GetSystemDrawingColor_InverseOrder {
            get { return m_color.GetSystemInverseColor(); }
        }	

		/// <summary>
		/// Returns line spacing
		/// </summary>
		/// <value>The line spacing.</value>
		public float GetLineSpacing {
			get { return m_lineSpacing; }
		}
	}
}