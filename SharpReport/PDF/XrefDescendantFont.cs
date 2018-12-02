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

//using System.Linq;
using System.Text;

namespace SharpReport.PDF {
	public class XrefDescendantFont : Xref {
        private readonly XrefFontTtfSubset m_font;

        private readonly XrefFontDescriptor m_fontDescriptor;

        public XrefDescendantFont(XrefFontTtfSubset font, XrefFontDescriptor fontDescriptor) {
			m_font = font;
			m_fontDescriptor = fontDescriptor;
		}

		public override byte[] Write()
		{				
			StringBuilder sb = new StringBuilder();

			sb.Append("<</Type/Font/Subtype/CIDFontType2/BaseFont/" + m_font.sixHash + "+" + m_font.fontName);
			sb.Append("/CIDSystemInfo<</Registry (Adobe)/Ordering (Identity)/Supplement 0>>/CIDToGIDMap/Identity /FontDescriptor " + m_fontDescriptor.ID + " 0 R /DW " + m_font.Width);
			sb.Append("/W [");

            int[] glyphList = new int[m_font.hashChar.Count];
            m_font.hashChar.CopyTo(glyphList);
            
            for (int i = 0; i < glyphList.Length; i++) {
                glyphList[i] = m_font.GetGlyphId(glyphList[i]);
            }

            int previousGlypthId = -100;
            bool isFirst = true;
            foreach (int glyphIndex in glyphList) { //.OrderBy(p => p)) {     
                if (previousGlypthId == glyphIndex - 1) {
                    sb.Append(" " + m_font.GetGlyph(glyphIndex).width); 
                } else {      
                    if (isFirst) {
                        isFirst = false;
                    } else {
                        sb.Append("] ");
                    }

                    sb.Append(glyphIndex.ToString() + " [" + m_font.GetGlyph(glyphIndex).width); 
                }

                previousGlypthId = glyphIndex;
                // mejora, si se va a escribir 2 o mas key consecutivos, se puede poner los 2 o mas values: 1 [10] 2 [20] => 1 [10 20]
            }
            sb.Append("]]>>");

			return GetBytes(sb.ToString());
		}
	}
}
