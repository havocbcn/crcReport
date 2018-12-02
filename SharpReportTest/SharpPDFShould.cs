using Xunit;
using SharpReport;
using System.IO;
using System;
using SharpReport.PDF;

namespace SharpReportTest
{
    public class PDFShould
    {
        [Fact]
        public void CreateOnePagePDFTreeFromEmptyReport() {
            SharpReport.PDF.SharpPdf pdf = new SharpReport.PDF.SharpPdf(1000, 1000);
            pdf.UseCompression(false);
            pdf.NewPage();
            MemoryStream ms = pdf.SaveStream();
            string actual = System.Text.UTF8Encoding.UTF8.GetString(ms.ToArray());            
            string expected = @"%PDF-1.31 0 obj<</Type /Catalog /Pages 2 0 R>>endobj
2 0 obj<</Type /Pages /Kids [3 0 R] /Count 1>>endobj
3 0 obj<</Type /Page /Parent 2 0 R /Resources 4 0 R /MediaBox [0 0 1000 1000] /Contents 5 0 R>>endobj
4 0 obj<<>>endobj
5 0 objendobj
xref 0 6
0000000000 65535 f
0000000008 00000 n
0000000053 00000 n
0000000106 00000 n
0000000208 00000 n
0000000226 00000 n
trailer <</Size 6/Root 1 0 R>>
startxref 240%%EOF";
            
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CreateOnePagePDFTreeFromOneLabel() {
            SharpReport.PDF.SharpPdf pdf = new SharpReport.PDF.SharpPdf(1000, 1000);
            pdf.UseCompression(false);
            pdf.NewPage();
            pdf.DrawText("hola", 1, 1);
            MemoryStream ms = pdf.SaveStream();
            string actual = System.Text.UTF8Encoding.UTF8.GetString(ms.ToArray());            
            string expected = @"%PDF-1.31 0 obj<</Type /Catalog /Pages 2 0 R>>endobj
2 0 obj<</Type /Pages /Kids [3 0 R] /Count 1>>endobj
3 0 obj<</Type /Page /Parent 2 0 R /Resources 4 0 R /MediaBox [0 0 1000 1000] /Contents 6 0 R>>endobj
4 0 obj<</Font <</F1 5 0 R>>>>endobj
5 0 obj<</Type/Font/Subtype/Type1/BaseFont/Times-Roman/Encoding/WinAnsiEncoding>>endobj
6 0 obj<</Length 33>>
stream
BT /F1 12 Tf 1 1 Td (hola) Tj ET 
endstream
endobj
xref 0 7
0000000000 65535 f
0000000008 00000 n
0000000053 00000 n
0000000106 00000 n
0000000208 00000 n
0000000245 00000 n
0000000333 00000 n
trailer <</Size 7/Root 1 0 R>>
startxref 413%%EOF";
            
            Assert.Equal(expected, actual);
        }

           [Fact]
        public void CreateTwoPagePDFTreeFromTwoLabel_OptimizeRepeatedFont() {
            SharpReport.PDF.SharpPdf pdf = new SharpReport.PDF.SharpPdf(1000, 1000);
            pdf.UseCompression(false);
            pdf.NewPage();
            pdf.DrawText("hola", 1, 1);
            pdf.NewPage();
            pdf.DrawText("mundo", 1, 1);
            MemoryStream ms = pdf.SaveStream();
            string actual = System.Text.UTF8Encoding.UTF8.GetString(ms.ToArray());            
            string expected = @"%PDF-1.31 0 obj<</Type /Catalog /Pages 2 0 R>>endobj
2 0 obj<</Type /Pages /Kids [3 0 R 7 0 R] /Count 2>>endobj
3 0 obj<</Type /Page /Parent 2 0 R /Resources 4 0 R /MediaBox [0 0 1000 1000] /Contents 6 0 R>>endobj
4 0 obj<</Font <</F1 5 0 R>>>>endobj
5 0 obj<</Type/Font/Subtype/Type1/BaseFont/Times-Roman/Encoding/WinAnsiEncoding>>endobj
6 0 objBT /F1 12 Tf 1 1 Td (hola) Tj ET 
endobj
7 0 obj<</Type /Page /Parent 2 0 R /Resources 8 0 R /MediaBox [0 0 1000 1000] /Contents 9 0 R>>endobj
8 0 obj<</Font <</F1 5 0 R>>>>endobj
9 0 objBT /F1 12 Tf 1 1 Td (mundo) Tj ET 
endobj
xref 0 10
0000000000 65535 f
0000000008 00000 n
0000000053 00000 n
0000000112 00000 n
0000000214 00000 n
0000000251 00000 n
0000000339 00000 n
0000000387 00000 n
0000000489 00000 n
0000000526 00000 n
trailer <</Size 10/Root 1 0 R>>
startxref 575%%EOF";
            
            Assert.Equal(expected, actual);
        }


        [Fact]
        public void CreateOnePagePDFTreeFromOneLabel45Degrees() {
            SharpReport.PDF.SharpPdf pdf = new SharpReport.PDF.SharpPdf(1000, 1000);
            pdf.UseCompression(false);
            pdf.NewPage();

    		const float degreesToRadiant = 0.01745329252f;

            float sinus = (float)Math.Sin(45.0f * degreesToRadiant);
            float cosinus = (float)Math.Cos(45.0f * degreesToRadiant);
            pdf.SetTextMatrix(new TransformMatrix(cosinus, sinus, -sinus, cosinus));
                
            pdf.DrawText("hola", 1, 1);
            MemoryStream ms = pdf.SaveStream();
            string actual = System.Text.UTF8Encoding.UTF8.GetString(ms.ToArray());            
            string expected = @"%PDF-1.31 0 obj<</Type /Catalog /Pages 2 0 R>>endobj
2 0 obj<</Type /Pages /Kids [3 0 R] /Count 1>>endobj
3 0 obj<</Type /Page /Parent 2 0 R /Resources 4 0 R /MediaBox [0 0 1000 1000] /Contents 6 0 R>>endobj
4 0 obj<</Font <</F1 5 0 R>>>>endobj
5 0 obj<</Type/Font/Subtype/Type1/BaseFont/Times-Roman/Encoding/WinAnsiEncoding>>endobj
6 0 objBT /F1 12 Tf 0.70711 0.70711 -0.70711 0.70711 1 1 Tm (hola) Tj ET 
endobj
xref 0 7
0000000000 65535 f
0000000008 00000 n
0000000053 00000 n
0000000106 00000 n
0000000208 00000 n
0000000245 00000 n
0000000333 00000 n
trailer <</Size 7/Root 1 0 R>>
startxref 414%%EOF";
            
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CreateOnePagePDFTreeFromOneRectangle() {
            SharpReport.PDF.SharpPdf pdf = new SharpReport.PDF.SharpPdf(1000, 1000);
            pdf.UseCompression(false);
            pdf.NewPage();
            pdf.DrawRectangle(10, 10, 50, 50);
            MemoryStream ms = pdf.SaveStream();
            string actual = System.Text.UTF8Encoding.UTF8.GetString(ms.ToArray());            
            string expected = @"%PDF-1.31 0 obj<</Type /Catalog /Pages 2 0 R>>endobj
2 0 obj<</Type /Pages /Kids [3 0 R] /Count 1>>endobj
3 0 obj<</Type /Page /Parent 2 0 R /Resources 4 0 R /MediaBox [0 0 1000 1000] /Contents 5 0 R>>endobj
4 0 obj<<>>endobj
5 0 obj10.00000 10.00000 50.00000 50.00000 re S 
endobj
xref 0 6
0000000000 65535 f
0000000008 00000 n
0000000053 00000 n
0000000106 00000 n
0000000208 00000 n
0000000226 00000 n
trailer <</Size 6/Root 1 0 R>>
startxref 282%%EOF";
            
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CreateOnePagePDFTreeFromOneLabelCourierFont() {
            SharpReport.PDF.SharpPdf pdf = new SharpReport.PDF.SharpPdf(1000, 1000);
            pdf.UseCompression(false);
            pdf.NewPage();            
            pdf.SetFont(new Font("Courier", 12, FontEmphasis.Bold));
            pdf.DrawText("hola", 1, 1);
            MemoryStream ms = pdf.SaveStream();
            string actual = System.Text.UTF8Encoding.UTF8.GetString(ms.ToArray());            
            string expected = @"%PDF-1.31 0 obj<</Type /Catalog /Pages 2 0 R>>endobj
2 0 obj<</Type /Pages /Kids [3 0 R] /Count 1>>endobj
3 0 obj<</Type /Page /Parent 2 0 R /Resources 4 0 R /MediaBox [0 0 1000 1000] /Contents 6 0 R>>endobj
4 0 obj<</Font <</F1 5 0 R>>>>endobj
5 0 obj<</Type/Font/Subtype/Type1/BaseFont/Courier-Bold/Encoding/WinAnsiEncoding>>endobj
6 0 objBT /F1 12 Tf 1 1 Td (hola) Tj ET 
endobj
xref 0 7
0000000000 65535 f
0000000008 00000 n
0000000053 00000 n
0000000106 00000 n
0000000208 00000 n
0000000245 00000 n
0000000334 00000 n
trailer <</Size 7/Root 1 0 R>>
startxref 382%%EOF";
            
            Assert.Equal(expected, actual);
        }


        [Fact]
        public void CreateOnePagePDFTreeFromOneLabelAllFonts() {
            SharpReport.PDF.SharpPdf pdf = new SharpReport.PDF.SharpPdf(1000, 1000);
            pdf.UseCompression(false);
            pdf.NewPage();            
            pdf.SetFont(new Font("Courier", 12, FontEmphasis.None));
            pdf.DrawText("Courier", 1, 1);
            pdf.SetFont(new Font("Courier", 12, FontEmphasis.Bold));
            pdf.DrawText("Courier bold", 1, 2);
            pdf.SetFont(new Font("Courier", 12, FontEmphasis.Italic));
            pdf.DrawText("Courier italic", 1, 3);
            pdf.SetFont(new Font("Courier", 12, FontEmphasis.Bold | FontEmphasis.Italic));
            pdf.DrawText("Courier bold italic", 1, 4);
            pdf.SetFont(new Font("Helvetica", 12, FontEmphasis.None));
            pdf.DrawText("Helvetica", 1, 5);
            pdf.SetFont(new Font("Helvetica", 12, FontEmphasis.Bold));
            pdf.DrawText("Helvetica bold", 1, 6);
            pdf.SetFont(new Font("Helvetica", 12, FontEmphasis.Italic));
            pdf.DrawText("Helvetica italic", 1, 7);
            pdf.SetFont(new Font("Helvetica", 12, FontEmphasis.Bold | FontEmphasis.Italic));
            pdf.DrawText("Helvetica bold italic", 1, 8);
            pdf.SetFont(new Font("Symbol", 12, FontEmphasis.Bold));
            pdf.DrawText("Symbol", 1, 9);
            pdf.SetFont(new Font("Times-Roman", 12, FontEmphasis.None));
            pdf.DrawText("Times-Roman", 1, 10);
            pdf.SetFont(new Font("Times-Roman", 12, FontEmphasis.Bold));
            pdf.DrawText("Times-Roman bold", 1, 11);
            pdf.SetFont(new Font("Times-Roman", 12, FontEmphasis.Italic));
            pdf.DrawText("Times-Roman italic", 1, 12);
            pdf.SetFont(new Font("Times-Roman", 12, FontEmphasis.Bold | FontEmphasis.Italic));
            pdf.DrawText("Times-Roman bold italic", 1, 13);
            pdf.SetFont(new Font("ZapfDingbats", 12, FontEmphasis.None));
            pdf.DrawText("ZapfDingbats", 1, 14);
            MemoryStream ms = pdf.SaveStream();
            string actual = System.Text.UTF8Encoding.UTF8.GetString(ms.ToArray());            
            string expected = @"%PDF-1.31 0 obj<</Type /Catalog /Pages 2 0 R>>endobj
2 0 obj<</Type /Pages /Kids [3 0 R] /Count 1>>endobj
3 0 obj<</Type /Page /Parent 2 0 R /Resources 4 0 R /MediaBox [0 0 1000 1000] /Contents 19 0 R>>endobj
4 0 obj<</Font <</F1 5 0 R/F2 6 0 R/F3 7 0 R/F4 8 0 R/F5 9 0 R/F6 10 0 R/F7 11 0 R/F8 12 0 R/F9 13 0 R/F10 14 0 R/F11 15 0 R/F12 16 0 R/F13 17 0 R/F14 18 0 R>>>>endobj
5 0 obj<</Type/Font/Subtype/Type1/BaseFont/Courier/Encoding/WinAnsiEncoding>>endobj
6 0 obj<</Type/Font/Subtype/Type1/BaseFont/Courier-Bold/Encoding/WinAnsiEncoding>>endobj
7 0 obj<</Type/Font/Subtype/Type1/BaseFont/Courier-Oblique/Encoding/WinAnsiEncoding>>endobj
8 0 obj<</Type/Font/Subtype/Type1/BaseFont/Courier-BoldOblique/Encoding/WinAnsiEncoding>>endobj
9 0 obj<</Type/Font/Subtype/Type1/BaseFont/Helvetica/Encoding/WinAnsiEncoding>>endobj
10 0 obj<</Type/Font/Subtype/Type1/BaseFont/Helvetica-Bold/Encoding/WinAnsiEncoding>>endobj
11 0 obj<</Type/Font/Subtype/Type1/BaseFont/Helvetica-Oblique/Encoding/WinAnsiEncoding>>endobj
12 0 obj<</Type/Font/Subtype/Type1/BaseFont/Helvetica-BoldOblique/Encoding/WinAnsiEncoding>>endobj
13 0 obj<</Type/Font/Subtype/Type1/BaseFont/Symbol/Encoding/WinAnsiEncoding>>endobj
14 0 obj<</Type/Font/Subtype/Type1/BaseFont/Times-Roman/Encoding/WinAnsiEncoding>>endobj
15 0 obj<</Type/Font/Subtype/Type1/BaseFont/Times-Bold/Encoding/WinAnsiEncoding>>endobj
16 0 obj<</Type/Font/Subtype/Type1/BaseFont/Times-Italic/Encoding/WinAnsiEncoding>>endobj
17 0 obj<</Type/Font/Subtype/Type1/BaseFont/Times-BoldItalic/Encoding/WinAnsiEncoding>>endobj
18 0 obj<</Type/Font/Subtype/Type1/BaseFont/ZapfDingbats/Encoding/WinAnsiEncoding>>endobj
19 0 obj<</Length 614>>
stream
BT /F1 12 Tf 1 1 Td (Courier) Tj ET BT /F2 12 Tf 1 2 Td (Courier bold) Tj ET BT /F3 12 Tf 1 3 Td (Courier italic) Tj ET BT /F4 12 Tf 1 4 Td (Courier bold italic) Tj ET BT /F5 12 Tf 1 5 Td (Helvetica) Tj ET BT /F6 12 Tf 1 6 Td (Helvetica bold) Tj ET BT /F7 12 Tf 1 7 Td (Helvetica italic) Tj ET BT /F8 12 Tf 1 8 Td (Helvetica bold italic) Tj ET BT /F9 12 Tf 1 9 Td (Symbol) Tj ET BT /F10 12 Tf 1 10 Td (Times-Roman) Tj ET BT /F11 12 Tf 1 11 Td (Times-Roman bold) Tj ET BT /F12 12 Tf 1 12 Td (Times-Roman italic) Tj ET BT /F13 12 Tf 1 13 Td (Times-Roman bold italic) Tj ET BT /F14 12 Tf 1 14 Td (ZapfDingbats) Tj ET 
endstream
endobj
xref 0 20
0000000000 65535 f
0000000008 00000 n
0000000053 00000 n
0000000106 00000 n
0000000209 00000 n
0000000377 00000 n
0000000461 00000 n
0000000550 00000 n
0000000642 00000 n
0000000738 00000 n
0000000824 00000 n
0000000916 00000 n
0000001011 00000 n
0000001110 00000 n
0000001194 00000 n
0000001283 00000 n
0000001371 00000 n
0000001461 00000 n
0000001555 00000 n
0000001645 00000 n
trailer <</Size 20/Root 1 0 R>>
startxref 2308%%EOF";
            
            Assert.Equal(expected, actual);
        }
        
        [Fact]
        public void CreateOnePagePDFTreeFromOneLabelSystemFont() {
            SharpReport.PDF.SharpPdf pdf = new SharpReport.PDF.SharpPdf(1000, 1000);
            pdf.UseCompression(false);
            pdf.NewPage();            
            pdf.SetFont(new Font("FreeSerif", 12, FontEmphasis.None));
            pdf.DrawText("hola", 1, 1);
            MemoryStream ms = pdf.SaveStream();
            string actual = System.Text.UTF8Encoding.UTF8.GetString(ms.ToArray());            
            string expected = @"%PDF-1.31 0 obj<</Type /Catalog /Pages 2 0 R>>endobj
2 0 obj<</Type /Pages /Kids [3 0 R] /Count 1>>endobj
3 0 obj<</Type /Page /Parent 2 0 R /Resources 4 0 R /MediaBox [0 0 1000 1000] /Contents 7 0 R>>endobj
4 0 obj<</Font <</F1 5 0 R>>>>endobj
5 0 obj<</Encoding/WinAnsiEncoding/Type/Font/Subtype/TrueType/Widths [435 0 0 0 0 0 0 498 0 0 0 258 0 0 491]/FirstChar 97/LastChar 111/FontDescriptor 6 0 R/BaseFont/FreeSerif>>endobj
6 0 obj<</Type/FontDescriptor/StemV/80/Flags 32/FontName /FreeSerif/FontBBox [-879 -551 1767 936]/ItalicAngle 0/Ascent 900/Descent -200/CapHeight 729>>endobj
7 0 objBT /F1 12 Tf 1 1 Td (hola) Tj ET 
endobj
xref 0 8
0000000000 65535 f
0000000008 00000 n
0000000053 00000 n
0000000106 00000 n
0000000208 00000 n
0000000245 00000 n
0000000428 00000 n
0000000586 00000 n
trailer <</Size 8/Root 1 0 R>>
startxref 634%%EOF";
            
            Assert.Equal(expected, actual);
        }

         [Fact]
        public void CreateOnePagePDFTreeFromOneLabelSubsetSystemFont() {
            SharpReport.PDF.SharpPdf pdf = new SharpReport.PDF.SharpPdf(1000, 1000);
            SharpReport.Font font = new FontMock("FreeSerif", 12, FontEmphasis.Bold);
            font.SetEmbedded(FontPlacement.Embedded);
            pdf.UseCompression(false);
            pdf.NewPage();            
            pdf.SetFont(font);
            pdf.DrawText("hola", 1, 1);
            MemoryStream ms = pdf.SaveStream();
            string actual = System.Text.UTF8Encoding.UTF8.GetString(ms.ToArray());            
            string expected = @"%PDF-1.31 0 obj<</Type /Catalog /Pages 2 0 R>>endobj
2 0 obj<</Type /Pages /Kids [3 0 R] /Count 1>>endobj
3 0 obj<</Type /Page /Parent 2 0 R /Resources 4 0 R /MediaBox [0 0 1000 1000] /Contents 10 0 R>>endobj
4 0 obj<</Font <</F1 5 0 R>>>>endobj
5 0 obj<</Encoding /Identity-H/ToUnicode 9 0 R/Subtype /Type0/Type /Font/DescendantFonts [7 0 R]/BaseFont /IKWLZJ+FreeSans>>endobj
6 0 obj<</Type/FontDescriptor/FontName/IKWLZJ+FreeSans/Flags 32/FontBBox [-879 -551 1767 936]/ItalicAngle 0/Ascent 900/Descent -200/CapHeight 729/StemV 80/FontFile2 8 0 R>>endobj
7 0 obj<</Type/Font/Subtype/CIDFontType2/BaseFont/IKWLZJ+FreeSans/CIDSystemInfo<</Registry (Adobe)/Ordering (Identity)/Supplement 0>>/CIDToGIDMap/Identity /FontDescriptor 6 0 R /DW 1000/W [1 [498 491 258 435]]>>endobj
8 0 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endobj
9 0 obj<</Length 382>>
stream
/CIDInit/ProcSet findresource begin 4 dict begin begincmap/CIDSystemInfo <</Registry (QWERT)/Ordering (ASDFG)/Supplement 0>> def /CMapName /QWERT def/CMapType 2 def 1 begincodespacerange
<0000><FFFF>
endcodespacerange
4 beginbfrange
<0004><0004><0061>
<0001><0001><0068>
<0003><0003><006c>
<0002><0002><006f>
endbfrange
endcmap
CMapName currentdict /CMap defineresource pop
end end

endstream
endobj
10 0 objBT /F1 12 Tf 1 1 Td (1,2,3,4,) Tj ET 
endobj
xref 0 11
0000000000 65535 f
0000000008 00000 n
0000000053 00000 n
0000000106 00000 n
0000000209 00000 n
0000000246 00000 n
0000000377 00000 n
0000000556 00000 n
0000000774 00000 n
0000004868 00000 n
0000005298 00000 n
trailer <</Size 11/Root 1 0 R>>
startxref 5351%%EOF";
            
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CreateOnePagePDFTreeFromJpeg() {
            SharpReport.PDF.SharpPdf pdf = new SharpReport.PDF.SharpPdf(1000, 1000);
            pdf.UseCompression(false);
            pdf.NewPage();
    		pdf.DrawImage(1, 1, 10, 10, new XrefImage(File.ReadAllBytes("logo.jpg"), true));

            MemoryStream ms = pdf.SaveStream();
            string actual = System.Text.UTF8Encoding.UTF8.GetString(ms.ToArray());            
            string expected = @"%PDF-1.31 0 obj<</Type /Catalog /Pages 2 0 R>>endobj
2 0 obj<</Type /Pages /Kids [3 0 R] /Count 1>>endobj
3 0 obj<</Type /Page /Parent 2 0 R /Resources 4 0 R /MediaBox [0 0 1000 1000] /Contents 6 0 R>>endobj
4 0 obj<</XObject <</Im1 5 0 R>>>>endobj
5 0 obj<</Filter/DCTDecode/Type/XObject/Length 948/Height 83/Width 64/BitsPerComponent 8/ColorSpace/DeviceRGB/Subtype/Image>>
stream
/9j/4AAQSkZJRgABAQEASABIAAD//gA7Q1JFQVRPUjogZ2QtanBlZyB2MS4wICh1c2luZyBJSkcgSlBFRyB2NjIpLCBxdWFsaXR5ID0gOTAK/9sAQwBQNzxGPDJQRkFGWlVQX3jIgnhubnj1r7mRyP///////////////////////////////////////////////////9sAQwFVWlp4aXjrgoLr/////////////////////////////////////////////////////////////////////////8IAEQgAUwBAAwERAAIRAQMRAf/EABcAAQEBAQAAAAAAAAAAAAAAAAABAgP/xAAUAQEAAAAAAAAAAAAAAAAAAAAA/9oADAMBAAIQAxAAAAHQABAaIAAQ0bOYAIU6AwQAGygwCA2UAwAUoAMgoKADJQCgAgABQCAAFAIAAUA//8QAFhABAQEAAAAAAAAAAAAAAAAAASBg/9oACAEBAAEFArLMV//EABQRAQAAAAAAAAAAAAAAAAAAAGD/2gAIAQMBAT8BMf/EABQRAQAAAAAAAAAAAAAAAAAAAGD/2gAIAQIBAT8BMf/EABQQAQAAAAAAAAAAAAAAAAAAAGD/2gAIAQEABj8CMf/EABkQAQEBAAMAAAAAAAAAAAAAAAEgEQAxUP/aAAgBAQABPyGxY2w2++Blhlnm/wD/2gAMAwEAAgADAAAAEJABJJABAJIJABIBAAIAAJJJJJBBJIBABABIAAJIAAJIAP/EABQRAQAAAAAAAAAAAAAAAAAAAGD/2gAIAQMBAT8QMf/EABQRAQAAAAAAAAAAAAAAAAAAAGD/2gAIAQIBAT8QMf/EABwQAQADAAIDAAAAAAAAAAAAAAEAESAhMUFQYf/aAAgBAQABPxDdvL1u1z1tFUACjQKogCjXmpU+6qAG5ez0v//Z
endstream
endobj
6 0 obj<</Length 30>>
stream
q 10 0 0 10 1 1 cm /Im1 Do Q 

endstream
endobj
xref 0 7
0000000000 65535 f
0000000008 00000 n
0000000053 00000 n
0000000106 00000 n
0000000208 00000 n
0000000249 00000 n
0000001348 00000 n
trailer <</Size 7/Root 1 0 R>>
startxref 1425%%EOF";
            
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void RaiseExceptionBecauseNoFontFound() {
            SharpReport.PDF.SharpPdf pdf = new SharpReport.PDF.SharpPdf(1000, 1000);
            pdf.UseCompression(false);
            pdf.NewPage();            

            Exception ex = Assert.Throws<FontException>(() => pdf.SetFont(new Font("NOFOUND", 12, FontEmphasis.None)));            
            
            Assert.Equal("Font NOFOUND not found", ex.Message);
        }
    }
}