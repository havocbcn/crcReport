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
            this.R = R;
            this.G = G;
            this.B = B;
        }

        public override int GetHashCode() {
            long res = (R << 16) + (G << 8) + B;
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
            return System.Drawing.Color.FromArgb(B, G, R);
        }

        internal string GetPdfTextColor() {
            return (R / 255.0f).ToString("F5", CultureInfo.InvariantCulture) + " " 
                + (G / 255.0f).ToString("F5", CultureInfo.InvariantCulture) + " " 
                + (B / 255.0f).ToString("F5", CultureInfo.InvariantCulture);
        }

        public override string ToString() {
            return "Color (" + R + "," + G + "," + B + ")";
        }
       

        /// <summary>
        /// Red
        /// </summary>
        internal int R { get; }
        
        /// <summary>
        /// Green
        /// </summary>
        internal int G { get; }

        /// <summary>
        /// Blue
        /// </summary>
        internal int B  { get; }

        /// <summary>
        /// Red
        /// </summary>
        internal float R_Normalized => (float)R / 255f;

        /// <summary>
        /// Green
        /// </summary>
        internal float G_Normalized => (float)G / 255f;

        /// <summary>
        /// Blue
        /// </summary>
        internal float B_Normalized => (float)B / 255f;
    }
}
