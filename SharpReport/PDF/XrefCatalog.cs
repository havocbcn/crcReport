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
    /// <summary>
    /// Every object in the catalog
    /// </summary>
	public class XrefCatalog : Xref {		
		public XrefCatalog(XrefPageTree page) {			
			page.parent = this;
			this.sons.Add(page);
		}

		public void Traverse() {
			int c = 1;
			Traverse(this, ref c);
		}

        private void Traverse(Xref xref, ref int c) {
			xref.ID = c;
			c++;
			foreach (Xref son in xref.sons) {				
                Traverse(son, ref c);
			}
		}

		public override byte[] Write() {
			return GetBytes("<</Type /Catalog /Pages 2 0 R>>");
		}
	}
}
