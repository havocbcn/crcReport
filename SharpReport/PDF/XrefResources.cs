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
using System.Text;

namespace SharpReport.PDF
{
	public class XrefResources : Xref
	{
        readonly Dictionary<XrefFont, int> m_lstFont = new Dictionary<XrefFont, int>();
        readonly Dictionary<XrefImage, int> m_lstImage = new Dictionary<XrefImage, int>();

		private static object lck = new object();

		public int AddFont(XrefFont font, XrefPageTree pages)
		{
			if (m_lstFont.ContainsKey(font))
				return m_lstFont[font];

			// esta página no tiene la fuente
			// puede que la tenga otra página, mejor la referencio
			int fontId ;
			if (pages.m_lstFont.ContainsKey(font))
			{
				fontId = pages.m_lstFont[font];
				m_lstFont.Add(font, fontId);
				return fontId;
			}

			lock (lck)
			{
				fontId  = pages.LastFontID;
				pages.LastFontID++;
			}

			m_lstFont.Add(font, fontId);		// tanto a esta página
			pages.m_lstFont.Add(font, fontId);	// como al diccionario de todas las páginas

			font.parent = this;
			this.sons.Add(font);

			return fontId;
		}

        public int AddImage(XrefImage img, XrefPageTree pages)
        {
            if (m_lstImage.ContainsKey(img))
                return m_lstImage[img];

            // esta página no tiene la imagen
            // puede que la tenga otra página, mejor la referencio
            int imageId;
            if (pages.m_lstImage.ContainsKey(img))
            {
                imageId = pages.m_lstImage[img];
                m_lstImage.Add(img, imageId);
                return imageId;
            }

            lock (lck)
            {
                imageId = pages.LastImageID;
                pages.LastImageID++;
            }

            m_lstImage.Add(img, imageId);        // tanto a esta página
            pages.m_lstImage.Add(img, imageId);  // como al diccionario de todas las páginas

            img.parent = this;
            this.sons.Add(img);

            return imageId;
        }

		public override byte[] Write()
		{
			StringBuilder sb = new StringBuilder();

            sb.Append("<<");
            if (m_lstFont.Count > 0)
            {
                sb.Append("/Font <<");
            
    			foreach (KeyValuePair<XrefFont, int> kvp in m_lstFont)
    			{
    				sb.Append("/F" + kvp.Value + " " + kvp.Key.ID + " 0 R");
    			}
                sb.Append(">>");
            }

            if (m_lstImage.Count > 0)
            {
                sb.Append("/XObject <<");

                foreach (KeyValuePair<XrefImage, int> kvp in m_lstImage)
                {
                    sb.Append("/Im" + kvp.Value + " " + kvp.Key.ID + " 0 R");
                }
                sb.Append(">>");
            }

            sb.Append(">>");
			return GetBytes(sb.ToString());
		}
	}
}
