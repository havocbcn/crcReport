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

namespace SharpReport.PDF {
    /// <summary>
    /// Image repository
    /// </summary>
    public static class XrefImageFactory {

        private static Dictionary<byte[], XrefImage> dct = new Dictionary<byte[], XrefImage>();
        private static object lck = new object();

        /// <summary>
        /// Gets the image JPEG.
        /// </summary>
        /// <returns>The image JPEG.</returns>
        /// <param name="image">Image.</param>
        public static XrefImage GetImageJpeg(byte[] image) {
            lock (lck) {
                if (dct.ContainsKey(image))
                    return dct[image];
            }
            
            XrefImage img = new XrefImage(image, false);

            lock (lck) {
                dct.Add(image, img);
            }

            return img;
        }
    }
}
