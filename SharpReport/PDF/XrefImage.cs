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
using System.Text;

namespace SharpReport.PDF
{
    public class XrefImage: Xref
    {
        private readonly IXrefImage m_image;

        private readonly bool m_useBase64;

        public XrefImage(byte[] img, bool useBase64)
        {
            if (img.Length < 3) {
                throw new ImageException("0 byte image not allowed");
            }

            // https://en.wikipedia.org/wiki/JPEG_File_Interchange_Format
            // SOI FF D8 Start of Image
            if (img[0] == 0xFF && img[1] == 0xD8) {     
                m_image = new XrefImageJpeg(img);
            } else {
                throw new ImageException("Image format not supported");
            }

            m_useBase64 = useBase64;
        }

        public override byte[] Write()
        {
            byte[] imageByte = m_image.Image();

            if (m_useBase64) {
                imageByte = GetBytes(Convert.ToBase64String(imageByte));
            }

            byte[] a1 = Encoding.GetEncoding(1252).GetBytes(@"<</Filter/" + m_image.Encoding() + "/Type/XObject/Length " + imageByte.Length + "/Height " + m_image.Height() + "/Width " + m_image.Width() + @"/BitsPerComponent " + m_image.BitsPerComponent() + @"/ColorSpace" + (m_image.Components() == 3 ? "/DeviceRGB" : "/DeviceGray") + @"/Subtype/Image>>
stream
");
            byte[] a3 = Encoding.GetEncoding(1252).GetBytes(@"
endstream
");

            byte[] bytes = new byte[a1.Length + imageByte.Length + a3.Length];
            System.Buffer.BlockCopy(a1, 0, bytes, 0, a1.Length);
            System.Buffer.BlockCopy(imageByte, 0, bytes, a1.Length, imageByte.Length);
            System.Buffer.BlockCopy(a3, 0, bytes, a1.Length + imageByte.Length, a3.Length);
                
            return bytes;
        }
    }
}
