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

namespace SharpReport
{
    /// <summary>
    /// Un label
    /// </summary>
    public class ReportBox : ReportDrawable
    {
		/// <summary>
		/// Relleno
		/// </summary>
		private readonly bool m_IsFull;
		/// <summary>
		/// Rojo
		/// </summary>
		private readonly Color m_color;
		/// <summary>
		/// Como de atrás se debe dibujar
		/// </summary>
		private readonly float m_Z;

		/// <summary>
		/// A box
		/// </summary>
		/// <param name="report"></param>
		/// <param name="render"></param>
		/// <param name="pos"></param>
		/// <param name="Z"></param>
		/// <param name="isFull"></param>
		/// <param name="color"></param>
		/// <returns></returns>
        public ReportBox (Report report, Render render, Rectangle pos, float Z, bool isFull, Color color) : base(report, render, pos)
        {
            m_IsFull = isFull;
            m_color = color;
            m_Z = Z;
        }

        internal override void Draw(EPagePosition pagePosition, float pageXPos, float pageYPos, float pageZPos) {		
			m_render.AddRenderElement(
				PageIndex,
				new RenderBox(pageXPos + position.x,
				              pageYPos + position.y,
				              position.width, position.height,
				              pageZPos + m_Z,
				              m_IsFull,
				              m_color)
			);
        }
    }
}

