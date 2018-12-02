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
    /// <summary>
    /// Indica si la fuente debe ser incrustada en el pdf
    /// </summary>
    public enum FontPlacement {
        /// <summary>
        /// The font is not embedded in the final document
        /// </summary>
        NotEmbedded = 0,

        /// <summary>
        /// the font in embedded in the final document if possible (subsetted)
        /// </summary>
        Embedded = 1,
    }
}

