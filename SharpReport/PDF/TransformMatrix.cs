using System;
using System.Globalization;

namespace SharpReport.PDF
{
    public class TransformMatrix
    {
        private readonly string m_pdfRepresentation;

        public TransformMatrix(float a, float b, float c, float d)
        {
            m_pdfRepresentation = GetNumber(a) + " " 
                          + GetNumber(b) + " " 
                          + GetNumber(c) + " " 
                          + GetNumber(d);
        }

        public override string  ToString() {
            return m_pdfRepresentation;
        }

        private string GetNumber(float number) {
            return Math.Round(number, 5).ToString(CultureInfo.InvariantCulture);
        }

    }
}