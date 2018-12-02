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
namespace SharpReport.PDF
{
	public class XrefFontDescriptor : Xref
	{
        private readonly XrefFontTtfSubset m_font_subset;
        private readonly XrefFontTtf m_font;

        public XrefFontDescriptor(XrefFontTtfSubset font)
		{
			m_font_subset = font;
		}

		public XrefFontDescriptor(XrefFontTtf font)
		{
			m_font = font;
		}

		/// <summary>
		/// Write to pdf
		/// </summary>
		/// <returns></returns>
		public override byte[] Write() {			
			if (m_font_subset != null) {
				string str = "<</Type/FontDescriptor/FontName/" + m_font_subset.sixHash + "+" + m_font_subset.fontName + "/Flags " + (int)m_font_subset.fontsFlags;
				str += "/FontBBox [" + m_font_subset.boundingBox[0] + " " + m_font_subset.boundingBox[1] + " " + m_font_subset.boundingBox[2] + " " + m_font_subset.boundingBox[3] + "]";
				str += "/ItalicAngle " + m_font_subset.ItalicAngle;
				str += "/Ascent " + m_font_subset.Ascendent;
				str += "/Descent " + m_font_subset.Descendent;

				if (m_font_subset.Leading != 0)
					str += "/Leading " + m_font_subset.Leading;

				str += "/CapHeight " + m_font_subset.CapHeight;
				str += "/StemV " + m_font_subset.StemV;
				str += "/FontFile2 " + m_font_subset.GetContentID() + " 0 R>>";

				return GetBytes(str);
			} else {
				string str = "<</Type/FontDescriptor/StemV/" + m_font.StemV + "/Flags " + (int)m_font.fontsFlags;
				str += "/FontName /" + m_font.FontName;
				str += "/FontBBox [" + m_font.boundingBox[0] + " " + m_font.boundingBox[1] + " " + m_font.boundingBox[2] + " " + m_font.boundingBox[3] + "]";
				str += "/ItalicAngle " + m_font.ItalicAngle;
				str += "/Ascent " + m_font.Ascendent;
				str += "/Descent " + m_font.Descendent;

				if (m_font.Leading != 0)
					str += "/Leading " + m_font.Leading;

				str += "/CapHeight " + m_font.CapHeight;
				str += ">>";

				return GetBytes(str);
			}
		}
	}
}
