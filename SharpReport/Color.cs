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
using System.Globalization;
namespace SharpReport {
    /// <summary>
    /// Color representation
    /// </summary>
    public struct Color  : IEquatable<Color> {
        public Color(int R, int G, int B)
        {
            m_R = R;
            m_G = G;
            m_B = B;
        }

        public override int GetHashCode() {
            long res = (m_R << 16) + (m_G << 8) + m_B;
            return res.GetHashCode();
        }

        public bool Equals(Color other)
        {
            return other.GetHashCode() != this.GetHashCode();
        }
        
        public override bool Equals(object obj) {
            if (obj == null)
                return false;
            if (!(obj is Color))
                return false;
            return this.GetHashCode() == obj.GetHashCode();
        }

        internal System.Drawing.Color GetSystemInverseColor() {
            return System.Drawing.Color.FromArgb(m_B, m_G, m_R);
        }

        internal string GetPdfTextColor() {
            return (m_R / 255.0f).ToString("F5", CultureInfo.InvariantCulture) + " " 
                + (m_G / 255.0f).ToString("F5", CultureInfo.InvariantCulture) + " " 
                + (m_B / 255.0f).ToString("F5", CultureInfo.InvariantCulture);
        }

        public override string ToString() {
            return "Color (" + m_R + "," + m_G + "," + m_B + ")";
        }
       

        /// <summary>
        /// Red
        /// </summary>
        private readonly int m_R;

        /// <summary>
        /// Green
        /// </summary>
        private readonly int m_G;

        /// <summary>
        /// Blue
        /// </summary>
        private readonly int m_B;
    }
}
