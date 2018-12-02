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
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Text;


namespace SharpReport.PDF
{
	public class XrefContents : Xref
	{
		private readonly StringBuilder sb;
        private readonly bool m_useCompression;

        public XrefContents(bool useCompression)
        {
            m_useCompression = useCompression;
            sb = new StringBuilder();
        }

		public override byte[] Write()
		{
            byte[] a1 = Encoding.GetEncoding(1252).GetBytes(@"
stream
");
            byte[] a2;
            if (m_useCompression) {
                a2 = Flate(Encoding.GetEncoding(1252).GetBytes(sb.ToString()));
            } else {
                a2 = Encoding.GetEncoding(1252).GetBytes(sb.ToString());
            }
            byte[] a3 = Encoding.GetEncoding(1252).GetBytes(@"
endstream
");
            
            byte[] bytes = new byte[a1.Length + a2.Length + a3.Length];
            System.Buffer.BlockCopy(a1, 0, bytes, 0, a1.Length);
            System.Buffer.BlockCopy(a2, 0, bytes, a1.Length, a2.Length);
            System.Buffer.BlockCopy(a3, 0, bytes, a1.Length + a2.Length, a3.Length);

            byte[] preamble = GetBytes("<</Length " + a2.Length + (m_useCompression ? " /Filter/FlateDecode>>" : ">>"));
            byte[] finalBytes = new byte[bytes.Length + preamble.Length];
            System.Buffer.BlockCopy(preamble, 0, finalBytes, 0, preamble.Length);
            System.Buffer.BlockCopy(bytes, 0, finalBytes, preamble.Length, bytes.Length);

            return finalBytes;
           
		}

        private Color previousColor = new Color(0,0,0);

        /// <summary>
        /// Dibuja texto
        /// </summary>
        /// <param name="text">Text.</param>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="font">Font.</param>
        /// <param name="size">Size.</param>
        /// <param name="color">The color of the text</param>
        /// <param name="UseMatrix">Transformation matrix, null if not used</param>
		public void DrawText(string text, float x, float y, XrefFont font, float size, Color color, TransformMatrix matrix)
		{
			XrefPage page =  ((XrefPage)this.parent);
			XrefPageTree pages = ((XrefPageTree)page.parent);
			int fontNumber = page.m_resources.AddFont(font, pages);

			font.SetText(text);

            sb.Append(SetColor(color));            

		    sb.Append("BT /F" + fontNumber + " " + size.ToString(CultureInfo.InvariantCulture) + " Tf ");

			if (matrix != null) {
                sb.Append(matrix.ToString() + " " 
                          + GetNumber(x) + " " 
                          + GetNumber(y) + " Tm ");
            } else {
                sb.Append(GetNumber(x)  + " " + GetNumber(y)  + " Td ");
            }

            if (font.isUnicode) {
                sb.Append("(");
                foreach (char c in text) {
                    int glyphId = font.GetGlyphId(c);
                    int upperGlyphtId = (glyphId >> 8);
                    int lowerGlyphtId = (glyphId & 0x00ff);

                    if (m_useCompression) {
                        sb.Append((char)upperGlyphtId);
                        sb.Append((char)lowerGlyphtId);
                    } else {
                        sb.Append(glyphId.ToString() + ",");
                    }
                }
                sb.Append(") Tj ET ");
            } else {
    			sb.Append("(" + text.Replace("(", @"\(").Replace(")", @"\)") + ") Tj ET ");
            }
		}

        private string GetNumber(float number) {
            return Math.Round(number, 5).ToString(CultureInfo.InvariantCulture);
        }

        private string SetColor(Color color) {
            if (previousColor.Equals(color)) {
                previousColor = color;
                return color.GetPdfTextColor() + " rg ";
            }
            return "";            
        }

		public void DrawRectable(float x, float y, float width, float height, Color color) {
			sb.Append(x.ToString("F5", CultureInfo.InvariantCulture) + 
			              " " + y.ToString("F5", CultureInfo.InvariantCulture) + 
			              " " + width.ToString("F5", CultureInfo.InvariantCulture) +
			              " " + height.ToString("F5", CultureInfo.InvariantCulture) + " re " +		
			              SetColor(color) + "S ");			
		}

		public void DrawRectableFull(float x, float y, float width, float height, Color color) {
			sb.Append(x.ToString(CultureInfo.InvariantCulture) + 
			              " " + y.ToString(CultureInfo.InvariantCulture) + 
			              " " + width.ToString(CultureInfo.InvariantCulture) +
			              " " + height.ToString(CultureInfo.InvariantCulture) + " re " +		
			              SetColor(color) + "f S ");			
		}

        public void DrawImage(float x, float y, float width, float height, XrefImage img)  {            
            XrefPage page =  ((XrefPage)this.parent);
            XrefPageTree pages = ((XrefPageTree)page.parent);
            int imgNumber = page.m_resources.AddImage(img, pages);

            sb.Append("q " + GetNumber(width) + " 0 0 " + GetNumber(height) + " " + GetNumber(x) + " " + GetNumber(y) + " cm /Im" + imgNumber + " Do Q ");
        }

		private byte[] Flate(byte[] b) {            
            MemoryStream msOut = new MemoryStream();

            msOut.WriteByte(120);
            msOut.WriteByte(156);
       
            using (MemoryStream originalFileStream = new MemoryStream(b)) {
                using (DeflateStream compressionStream = new DeflateStream(msOut, CompressionMode.Compress)) {                    
                    originalFileStream.CopyTo(compressionStream);
                }
            }

            return msOut.ToArray();
		}
	}
}
