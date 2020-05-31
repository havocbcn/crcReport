// This file is part of crcReport.
// 
// crcReport is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// crcReport is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with crcReport.  If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Runtime.Serialization;

namespace SharpReport
{
    [Serializable]
    public class SharpReportException : Exception
    {
        // This protected constructor is used for deserialization.
        protected SharpReportException( SerializationInfo info, 
            StreamingContext context ) :
                base( info, context )
        { }

        public SharpReportException(SharpReportExceptionCodes code, string description)
            : base(code.ToString() + ": " + description)
        {

        }    
    }
}