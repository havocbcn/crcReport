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

namespace SharpReport
{
	public class ReportImage : ReportDrawable
	{
		readonly byte[] m_image;

		/// <summary>
		/// An image
		/// </summary>
		/// <param name="report"></param>
		/// <param name="render"></param>
		/// <param name="pos"></param>
		/// <param name="image"></param>
		/// <returns></returns>
		public ReportImage (Report report, Render render, Rectangle pos, byte[] image) : base(report, render, pos) {
			m_image = image;
		}

		internal override void Draw(EPagePosition pagePosition, float pageXPos, float pageYPos, float pageZPos)
		{
			m_render.AddRenderElement(
				PageIndex,
				new RenderImage(pageXPos + position.x,
				                pageYPos + position.y,
				                pageZPos,
				                position.width,
				                position.height,
				                m_image)
			);
		}
	}
}
