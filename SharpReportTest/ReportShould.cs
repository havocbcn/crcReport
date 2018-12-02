using Xunit;
using SharpReport;
using System.IO;
using System;

namespace ReportShould {    
    public class ReportShould {
        
        [Fact]
        public void RenderAnEmptyPageGivenAnEmptyReport() {
            Report report = new Report();
            Render renderToTest = new Render(new PageSize(EPaperSizes.A4));
            Render render = report.Render(renderToTest);

            Assert.Equal(1, render.GetPages().Count);
        }

        [Fact]
        public void RenderALabelGivenADetailWithAFixedLabel() {
            Report report = new Report();

            Render renderToTest = new Render(new PageSize(EPaperSizes.A4, 72, 2, 2, 2));

            report.SetBodyFunction(
                (rb, obj) => { rb.AddLabel(0,0, 10, "Hello"); },
                null);
            
            Render render = report.Render(renderToTest);

            Assert.Equal(1, render.GetPages().Count);
            Assert.Equal(1, render.GetPages()[0].LstRender.Count);
            Assert.Equal(2, render.GetPages()[0].LstRender[0].x);
            Assert.Equal(2, render.GetPages()[0].LstRender[0].y);
            Assert.Equal(0, render.GetPages()[0].LstRender[0].z);
        }

        [Fact]
        public void RenderTwoLabelsGivenADetailWithAFixedTwoLineLabelBecauseWidth() {
            Report report = new Report();

            Render renderToTest = new Render(new PageSize(EPaperSizes.A4, 72, 2, 2, 2));

            report.SetBodyFunction(
                (rb, obj) => { rb.AddLabel(0, 0, 3, "Hello Hello Hello Hello"); },
                null);

            Render render = report.Render(renderToTest);

            Assert.Equal(1, render.GetPages().Count);
            Assert.Equal(2, render.GetPages()[0].LstRender.Count);
            Assert.Equal(2, render.GetPages()[0].LstRender[0].x);
            Assert.Equal(2, render.GetPages()[0].LstRender[0].y);
            Assert.Equal(0, render.GetPages()[0].LstRender[0].z);
            Assert.Equal(2, render.GetPages()[0].LstRender[1].x);
            Assert.Equal(2.495, render.GetPages()[0].LstRender[1].y, 4);
            Assert.Equal(0, render.GetPages()[0].LstRender[1].z);
        }

         [Fact]
        public void RenderTwoLabelsGivenADetailWithAFixedTwoLineLabelBecauseNewLine() {
            Report report = new Report();

            Render renderToTest = new Render(new PageSize(EPaperSizes.A4, 72, 2, 2, 2));

            report.SetBodyFunction(
                (rb, obj) => { rb.AddLabel(0, 0, 3, "Hello Hello\nHello Hello"); },
                null);

            Render render = report.Render(renderToTest);

            Assert.Equal(1, render.GetPages().Count);
            Assert.Equal(2, render.GetPages()[0].LstRender.Count);
            Assert.Equal("Text [2,2] Z:0 Hello Hello Font: Font: Times Roman,0.5 no bold no italic Align:Left Angle:0 Color: Color (0,0,0)", render.GetPages()[0].LstRender[0].ToString());
            Assert.Equal("Text [2,2.495] Z:0 Hello Hello Font: Font: Times Roman,0.5 no bold no italic Align:Left Angle:0 Color: Color (0,0,0)", render.GetPages()[0].LstRender[1].ToString());
        }

          [Fact]
        public void RenderA45DegreesLabel() {
            Report report = new Report();

            Render renderToTest = new Render(new PageSize(EPaperSizes.A4, 72, 2, 2, 2));

            report.SetBodyFunction(
                (rb, obj) => { 
                    var font = new Font("Times", 10);
                    font.SetAngle(45);
                    rb.SetFont(font);
                    rb.AddLabel(0, 0, 3, "Hello"); 
                    },
                null);

            Render render = report.Render(renderToTest);

            Assert.Equal(1, render.GetPages().Count);
            Assert.Equal(1, render.GetPages()[0].LstRender.Count);
            Assert.Equal("Text [2,2] Z:0 Hello Font: Font: Times,0.1 no bold no italic Align:Left Angle:45 Color: Color (0,0,0)", render.GetPages()[0].LstRender[0].ToString());;
        }

