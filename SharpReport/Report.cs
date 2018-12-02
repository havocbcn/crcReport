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

namespace SharpReport {
	/// <summary>
	/// Report, here are the magic
    /// </summary>
	public class Report {
		/// <summary>
		/// When drawing backgrounds, headers or footers the AddPage function is banned
		/// </summary>
        bool disallowCreateNewPages;

		/// <summary>
		/// Header user action
		/// </summary>
        Action<ReportBlock, object> m_actionHeader;

		/// <summary>
		/// Header user data
		/// </summary>
		object m_actionHeaderData;

		/// <summary>
		/// Footer user action
		/// </summary>
        Action<ReportBlock, object> m_actionFooter;

		/// <summary>
		/// Footer user data
		/// </summary>
        object m_actionFooterData;

		/// <summary>
		/// Body user action
		/// </summary>
        Action<ReportBlock, object> m_actionBody;

		/// <summary>
		/// Body user data
		/// </summary>
        object m_actionBodyData;

		/// <summary>
		/// Background user action
		/// </summary>
		Action<ReportBlock, object> m_actionBackground;

		/// <summary>
		/// Background user data
		/// </summary>
		object m_actionBackgroundData;

		/// <summary>
		/// All report variables
		/// </summary>
		private readonly Dictionary<string, string> dctVariables = new Dictionary<string, string>();

		internal List<ReportBlock> rbBackground = new List<ReportBlock>();
		internal List<ReportBlock> rbHeader = new List<ReportBlock>();
        internal List<ReportBlock> rbFooter = new List<ReportBlock>();

		/// <summary>
		/// The height of each page.
		/// </summary>
        internal List<float> heightListOfEachPage = new List<float>();

		internal PageSize pageSize = new PageSize(EPaperSizes.A4);

		/// <summary>
		/// Set the header user function to be call when rendering the report
		/// </summary>
		/// <param name="action">User action function</param>
		/// <param name="data">Data.</param>
		public void SetHeaderFunction(Action<ReportBlock, object> action, object data) {
			m_actionHeader = action;
			m_actionHeaderData = data;
		}

        /// <summary>
        /// Set the footer user function to be call when rendering the report
        /// </summary>
        /// <param name="action">User action function</param>
        /// <param name="data">Data.</param>
        public void SetFooterFunction(Action<ReportBlock, object> action, object data) {
            m_actionFooter = action;
            m_actionFooterData = data;
        }

		/// <summary>
		/// Set the body user function to be call when rendering the report
		/// </summary>
		/// <param name="action">User action function</param>
		/// <param name="data">Data.</param>
		public void SetBodyFunction(Action<ReportBlock, object> action, object data)
		{
			m_actionBody = action;
			m_actionBodyData = data;
		}

		/// <summary>
		/// Set the background user function to be call when rendering the report
		/// </summary>
		/// <param name="action">User action function</param>
		/// <param name="data">Data passed by</param>
		public void SetBackgroundFunction(Action<ReportBlock, object> action, object data) {
			m_actionBackground = action;
			m_actionBackgroundData = data;
		}		
	
		/// <summary>
		/// Render the report
		/// </summary>
		/// <returns>A report renderer</returns>
		public Render Render() {
            Render render = new Render (pageSize);

            return InternalRender(render);
		}

		/// <summary>
		/// Render the report
		/// </summary>
		/// <returns>A report renderer</returns>
		public Render Render(PageSize pageSize) {
			this.pageSize = pageSize;
            Render render = new Render (pageSize);

            return InternalRender(render);
		}

        /// <summary>
        ///  Render the report, only for unit testing
        /// <param name="render">The render to be used</param>
		/// <returns>A report renderer</returns>
        /// </summary>
        internal Render Render(Render render) {
            return InternalRender(render);
        }

        private Render InternalRender(Render render) {
            ReportBlock rbPage = new ReportBlock(this, render, 0, 0);

            rbPage.AddDetail (0, rbPage.position.height, m_actionBody, m_actionBodyData, false);

            // dibuja el body
            rbPage.Draw (EPagePosition.Body, 0, 0, 0);

            return render;
        }

        public void AddVariable(string variableName, string value)
        {
            if (dctVariables.ContainsKey(variableName)) {
                dctVariables[variableName] = value;
            } else {
                dctVariables.Add(variableName, value);
            }
        }

        internal string Evaluate(string text)
        {
            if (text.Contains("{")) {
                string newText = text.Replace("{PageCount}", heightListOfEachPage.Count.ToString());

                foreach (KeyValuePair<string, string> kvp in dctVariables) {
                    newText = newText.Replace("{" + kvp.Key + "}", kvp.Value);
                }

                return newText;
            }
            return text;
        }

        internal int GiveMePage (Render render, float y)
		{		
			float countY = 0;
			int pageIndex = 0;
			for (int i = 0; i < heightListOfEachPage.Count; i++)
			{
				if (y >= countY && y < countY + heightListOfEachPage[i])
					return pageIndex;
				pageIndex++;
				countY += heightListOfEachPage[i];
			}

			// has reach a new page
			AddPage (render);
			return pageIndex;
		}

		/// <summary>
		/// Return the top position of a page in continuous paper - bodyHeight.
		/// </summary>
		/// <returns>The me top.</returns>
		/// <param name="pageIndex">Page index.</param>
		internal float GiveMeTop (int pageIndex) {
			float countY = 0;
			
			for (int i = 0; i < pageIndex; i++) {
				countY += heightListOfEachPage [i];
			}

			return countY;
		}

		/// <summary>
		/// Añade una página
		/// </summary>
		private void AddPage(Render render)
		{
			if (disallowCreateNewPages)
				return;
			
			int pageIndex = heightListOfEachPage.Count;

            disallowCreateNewPages = true;
            			
        	rbBackground.Add(ExecuteUserCode(render, pageIndex, m_actionBackground, m_actionBackgroundData));
            rbHeader.Add(ExecuteUserCode(render, pageIndex, m_actionHeader, m_actionHeaderData));
            rbFooter.Add(ExecuteUserCode(render, pageIndex, m_actionFooter, m_actionFooterData));
                                        
            // set the available height of this page
            heightListOfEachPage.Add (pageSize.GetFreeHeightInCM - rbHeader[pageIndex].position.height - rbFooter[pageIndex].position.height);

			disallowCreateNewPages = false;
		}

        private ReportBlock ExecuteUserCode(Render render, int pageIndex, Action<ReportBlock, object> action, object data) {
            ReportBlock block = new ReportBlock (this, render, 0, 0);
            block.AddDetail (0, 0, action, data, false, pageIndex);
            block.AssignPageIndexRecursive (pageIndex);
            return block;
        }		
	}
}

