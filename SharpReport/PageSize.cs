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

namespace SharpReport {	
	public class PageSize {
		private readonly float m_width_cm;
		private readonly float m_height_cm;
		private readonly float m_dpi;
		private readonly float m_marginTop;
		private readonly float m_marginBottom;
		private readonly float m_marginLeft;

		public PageSize(EPaperSizes size, float dpi = 72, float marginTop = 2, float marginBottom = 2, float marginLeft = 2)
		{
			m_dpi = dpi;
			m_marginTop = marginTop;
			m_marginLeft = marginLeft;
			m_marginBottom = marginBottom;
			
			switch (size)
			{
				case EPaperSizes.A4:
					m_width_cm = 21;
					m_height_cm = 29.7f;
					break;
			}
		}

		public float GetWidthInCM {
			get {
				return m_width_cm;
			}
		}
		
		public int GetWidthInPixels {
			get {
				return (int)(m_width_cm * m_dpi);
			}
		}

		public float GetHeightInCM {
			get {			
				return m_height_cm;
			}
		}

        public float GetFreeHeightInCM {
            get {
                return GetHeightInCM - GetMarginTop - GetMarginBottom;
                }
        }
		
		public int GetHeightInPixels {
			get {			
				return (int)(m_height_cm * m_dpi);
			}
		}
		
		public float GetDPI	{
			get	{
				return m_dpi;
			}
		}

		public float GetMarginLeft {
			get { 
				return m_marginLeft;
			}
		}

		public float GetMarginTop {
			get {
				return m_marginTop;
			}
		}

		public float GetMarginBottom {
			get {
				return m_marginBottom;
			}
		}
	}
}