        [Fact]
        public void RenderTwoPages() {
            Report report = new Report();

            Render renderToTest = new Render(new PageSize(EPaperSizes.A4, 72, 2, 2, 2));

            report.SetBodyFunction(
                (rb, obj) => { 
                    for (int i = 0; i < 5; i++) {
                        rb.AddDetail(0, rb.position.height, (detail, objDetail) => {
                            detail.AddLabel(0, 0, 1, "Hello");
                            for (int j = 0; j < 10; j++) {
                                detail.AddLabel(0, detail.position.height, 1, "InnerHello");
                            }
                        }, null, false, 0);
                    }
                },
                null);

            Render render = report.Render(renderToTest);

            Assert.Equal(2, render.GetPages().Count);
            Assert.Equal(51, render.GetPages()[0].LstRender.Count);
            Assert.Equal(4, render.GetPages()[1].LstRender.Count);
        }

        [Fact]
        public void RenderTwoPagesKeepingTogether() {
            Report report = new Report();

            Render renderToTest = new Render(new PageSize(EPaperSizes.A4, 72, 2, 2, 2));

            report.SetBodyFunction(
                (rb, obj) => { 
                    for (int i = 0; i < 5; i++) {
                        rb.AddDetail(0, rb.position.height, (detail, objDetail) => {
                            detail.AddLabel(0, 0, 1, "Hello");
                            for (int j = 0; j < 10; j++) {
                                detail.AddLabel(0, detail.position.height, 1, "InnerHello");
                            }
                        }, null, true, 0);
                    }
                },
                null);

            Render render = report.Render(renderToTest);

            Assert.Equal(2, render.GetPages().Count);
            Assert.Equal(44, render.GetPages()[0].LstRender.Count);
            Assert.Equal(11, render.GetPages()[1].LstRender.Count);
        }

        [Fact]
        public void RenderOnePageWithHeader() {
            Report report = new Report();

            Render renderToTest = new Render(new PageSize(EPaperSizes.A4, 72, 2, 2, 2));

            report.SetBodyFunction(
                (rb, obj) => { rb.AddLabel(0,0, 10, "Hello"); },
                null);

            report.SetHeaderFunction(
                (rb, obj) => { rb.AddLabel(0,0, 10, "Hello"); },
                null);
            
            Render render = report.Render(renderToTest);

            Assert.Equal(1, render.GetPages().Count);
            Assert.Equal(2, render.GetPages()[0].LstRender.Count);
            Assert.Equal(2, render.GetPages()[0].LstRender[0].x);
            Assert.Equal(2, render.GetPages()[0].LstRender[0].y);
            Assert.Equal(0, render.GetPages()[0].LstRender[0].z);
            Assert.Equal(2, render.GetPages()[0].LstRender[1].x);
            Assert.Equal(2.495, render.GetPages()[0].LstRender[1].y, 4);
            Assert.Equal(0, render.GetPages()[0].LstRender[1].z);
        }

        [Fact]
        public void RenderOnePageWithHeaderAndFooter() {
            Report report = new Report();

            Render renderToTest = new Render(new PageSize(EPaperSizes.A4, 72, 2, 2, 2));

            report.SetBodyFunction(
                (rb, obj) => { rb.AddLabel(0,0, 10, "Hello"); },
                null);

            report.SetHeaderFunction(
                (rb, obj) => { rb.AddLabel(0,0, 10, "Hello"); },
                null);

            report.SetFooterFunction(
                (rb, obj) => { rb.AddLabel(0,0, 10, "Hello"); },
                null);
            
            Render render = report.Render(renderToTest);

            Assert.Equal(1, render.GetPages().Count);
            Assert.Equal(3, render.GetPages()[0].LstRender.Count);
            Assert.Equal(2, render.GetPages()[0].LstRender[0].x);
            Assert.Equal(2, render.GetPages()[0].LstRender[0].y);
            Assert.Equal(0, render.GetPages()[0].LstRender[0].z);
            Assert.Equal(2, render.GetPages()[0].LstRender[1].x);
            Assert.Equal(27.205f, render.GetPages()[0].LstRender[1].y, 4);
            Assert.Equal(0, render.GetPages()[0].LstRender[1].z);
            Assert.Equal(2, render.GetPages()[0].LstRender[2].x);
            Assert.Equal(2.495, render.GetPages()[0].LstRender[2].y, 4);
            Assert.Equal(0, render.GetPages()[0].LstRender[2].z);
        }

