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
//using System.Linq;
using System.Text;

namespace SharpReport.PDF
{
	/// <summary>
	/// Representa el mapeo de glyph a unicode (CMAP)
	/// </summary>
	public class XrefFontCmap : Xref
	{		
		public override byte[] Write()
		{
			//	/CIDInit		/ProcSet findresource begin 12
			//		
			// 	/CIDSystemInfo
			//		/Registry 	A string identifying the issuer of the character collection. 
			//					For information about assigning a registry identifier, contact the Adobe   
			//					Solutions Network or consult the ASN Web site (see the Bibliography).
			//		/Ordering	A string that uniquely names the character collection within the specified registry.
			//		/Supplement	The supplement number of the character collection. An original character collection
			//					has a supplement number of  0. Whenever additional CIDs are assigned in a character 
			//					collection, the supplement number shall be increased. Supplements shall not alter 
			//					the ordering of existing CIDs in the character collection. This value shall not be 
			//					used in determining compatibility between character collections. 

			XrefFont font = ((XrefFont)parent);

            StringBuilder sb = new StringBuilder();

            sb.Append("/CIDInit/ProcSet findresource begin " + font.hashChar.Count + @" dict begin begincmap/CIDSystemInfo <</Registry (QWERT)/Ordering (ASDFG)/Supplement 0>> def /CMapName /QWERT def/CMapType 2 def 1 begincodespacerange
<0000><FFFF>
endcodespacerange
");

            sb.Append(font.hashChar.Count + " beginbfrange\n");
            foreach (int i in font.hashChar) {
                sb.Append("<" + font.GetGlyphId(i).ToString("x4") + "><" + font.GetGlyphId(i).ToString("x4") + "><" +  i.ToString("x4") + ">\n");    
                // TODO si el siguiente kvp.key es consecutivo y el siguiente unicode también, se puede poner un rango  <0001><0001><0010> <0002><0002><0011> => <0001><0002><0010>                
            }

            sb.Append("endbfrange\nendcmap\nCMapName currentdict /CMap defineresource pop\nend end\n");

  
            byte[] a1 = Encoding.GetEncoding(1252).GetBytes(@"
stream
");
            byte[] a2 = Encoding.GetEncoding(1252).GetBytes(sb.ToString());
            byte[] a3 = Encoding.GetEncoding(1252).GetBytes(@"
endstream
");

            byte[] bytes = new byte[a1.Length + a2.Length + a3.Length];
            System.Buffer.BlockCopy(a1, 0, bytes, 0, a1.Length);
            System.Buffer.BlockCopy(a2, 0, bytes, a1.Length, a2.Length);
            System.Buffer.BlockCopy(a3, 0, bytes, a1.Length + a2.Length, a3.Length);

            byte[] preamble = GetBytes("<</Length " + a2.Length + ">>");
            byte[] finalBytes = new byte[bytes.Length + preamble.Length];
            System.Buffer.BlockCopy(preamble, 0, finalBytes, 0, preamble.Length);
            System.Buffer.BlockCopy(bytes, 0, finalBytes, preamble.Length, bytes.Length);

            return finalBytes;
		}
	}
}
