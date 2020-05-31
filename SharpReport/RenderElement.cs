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

using System.Drawing;
using crcPdf;

namespace SharpReport
{
	/// <summary>
	/// Represents an element to be rendered
	/// </summary>
	internal abstract class RenderElement {
		internal float x;
		internal float y;
		internal float z;

		/// <summary>
		/// Draw this element to a PDF
		/// </summary>
		/// <param name="cb">PDF writer</param>
		internal abstract void RenderPDF(SimplePdf pdf, PageSize pageSize);

		/// <summary>
		/// Draw this element to a TIFF
		/// </summary>
		/// <param name="graphics">TIFF writer</param>
		internal abstract void RenderTIFF(Graphics graphics, PageSize pageSize);

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="z">The z coordinate.</param>
		protected RenderElement(float x, float y, float z) {
			this.x = x;
			this.y = y;
			this.z = z;
		}		
	}
}