        [Fact]
        public void RenderOneHundedPagesWithHeaderBodyBackgroundFooter() {
            Report report = new Report();

            Render renderToTest = new Render(new PageSize(EPaperSizes.A4, 72, 2, 2, 2));

            report.SetBodyFunction(
                (rb, obj) => { 
                    for (int i = 0; i < 400; i++) {
                        rb.AddDetail(0, rb.position.height, (detail, objDetail) => {
                            detail.AddLabel(0, 0, 1, "Hello");
                            for (int j = 0; j < 10; j++) {
                                detail.AddLabel(0, detail.position.height, 1, "InnerHello");
                            }
                        }, null, true, 0);
                    }
                },
                null);

            report.SetHeaderFunction(
                (rb, obj) => { rb.AddLabel(0,0, 10, "Hello"); },
                null);

            report.SetFooterFunction(
                (rb, obj) => { rb.AddLabel(0,0, 10, "Hello"); },
                null);
                
            report.SetBackgroundFunction(
                (rb, obj) => { rb.AddLabel(0,0, 10, "Hello"); },
                null);
            
            Render render = report.Render(renderToTest);

            Assert.Equal(100, render.GetPages().Count);
            for (int i = 0; i < 100; i++)
                Assert.Equal(47, render.GetPages()[i].LstRender.Count);            
        }

         [Fact]
        public void RenderABoxGivenADetailWithABox() {
            Report report = new Report();

            Render renderToTest = new Render(new PageSize(EPaperSizes.A4, 72, 2, 2, 2));

            report.SetBodyFunction(
                (rb, obj) => { rb.AddBox(0,0,10,10,10,false, new Color(10, 20, 30)); },
                null);
            
            Render render = report.Render(renderToTest);

            Assert.Equal(1, render.GetPages().Count);
            Assert.Equal(1, render.GetPages()[0].LstRender.Count);
            Assert.Equal("Box [2,2,10,10] Z:10 Color (10,20,30)", render.GetPages()[0].LstRender[0].ToString());
        }

        [Fact]
        public void RenderPdfWithHeaderForReal() {
            Report report = new Report();

            report.SetBodyFunction(
                (rb, obj) => { 
                    rb.AddLabel(0,0, 10, "Hello"); 
                    rb.AddBox(0,0, 10, 10, 0, false, new Color(10, 20, 50)); 
                },
                null);

            report.SetHeaderFunction(
                (rb, obj) => { 
                    rb.AddLabel(0,0, 10, "Hello"); 
                    rb.AddBox(0,0, 10, 10, 0, true, new Color(40, 20, 20));
                    },
                null);
            
            Render render = report.Render();
            MemoryStream ms = render.SavePDFToStream();
            byte[] byteMs = ms.ToArray();
            string actual = Convert.ToBase64String(byteMs);
            string expected = "JVBERi0xLjMxIDAgb2JqPDwvVHlwZSAvQ2F0YWxvZyAvUGFnZXMgMiAwIFI+PmVuZG9iagoyIDAgb2JqPDwvVHlwZSAvUGFnZXMgL0tpZHMgWzMgMCBSXSAvQ291bnQgMT4+ZW5kb2JqCjMgMCBvYmo8PC9UeXBlIC9QYWdlIC9QYXJlbnQgMiAwIFIgL1Jlc291cmNlcyA0IDAgUiAvTWVkaWFCb3ggWzAgMCAxNTEyIDIxMzhdIC9Db250ZW50cyA2IDAgUj4+ZW5kb2JqCjQgMCBvYmo8PC9Gb250IDw8L0YxIDUgMCBSPj4+PmVuZG9iago1IDAgb2JqPDwvVHlwZS9Gb250L1N1YnR5cGUvVHlwZTEvQmFzZUZvbnQvVGltZXMtUm9tYW4vRW5jb2RpbmcvV2luQW5zaUVuY29kaW5nPj5lbmRvYmoKNiAwIG9iajw8L0xlbmd0aCAxMjcgL0ZpbHRlciAvRmxhdGVEZWNvZGU+PgpzdHJlYW0KeJx1j8sKwlAMRPf9ilnqoprkxvvYFhT35hNsBSkI/f9Fc7U+UMwiMyGEM+kM2wMjRNgAVgWXWGBnrI79ON7WsCv2huaxKookhLa2qQdteBdzdKWUNbx0umDACY3PtfCtvu9+uKJ/uMsRS3paxy+ufdt7HgpF5COPf0O58jzNDAplbmRzdHJlYW0KZW5kb2JqCnhyZWYgMCA3CjAwMDAwMDAwMDAgNjU1MzUgZgowMDAwMDAwMDA4IDAwMDAwIG4KMDAwMDAwMDA1MyAwMDAwMCBuCjAwMDAwMDAxMDYgMDAwMDAgbgowMDAwMDAwMjA4IDAwMDAwIG4KMDAwMDAwMDI0NSAwMDAwMCBuCjAwMDAwMDAzMzMgMDAwMDAgbgp0cmFpbGVyIDw8L1NpemUgNy9Sb290IDEgMCBSPj4Kc3RhcnR4cmVmIDUyOSUlRU9G";

            File.WriteAllBytes("RenderPdfWithHeaderForReal.pdf", byteMs);

            Assert.Equal(expected, actual);
        }

