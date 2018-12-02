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

namespace SharpReport.PDF
{
    /// <summary>
    /// A page in the pdf
    /// </summary>
    public class XrefPage : Xref {

		public XrefResources m_resources;
		public XrefContents m_contents;

		private readonly int m_width;
		private readonly int m_height;

		public XrefPage(XrefResources resources, XrefContents contents, int width, int height) {
			resources.parent = this;
			this.sons.Add(resources);

			contents.parent = this;
			this.sons.Add(contents);

			m_resources = resources;
			m_contents = contents;
			m_width = width;
			m_height = height;
		}

		public override byte[] Write() {
			return GetBytes("<</Type /Page /Parent " + parent.ID + " 0 R /Resources " + m_resources.ID + " 0 R /MediaBox [0 0 "+m_width+" "+m_height+"] /Contents " + m_contents.ID + " 0 R>>");
		}
	}
}
