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

using System.IO;
using System.Text;

namespace SharpReport.PDF
{
    /// <summary>
    /// A simple PDF library
    /// </summary>
	public class SharpPdf
	{		
		internal XrefCatalog catalog;
		private readonly XrefPageTree pageTree;
		private XrefContents contents;
		private XrefFont pdfFont;	// current font
		private float fontSize = 12.0f;
		readonly int width;
		readonly int height;
		private Color m_color;
		private TransformMatrix matrix;

		private bool m_useCompression = true;

		public void UseCompression(bool useCompression) {
			m_useCompression = useCompression;
		}

		public void SetFont(Font pdfFont) {
			this.pdfFont = pdfFont.GetPDFFont();
			this.SetColor(pdfFont.GetPDFColor);
		}

		public void SetFontSize(float size) {
			this.fontSize = size;
		}

		public SharpPdf(int width, int height)
		{
			System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

			pageTree = new XrefPageTree();
			catalog = new XrefCatalog(pageTree);

			pdfFont = XrefFontFactory.GetFont("Times-Roman", false, false, EEmbedded.NotEmbedded, false);

			this.width = width;
			this.height = height;
		}

		public void NewPage()
		{			
			XrefResources resources = new XrefResources();
			contents = new XrefContents(m_useCompression);
            XrefPage page = new XrefPage(resources, contents, width, height);
            pageTree.AddPage(page);
		}

		public void DrawText(string text, float x, float y)	{
			contents.DrawText(text, x, y, pdfFont, fontSize, m_color, matrix);
		}

		public void DrawRectangle(float x, float y, float width, float height) {
			contents.DrawRectable(x, y, width, height, m_color);
		}

		public void DrawRectangleFull(float x, float y, float width, float height) {
			contents.DrawRectableFull(x, y, width, height, m_color);
		}

        /// <summary>
        /// Draws an image, use XrefImageFactory to obtain the xrefimage
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="width">Width.</param>
        /// <param name="height">Height.</param>
        /// <param name="image">Image.</param>
        public void DrawImage(float x, float y, float width, float height, XrefImage image) {
            contents.DrawImage(x,y,width,height, image);
        }

		public void SetTextMatrix(TransformMatrix matrix) {
			this.matrix = matrix;
		}

		public void SetColor(Color color) {
			m_color = color;
		}

		public MemoryStream SaveStream() {
			catalog.Traverse();

			MemoryStream ms = new MemoryStream();

			Write(ms, "%PDF-1.3");

			// Catalog
			int numObj = Traverse(ms, catalog) + 1;
			long xRefPos = ms.Length;

			// cross reference table
			WriteLine(ms, "xref 0 " + numObj);
			WriteLine(ms, "0000000000 65535 f");

			TraverseTable(ms, catalog);

			WriteLine(ms, "trailer <</Size " + numObj + "/Root 1 0 R>>");
			Write(ms, "startxref " + xRefPos.ToString());
			Write(ms, "%%EOF");

			return ms;
		}

		int Traverse(MemoryStream ms, Xref xref) {
			int c = xref.ID;

			xref.position = ms.Length;
			Write(ms, xref.ID + " 0 obj");
			Write(ms, xref.Write());
			WriteLine(ms, "endobj");

			foreach (Xref son in xref.sons) {				
				int tempC = Traverse(ms, son);
				if (tempC > c)
					c = tempC;
			}

			return c;
		}

		void TraverseTable(MemoryStream ms, Xref xref)
		{
			WriteLine(ms, xref.position.ToString().PadLeft(10, '0') + " 00000 n");

			foreach (Xref son in xref.sons)
			{				
				TraverseTable(ms, son);
			}
		}

		void WriteLine(Stream stream, string text)
		{			
			byte[] textByte = Encoding.GetEncoding(1252).GetBytes(text + "\n");
			stream.Write(textByte, 0, textByte.Length);
		}

		void Write(Stream stream, string text)
		{			
			byte[] textByte = Encoding.GetEncoding(1252).GetBytes(text);
			stream.Write(textByte, 0, textByte.Length);
		}

		void Write(Stream stream, byte[] textByte)
		{						
			stream.Write(textByte, 0, textByte.Length);
		}
	}
}


