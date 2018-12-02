using System;

namespace SharpReport
{
    public class Rectangle
    {
        private float m_x;
        private float m_y;
        private float m_width;
        private float m_height;

        public float x => m_x;
        public float y => m_y;
        public float width => m_width;
        public float height => m_height;

        public Rectangle(float x, float y, float width, float height) {
            m_x = x;
            m_y = y;
            m_width = width;
            m_height = height;
        }

        /// <summary>
		/// Hace un control tan grande como lo indicado por x,y, width and height.
		/// Esto se hace recursivamente
		/// </summary>
		/// <param name="rect">The new rectangle.</param>
		internal void AutoGrow(Rectangle rect)
		{
			if (rect.x < m_x) {
				m_width += (m_x - rect.x);
				m_x = rect.x;
			}
			if (rect.y < m_y) {
				m_height += (m_y - rect.y);
				m_y = rect.y;
			}

			if (rect.x + rect.width > m_x + m_width) {
				m_width = x - m_x + width;
			}

			if (rect.y + rect.height > m_y + m_height) {
				m_height = rect.y - m_y + rect.height;
			}
		}

        internal void AddY(float sumY)
        {
            m_y += sumY;
        }
    }
}