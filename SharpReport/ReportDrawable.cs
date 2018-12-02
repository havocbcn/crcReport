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

namespace SharpReport
{
	public abstract class ReportDrawable
	{
		internal Report m_report;
        internal Render m_render;

		protected ReportDrawable() { }

        protected ReportDrawable(Report report, Render render, Rectangle pos)
		{
            m_report = report;
			m_render = render;
			PageIndex = m_report.GiveMePage(render, pos.y + pos.height);
			position = pos;
		}

		/// <summary>
		/// All the content must be together in one page (if possible)
		/// </summary>
		private bool m_KeepTogether;

		public void SetKeepTogether(bool keepTogether) {
			m_KeepTogether = keepTogether;
		}

		/// <summary>
		/// Draw a virtual element in memory
		/// </summary>
		/// <param name="pagePosition">Page position.</param>
		/// <param name="pageXPos">Page XP os.</param>
		/// <param name="pageYPos">Page YP os.</param>
		/// <param name="pageZPos">Page Z pos.</param>
		internal virtual void Draw(EPagePosition pagePosition, float pageXPos, float pageYPos, float pageZPos)
		{
			
		}

		/// <summary>
		/// x,y, width and height
		/// </summary>
		/// <returns></returns>
		public Rectangle position { get; set; }

		/// <summary>
		/// Page to draw into
		/// </summary>
		public int PageIndex { get; set; }

		internal  ReportBlock parent = null;

		internal  List<ReportDrawable> sons = new List<ReportDrawable>();

		/// <summary>
		/// Hace un control tan grande como lo indicado por x,y, width and height.
		/// Esto se hace recursivamente
		/// </summary>
		/// <param name="drawable">The control.</param>
		internal void AutoGrow(ReportDrawable drawable) {
			AutoGrow(drawable.position);
		}

		/// <summary>
		/// Hace un control tan grande como lo indicado por x,y, width and height.
		/// Esto se hace recursivamente
		/// </summary>
		/// <param name="rect">A rectangle</param>
		internal void AutoGrow(Rectangle rect)
		{
			position.AutoGrow(rect);

			if (parent != null) {
				parent.AutoGrow (rect);
			}
		}

        /// <summary>
        /// Assigns the page index to header and footer.
        /// </summary>
        /// <param name="pageIndex">Page index.</param>
        internal void AssignPageIndexRecursive(int pageIndex) {
            this.PageIndex = pageIndex;
            foreach (ReportDrawable rdSon in sons)
                rdSon.AssignPageIndexRecursive (pageIndex);
        }

        internal void CheckKeepTogether() {            
            // si alguno de mis hijos está en la siguiente página y yo tengo el keeptogether, nos movemos todos!
            if (m_KeepTogether && IsSplitedInPages(PageIndex, this)) {
				float sumY = m_report.GiveMeTop(PageIndex + 1) - position.y;
				position.AddY(sumY);				
				PageIndex++;

				foreach (ReportDrawable rd in sons)
					SumYToSon (PageIndex, rd, sumY);			
            }      
        }

        /// <summary>
        /// Devuelve si un hijo está en una página diferente que su padre
        /// </summary>
        /// <returns><c>true</c> if this instance is in another page,  otherwise, <c>false</c>.</returns>
        /// <param name="pageIndex">Página del padre</param>
        /// <param name="rd">Bloque a revisar</param>
        private bool IsSplitedInPages(int pageIndex, ReportDrawable rd) {
            if (rd.PageIndex != pageIndex)
                return true;

            foreach (ReportDrawable rdSon in rd.sons) {
                if (IsSplitedInPages(pageIndex, rdSon)) {
                    return true;
				}
            }

            return false;
        }

        /// <summary>
        /// Hace bajar a los hijos para que vayan a la siguiente página
        /// </summary>
        /// <param name="pageIndex">Página para que estén todos juntos</param>
        /// <param name="rd">El reportDrawable revisado</param>
        /// <param name="sumY">Cuanto hay que sumar a Y para que llegue</param>
        private void SumYToSon(int pageIndex, ReportDrawable rd, float sumY) {
            rd.position.AddY(sumY);
            rd.PageIndex = pageIndex;

            foreach (ReportDrawable rdSon in rd.sons) {
                SumYToSon(pageIndex, rdSon, sumY);
			}
        }
	}
}

