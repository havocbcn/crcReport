namespace SharpReport.PDF
{
    public class XrefFontTtf : XrefFontTtfBase
    {
        public XrefFontTtf(string ttfFileName) : base(ttfFileName)
		{  		
            m_descriptor = new XrefFontDescriptor(this) {
                parent = this
            };

            this.sons.Add(m_descriptor);

        }
    }
}