         [Fact]
        public void RenderTiffWithHeaderForReal() {
            Report report = new Report();

            report.SetBodyFunction(
                (rb, obj) => { 
                    rb.AddLabel(0,0, 10, "Hello"); 
                    rb.AddBox(0,0, 10, 10, 0, false, new Color(10, 20, 50));
                },
                null);

            report.SetHeaderFunction(
                (rb, obj) => { 
                    rb.AddLabel(0,0, 10, "Hello"); 
                    rb.AddBox(0,0, 10, 10, 0, true, new Color(40, 20, 20));
                    },
                null);
            
            Render render = report.Render(new PageSize(EPaperSizes.A4, 1));
            MemoryStream ms = render.SaveTiffToStream();
            byte[] byteMs = ms.ToArray();
            string actual = Convert.ToBase64String(byteMs);
            string expected = "SUkqAMQAAAAAAAAAAAAAAIA/4FA4JBYNB4RCYVC4ZDYdD4YKAoFIlFInFYxF41Fo5EI9GY5IJFG4rHohI5DJJVHZNDpRL5XJZbDZhKZtIJnNJjO5vE5zEZ5NZxP4VQqDG6JRaPPZtSYTRqZQ6dBgUFBlVavVqxW61XazX6xU4JWHXZXhZ5zYbFArVTrbYrfRLjbqtcLra3/c7Td7Xepnfp/gJNgr/fKThJbiIfio9XLBXsdka9eMplctl8xmc1m85nbXAQ0A/gAEAAEAAAACAAAAAAEDAAEAAAAVAAAAAQEDAAEAAAAdAAAAAgEDAAMAAABmAQAAAwEDAAEAAAAFAAAABgEDAAEAAAACAAAAEQEEAAEAAAAQAAAAFQEDAAEAAAADAAAAFgEDAAEAAAAdAAAAFwEEAAEAAAC0AAAAHAEDAAEAAAABAAAAKAEDAAEAAAADAAAAKQEDAAIAAAAAAAEAAAAAAAgACAAIAA==";

            File.WriteAllBytes("RenderTiffWithHeaderForReal.tiff", byteMs);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void RenderPdfWithImageForReal() {
            Report report = new Report();

            report.SetBodyFunction(
                (rb, obj) => { 
                    rb.AddLabel(0,0, 10, "Hello"); 
                    rb.AddImage(10, 10, 10, 10, File.ReadAllBytes("logo.jpg"));
                },
                null);

            report.SetHeaderFunction(
                (rb, obj) => { 
                    rb.AddLabel(0,0, 10, "Hello"); 
                    rb.AddBox(0,0, 10, 10, 0, true, new Color(40, 20, 20));
                    },
                null);
            
            Render render = report.Render();
            MemoryStream ms = render.SavePDFToStream();
            byte[] byteMs = ms.ToArray();
            string actual = Convert.ToBase64String(byteMs);
            string expected = "JVBERi0xLjMxIDAgb2JqPDwvVHlwZSAvQ2F0YWxvZyAvUGFnZXMgMiAwIFI+PmVuZG9iagoyIDAgb2JqPDwvVHlwZSAvUGFnZXMgL0tpZHMgWzMgMCBSIDcgMCBSXSAvQ291bnQgMj4+ZW5kb2JqCjMgMCBvYmo8PC9UeXBlIC9QYWdlIC9QYXJlbnQgMiAwIFIgL1Jlc291cmNlcyA0IDAgUiAvTWVkaWFCb3ggWzAgMCAxNTEyIDIxMzhdIC9Db250ZW50cyA2IDAgUj4+ZW5kb2JqCjQgMCBvYmo8PC9Gb250IDw8L0YxIDUgMCBSPj4+PmVuZG9iago1IDAgb2JqPDwvVHlwZS9Gb250L1N1YnR5cGUvVHlwZTEvQmFzZUZvbnQvVGltZXMtUm9tYW4vRW5jb2RpbmcvV2luQW5zaUVuY29kaW5nPj5lbmRvYmoKNiAwIG9iajw8L0xlbmd0aCA5NyAvRmlsdGVyIC9GbGF0ZURlY29kZT4+CnN0cmVhbQp4nHMKUdB3M1QwNlMISVMwNDFRMLQ0s1QISVHQ8EjNycnXVAjJUnANUeCCSFmaKJgbGSjogoiiVAUDPUNTMwszIG1gbmFiDKeL0hXSFIIVuIB8EFBAp4HyThj2GplgsxcACmVuZHN0cmVhbQplbmRvYmoKNyAwIG9iajw8L1R5cGUgL1BhZ2UgL1BhcmVudCAyIDAgUiAvUmVzb3VyY2VzIDggMCBSIC9NZWRpYUJveCBbMCAwIDE1MTIgMjEzOF0gL0NvbnRlbnRzIDEwIDAgUj4+ZW5kb2JqCjggMCBvYmo8PC9Gb250IDw8L0YxIDUgMCBSPj4vWE9iamVjdCA8PC9JbTEgOSAwIFI+Pj4+ZW5kb2JqCjkgMCBvYmo8PC9GaWx0ZXIvRENURGVjb2RlL1R5cGUvWE9iamVjdC9MZW5ndGggNzExL0hlaWdodCA4My9XaWR0aCA2NC9CaXRzUGVyQ29tcG9uZW50IDgvQ29sb3JTcGFjZS9EZXZpY2VSR0IvU3VidHlwZS9JbWFnZT4+CnN0cmVhbQr/2P/gABBKRklGAAEBAQBIAEgAAP/+ADtDUkVBVE9SOiBnZC1qcGVnIHYxLjAgKHVzaW5nIElKRyBKUEVHIHY2MiksIHF1YWxpdHkgPSA5MAr/2wBDAFA3PEY8MlBGQUZaVVBfeMiCeG5uePWvuZHI////////////////////////////////////////////////////2wBDAVVaWnhpeOuCguv/////////////////////////////////////////////////////////////////////////wgARCABTAEADAREAAhEBAxEB/8QAFwABAQEBAAAAAAAAAAAAAAAAAAECA//EABQBAQAAAAAAAAAAAAAAAAAAAAD/2gAMAwEAAhADEAAAAdAAEBogABDRs5gAhToDBAAbKDAIDZQDABSgAyCgoAMlAKACAAFAIAAUAgABQD//xAAWEAEBAQAAAAAAAAAAAAAAAAABIGD/2gAIAQEAAQUCssxX/8QAFBEBAAAAAAAAAAAAAAAAAAAAYP/aAAgBAwEBPwEx/8QAFBEBAAAAAAAAAAAAAAAAAAAAYP/aAAgBAgEBPwEx/8QAFBABAAAAAAAAAAAAAAAAAAAAYP/aAAgBAQAGPwIx/8QAGRABAQEAAwAAAAAAAAAAAAAAASARADFQ/9oACAEBAAE/IbFjbDb74GWGWeb/AP/aAAwDAQACAAMAAAAQkAEkkAEAkgkAEgEAAgAAkkkkkEEkgEAEAEgAAkgAAkgA/8QAFBEBAAAAAAAAAAAAAAAAAAAAYP/aAAgBAwEBPxAx/8QAFBEBAAAAAAAAAAAAAAAAAAAAYP/aAAgBAgEBPxAx/8QAHBABAAMAAgMAAAAAAAAAAAAAAQARICExQVBh/9oACAEBAAE/EN28vW7XPW0VQAKNAqiAKNealT7qoAbl7PS//9kKZW5kc3RyZWFtCmVuZG9iagoxMCAwIG9iajw8L0xlbmd0aCAxMTAgL0ZpbHRlciAvRmxhdGVEZWNvZGU+PgpzdHJlYW0KeJw9yrEKwkAQhOH+nuIvtTDZNevmrhUVLcV7A02EkBDM+xd6CjIwXzGzz9QnpXFyj5qhyRP5wercjeO8Jg8cM+E3JaPdCptSS4dUuvPoH6WN1vxdnvTcCK/vu6QY3UhuVRRR5T5RXyblMHMlvAEKZW5kc3RyZWFtCmVuZG9iagp4cmVmIDAgMTEKMDAwMDAwMDAwMCA2NTUzNSBmCjAwMDAwMDAwMDggMDAwMDAgbgowMDAwMDAwMDUzIDAwMDAwIG4KMDAwMDAwMDExMiAwMDAwMCBuCjAwMDAwMDAyMTQgMDAwMDAgbgowMDAwMDAwMjUxIDAwMDAwIG4KMDAwMDAwMDMzOSAwMDAwMCBuCjAwMDAwMDA1MDQgMDAwMDAgbgowMDAwMDAwNjA3IDAwMDAwIG4KMDAwMDAwMDY2NyAwMDAwMCBuCjAwMDAwMDE1MjkgMDAwMDAgbgp0cmFpbGVyIDw8L1NpemUgMTEvUm9vdCAxIDAgUj4+CnN0YXJ0eHJlZiAxNzA5JSVFT0Y=";

            File.WriteAllBytes("RenderPdfWithImageForReal.pdf", byteMs);

            Assert.Equal(expected, actual);

        }

        [Fact]
        public void RenderTiffWithImageForReal() {
            Report report = new Report();

            report.SetBodyFunction(
                (rb, obj) => { 
                    rb.AddLabel(0,0, 10, "Hello"); 
                    rb.AddImage(10, 10, 10, 10, File.ReadAllBytes("logo.jpg"));
                },
                null);

            report.SetHeaderFunction(
                (rb, obj) => { 
                    rb.AddLabel(0,0, 10, "Hello"); 
                    rb.AddBox(0,0, 10, 10, 0, true, new Color(40, 20, 20));
                    },
                null);
            
            Render render = report.Render(new PageSize(EPaperSizes.A4, 1));
            MemoryStream ms = render.SaveTiffToStream();
            byte[] byteMs = ms.ToArray();
            string actual = Convert.ToBase64String(byteMs);
            string expected = "SUkqAJQAAAAAAAAAAAAAAIA/4FA4JBYNB4RCYVC4ZDYdD4YKAoFIlFInFYxF41Fo5EI9GY5IJFG4rHohI5DJJVHZNDpRL5XJZbDZhKZtIJnNJjO5vE5zEZ5NZxP4VQqDG6JRaPPZtSYTRqZQ6dU6pLXXV3hWarW65Xa9X7BYbFY7JZbNZ7RabVa7Zbbdb7hcbXAQAA0A/gAEAAEAAAACAAAAAAEDAAEAAAAVAAAAAQEDAAEAAAAdAAAAAgEDAAMAAAA2AQAAAwEDAAEAAAAFAAAABgEDAAEAAAACAAAAEQEEAAEAAAAQAAAAFQEDAAEAAAADAAAAFgEDAAEAAAAdAAAAFwEEAAEAAACDAAAAHAEDAAEAAAABAAAAKAEDAAEAAAADAAAAKQEDAAIAAAAAAAIAMgIAAAgACAAIAIA/4FA4JBYNB4RCYVC4ZDYdD4YKAoFIlFInFYxF41Fo5EI9GY5IJFG4rHohI5DJJVHZNDpRL5XJZbDZhKZtIFlOZ0rZ4qJ9P5+nqFQ6HBprR43Op3PaBQKGn6g+alRpjVZvE6VWZ1QaEm686rA/bFVKvZZQvLRWq3PkrbX9b3xcblZKRZonaLwvFve50mr9csBgbpVrqxsNh2TiWZi3RjXPj3dkclkpnEMPh2XmWhm25nXJn8nocrD8Pm9Np21qc/q9Zq9HDtPsdNqdo2tbrNfDdk1d5td9t9dueFw+JxeNx+RyeVy+Zzedz+h0el0+p1et14bAQA0A/gAEAAEAAAACAAAAAAEDAAEAAAAVAAAAAQEDAAEAAAAdAAAAAgEDAAMAAADUAgAAAwEDAAEAAAAFAAAABgEDAAEAAAACAAAAEQEEAAEAAAA8AQAAFQEDAAEAAAADAAAAFgEDAAEAAAAdAAAAFwEEAAEAAAD2AAAAHAEDAAEAAAABAAAAKAEDAAEAAAADAAAAKQEDAAIAAAABAAIAAAAAAAgACAAIAA==";

            File.WriteAllBytes("RenderTiffWithImageForReal.tiff", byteMs);

            Assert.Equal(expected, actual);
        }

    }
}
