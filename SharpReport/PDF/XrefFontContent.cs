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
using System.IO;
using System.Text;

namespace SharpReport.PDF
{
	public class XrefFontContent : Xref
	{		
            private readonly bool m_useBase64;

            public XrefFontContent(bool useBase64) {
                  m_useBase64 = useBase64;         
            }
            
		public override byte[] Write()
		{
                  byte[] a1 = Encoding.GetEncoding(1252).GetBytes(@"
stream
");
                  byte[] a3 = Encoding.GetEncoding(1252).GetBytes(@"
endstream
");
                  XrefFont font = ((XrefFont)parent);
                  byte[] bytes = new byte[a1.Length + font.GetFont().Length + a3.Length];
                  System.Buffer.BlockCopy(a1, 0, bytes, 0, a1.Length);
                  System.Buffer.BlockCopy(font.GetFont(), 0, bytes, a1.Length, font.GetFont().Length);
                  System.Buffer.BlockCopy(a3, 0, bytes, a1.Length + font.GetFont().Length, a3.Length);

                  byte[] preamble = GetBytes("<</Length " + font.GetFont().Length + ">>");
                  byte[] finalBytes = new byte[bytes.Length + preamble.Length];
                  System.Buffer.BlockCopy(preamble, 0, finalBytes, 0, preamble.Length);
                  System.Buffer.BlockCopy(bytes, 0, finalBytes, preamble.Length, bytes.Length);
                  
                  if (m_useBase64) {
                        return GetBytes(Convert.ToBase64String(finalBytes));
                  }

                  return finalBytes;
		}
	}
}
