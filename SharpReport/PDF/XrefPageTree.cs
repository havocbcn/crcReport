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
using System.Text;

namespace SharpReport.PDF
{
    /// <summary>
    /// All pages in the pdf
    /// </summary>
	public class XrefPageTree : Xref
	{				
		public XrefPageTree()
		{
		}

		public Dictionary<XrefFont, int> m_lstFont = new Dictionary<XrefFont, int>();
        public Dictionary<XrefImage, int> m_lstImage = new Dictionary<XrefImage, int>();

		public int LastFontID = 1;
        public int LastImageID = 1;


		public void AddPage(XrefPage page)
		{
			page.parent = this;
			this.sons.Add(page);
		}

		public override byte[] Write()
		{
			StringBuilder ret = new StringBuilder();
			ret.Append ("<</Type /Pages /Kids [");
			bool first = true;
			foreach (Xref son in sons) {
				if (!first) {
					ret.Append(" ");
				}
				first = false;
				ret.Append(son.ID + " 0 R");
			}
			ret.Append("] /Count " + this.sons.Count + ">>");
			           
			return GetBytes(ret.ToString());
		}
	}
}
