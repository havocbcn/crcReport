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
	/// cualquier objeto del report
	/// </summary>
	public class ReportBlock : ReportDrawable
	{
		/// <summary>
		/// Fuente usada para este bloque
		/// </summary>
        private Font currentFont;

		/// <summary>
		/// Un report
		/// </summary>
		/// <param name="report">Report.</param>
		public ReportBlock (Report report, Render render, float x, float y) : base(report, render, new Rectangle(x, y, 0, 0))
		{
            currentFont = new Font("Times New Roman", 50, FontEmphasis.None);
		}

		/// <summary>
        /// Adds the detail.
        /// </summary>
        /// <returns>The detail.</returns>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="detail">Detail.</param>
        /// <param name="data">Data.</param>
        /// <param name="keepTogether">If set to <c>true</c> keep together.</param>
        /// <param name="pageIndexForHeaderOrFooter">Los headers y footers están en posición 0, pero en una página concreta, este valor representa esta página</param>
        internal ReportBlock AddDetail(float x, float y, Action<ReportBlock, object> detail, object data, bool keepTogether, int pageIndexForHeaderOrFooter)
		{
			ReportBlock rbDetail = new ReportBlock(m_report, m_render, x, y + position.y) {
                parent = this
            };
			rbDetail.SetKeepTogether(keepTogether);

            rbDetail.PageIndex += pageIndexForHeaderOrFooter;

            if (detail != null) {
                detail.Invoke (rbDetail, data);
            }

            rbDetail.CheckKeepTogether();     

            AddSon(rbDetail);

			return rbDetail;
		}
		
		/// <summary>
        /// Adds the detail.
        /// </summary>
        /// <returns>The detail.</returns>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="detail">Detail.</param>
        /// <param name="data">Data.</param>
        /// <param name="KeepTogether">If set to <c>true</c> keep together.</param>
        /// <param name="pageIndexForHeaderOrFooter">Los headers y footers están en posición 0, pero en una página concreta, este valor representa esta página</param>
        public ReportBlock AddDetail(float x, float y, Action<ReportBlock, object> detail, object data, bool KeepTogether) {
			return AddDetail(x, y, detail, data, KeepTogether, 0);            
		}

        private void AddSon(ReportDrawable rb)
        {
            sons.Add (rb);                    // y el hijo de mi padre
            AutoGrow (rb);
        }

        public void AddVariable(string variableName, string value)
        {
            m_report.AddVariable(variableName, value);
        }

		/// <summary>
		/// Añade un label
		/// </summary>
		/// <param name="x">The x coordinate in cm</param>
		/// <param name="y">The y coordinate in cm</param>
		/// <param name="width">The width of the textbox</param>
		/// <param name="text">Text to show</param>
        public void AddLabel(float x, float y, float width, string text)
		{
			Rectangle rect = new Rectangle(x + position.x, y + position.y, width, 0);
            ReportLabel label = new ReportLabel(m_report, m_render, rect, text, currentFont) {
                parent = this
            };

            AddSon(label);
		}

		/// <summary>
		/// Añade un label
		/// </summary>
		/// <param name="x">The x coordinate in cm</param>
		/// <param name="y">The y coordinate in cm</param>
		/// <param name="width">Width in cm of the image</param>
		/// <param name="height">Height in cm of the image</param>
		/// <param name="image">The image itself</param>
		public void AddImage(float x, float y, float width, float height, byte[] image)
		{
			Rectangle rect = new Rectangle(x + position.x, y + position.y, width, height);
            ReportImage reportImage = new ReportImage(m_report, m_render, rect, image) {
                parent = this
            };

            AddSon(reportImage);
		}

        /// <summary>
        /// Añade una caja
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="width">Width.</param>
        /// <param name="height">Height.</param>
        /// <param name="isFull">If set to <c>true</c> is full.</param>
        /// <param name="color">Color</param>
        public void AddBox(float x, float y, float width, float height, float Z, bool isFull, Color color)
        {
			Rectangle rect = new Rectangle(x + position.x, y + position.y, width, height);
            ReportBox box = new ReportBox(m_report, m_render, rect, Z, isFull, color) {
                parent = this
            };

            AddSon(box);
        }

		/// <summary>
		/// Set the current font to draw
		/// </summary>
		/// <param name="font">Font.</param>
		public void SetFont (Font font) {
			currentFont = font;
        }

		/// <summary>
		/// Dibuja un documento
		/// </summary>
		/// <param name="pagePosition">Posición del objeto</param>
        internal override void Draw(EPagePosition pagePosition, float pageXPos, float pageYPos, float pageZPos) {
			float pageX = m_report.pageSize.GetMarginLeft;

			// se pinta cada bloque recursivamente
			foreach (ReportDrawable drawable in sons) {
				if (pagePosition == EPagePosition.Body) {
					// si este elemento está en alguna página nueva
					while (drawable.PageIndex >= m_render.GetPageCount()) {					
						m_render.AddPage();
						int page = m_render.GetPageCount()-1;
						m_report.rbBackground[page].Draw(EPagePosition.Background, 0, 0, -100);
						m_report.rbHeader[page].Draw(EPagePosition.Header, 0, 0, 0);
						m_report.rbFooter[page].Draw(EPagePosition.Footer, 0, 0, 0);
					}
				}
					
				switch (pagePosition) {
					case EPagePosition.Background:						
						drawable.Draw(pagePosition, 0, 0, -100);
						break;
					case EPagePosition.Header:
						float headerPageY = m_report.pageSize.GetMarginTop;
						drawable.Draw(pagePosition, pageX, headerPageY, 0);
						break;
					case EPagePosition.Body:
						float bodyPageY = m_report.rbHeader[drawable.PageIndex].position.height + m_report.pageSize.GetMarginTop - m_report.GiveMeTop(drawable.PageIndex);
						drawable.Draw(pagePosition, pageX, bodyPageY, 0);
						break;
					case EPagePosition.Footer:
						float footerPageY = m_report.pageSize.GetHeightInCM - m_report.rbFooter[drawable.PageIndex].position.height - m_report.pageSize.GetMarginBottom;
						drawable.Draw(pagePosition, pageX, footerPageY, 0);
						break;
				}                
			}
		}
	}
}

