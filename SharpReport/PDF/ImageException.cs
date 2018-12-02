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
using System.Runtime.Serialization;

namespace SharpReport.PDF
{
    /// <summary>
    /// Exception dealing with images
    /// </summary>
    [Serializable]
    public class ImageException: Exception
    {
        public ImageException(string message) : base(message)
        {
        }

        public ImageException()
        {
        }

        public ImageException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }

        // Without this constructor, deserialization will fail
        protected ImageException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        {
        }
    }
}