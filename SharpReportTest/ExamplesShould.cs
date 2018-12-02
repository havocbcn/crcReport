using System;
using System.Collections.Generic;
using System.Globalization;
using SharpReport;
using Xunit;

namespace SharpReportTest
{
    public class ExamplesShould
    {
        [Fact]
        public void RenderAnAnualIncome()
        {
            Report report = new Report();

            Dictionary<string, float> departments = new Dictionary<string, float> {
                {"IT", 1}, {"Marketing", 0.8f}, {"Production", 1.2f}, {"R&D", 0.9f}, {"Purchasing", 1.2f}, {"Human Resources", 0.5f}, {"Accounting", 0.8f}
            };

            Dictionary<string, float> items = new Dictionary<string, float> {
                {"Salaries and wages", 100000},
                {"Other personal services", 50000},
                {"Consulting", 40000},
                {"Hardware", 100000},
                {"Software", 100000},
                {"Network infraestructure", 10000},
                {"Data processing", 10000},
                {"Training", 2000},
                {"Travel", 10000},					
                {"Other", 10000}
            };

            Dictionary<int, float> yearIncrease = new Dictionary<int, float> {
                {0, 1}, {1, 1.05f}, {2, 1.07f}, {3, 1.08f}, {4, 1.11f} 
                };

            report.SetBackgroundFunction((rb, obj) => {
                Font h1 = new Font("Times-Roman", 300, FontEmphasis.Bold);
                h1.SetColor(new Color(255, 0, 0));
                h1.SetAngle(45);
                rb.SetFont(h1);
                rb.AddLabel(6, 15, 8, "DRAFT");	
            }, null);

            report.SetHeaderFunction((rb, obj) => {
                Font h1 = new Font("Times-Roman", 80, FontEmphasis.Bold | FontEmphasis.Italic);
                Font h1minor = new Font("Times-Roman", 40, FontEmphasis.Bold | FontEmphasis.Italic);
                Font h1Left = new Font("Helvetica", 50, FontEmphasis.Bold);
                Font h1Middle = new Font("Helvetica", 50, FontEmphasis.Bold);

                if (rb.PageIndex == 0) {
                    rb.SetFont(h1);
                    rb.AddLabel(8, 0.1f, 8, "Company cost report");						
                    rb.SetFont(h1minor);
                    rb.AddLabel(8, 0.9f, 8, "221B Baker Street, London");	
                }

                float posY = rb.position.height;
                rb.SetFont(h1Left);
                rb.AddLabel(0, posY, 4, "Costs elements");

                int yearLabelX = 4; 
                int colorIndex = 200;
                int yearIni = DateTime.Now.Year;
                int yearEnd = yearIni + 5;
                rb.SetFont(h1Middle);
                for (int year = yearIni; year < yearEnd; year++) {
                    rb.AddLabel(yearLabelX, posY, 2, year.ToString());
                    rb.AddBox(yearLabelX, posY, 2, rb.position.height-posY, -1, true, new Color(colorIndex, colorIndex, 255));
                    colorIndex -= 20;
                    yearLabelX += 2;
                }
                rb.AddLabel(yearLabelX, posY, 2, "TOTAL");
                rb.AddBox(yearLabelX, posY, 2, rb.position.height-posY, -1, true, new Color(colorIndex, colorIndex, 200));
            }, null);
            
            report.SetBodyFunction(
                (rb, obj) => { 
                    Font regularLeft = new Font("Helvetica", 30);
                    Color gray = new Color(80, 80, 80);
                    Color lightGray = new Color(100, 100, 100);
                    regularLeft.SetColor(gray);

                    Font regularRight = new Font("Helvetica", 30);
                    regularRight.SetColor(gray);
                    regularRight.SetAlignment(EFontAlign.Right);

                    Font regularRightBold = new Font("Helvetica", 30, FontEmphasis.Bold);
                    regularRightBold.SetColor(gray);
                    regularRightBold.SetAlignment(EFontAlign.Right);

                    Font h1Left = new Font("Helvetica", 50, FontEmphasis.Bold);
                    Font h1Middle = new Font("Helvetica", 50, FontEmphasis.Bold);
                    h1Middle.SetAlignment(EFontAlign.Center);

                    Font h2Left = new Font("Helvetica", 40, FontEmphasis.Bold);
                    h2Left.SetColor(gray);
                    
                    int yearLabelX = 4; 
                    foreach (KeyValuePair<string, float> dept in departments) {		
                        rb.AddDetail(0, rb.position.height, (rbDept, objDept) => { 
                            rbDept.SetFont(h2Left);
                            rbDept.AddLabel(0.1f, rbDept.position.height + 0.5f, 3.8f, dept.Key);
                            rbDept.AddBox(0.1f, rbDept.position.height, 16, 0.05f, 0, true, lightGray);

                            float[] accum = new float[5];
                            foreach (KeyValuePair<string, float> item in items) {							
                                rbDept.AddDetail(0, rbDept.position.height, (rbItem, objItem) => { 
                                    rbItem.SetFont(regularLeft);
                                    rbItem.AddLabel(0.2f, 0.1f, 3.8f, item.Key);
                                    rbItem.SetFont(regularRight);
                                    yearLabelX = 4; 
                                    float itemSum = 0;
                                    float simulatedIncrease = 1;
                                    for (int year = 0; year < 5; year++) {
                                        simulatedIncrease *= yearIncrease[year];
                                        float value = item.Value * dept.Value * simulatedIncrease;
                                        accum[year] += value;
                                        rbItem.AddLabel(yearLabelX, 0.1f, 2.0f, value.ToString("$ #,###", CultureInfo.InvariantCulture));
                                        yearLabelX += 2;
                                        itemSum += value;
                                    }
                                    rbItem.SetFont(regularRightBold);
                                    rbItem.AddLabel(yearLabelX, 0.1f, 2.0f, itemSum.ToString("$ #,###", CultureInfo.InvariantCulture));
                                }, item, true);
                            }

                            rbDept.SetFont(regularRightBold);
                            rbDept.AddBox(0.1f, rbDept.position.height, 16, 0.05f, 0, true, new Color(100,100,100));
                            yearLabelX = 4; 
                            float posY = rbDept.position.height + 0.1f;
                            float deptSum = 0;
                            for (int year = 0; year < 5; year++) {
                                rbDept.AddLabel(yearLabelX, posY, 2.0f, accum[year].ToString("$ #,###", CultureInfo.InvariantCulture));
                                yearLabelX += 2;
                                deptSum += accum[year];
                            }
                            rbDept.AddLabel(yearLabelX, posY, 2.0f, deptSum.ToString("$ #,###", CultureInfo.InvariantCulture));

                        }, dept, true);								
                    }
                },
                null);
        
            Render render = report.Render();
            render.SavePDF("AnualIncome.pdf");            
            render.SaveTiff("AnualIncome.tiff");
        }
    

        [Fact]
        public void HelloWorld()
        {
            Report report = new Report();
            
            report.SetBodyFunction(
                (rb, obj) => { 
                    rb.AddLabel(5, 5, 20, "Hello World!");
                },
                null);
        
            Render render = report.Render();
            render.SavePDF("HelloWorld.pdf");            
            render.SaveTiff("HelloWorld.tiff");
        }

         [Fact]
        public void Standard14Fonts()
        {
            Report report = new Report();
            
            report.SetBodyFunction(
                (rb, obj) => { 
                    rb.SetFont(new Font("Courier", 100, FontEmphasis.None));
                    rb.AddLabel(0, 0, 20, "Courier");
                    rb.SetFont(new Font("Courier", 100, FontEmphasis.Bold));
                    rb.AddLabel(0, 1, 20, "Courier bold");
                    rb.SetFont(new Font("Courier", 100, FontEmphasis.Italic));
                    rb.AddLabel(0, 2, 20, "Courier italic");
                    rb.SetFont(new Font("Courier", 100, FontEmphasis.Bold | FontEmphasis.Italic));
                    rb.AddLabel(0, 3, 20, "Courier bold italic");
                    rb.SetFont(new Font("Helvetica", 100, FontEmphasis.None));
                    rb.AddLabel(0, 4, 20, "Helvetica");
                    rb.SetFont(new Font("Helvetica", 100, FontEmphasis.Bold));
                    rb.AddLabel(0, 5, 20, "Helvetica bold");
                    rb.SetFont(new Font("Helvetica", 100, FontEmphasis.Italic));
                    rb.AddLabel(0, 6, 20, "Helvetica italic");
                    rb.SetFont(new Font("Helvetica", 100, FontEmphasis.Bold | FontEmphasis.Italic));
                    rb.AddLabel(0, 7, 20, "Helvetica bold italic");
                    rb.SetFont(new Font("Times-Roman", 100, FontEmphasis.None));
                    rb.AddLabel(0, 8, 20, "Times-Roman");
                    rb.SetFont(new Font("Times-Roman", 100, FontEmphasis.Bold));
                    rb.AddLabel(0, 9, 20, "Times-Roman bold");
                    rb.SetFont(new Font("Times-Roman", 100, FontEmphasis.Italic));
                    rb.AddLabel(0, 10, 20, "Times-Roman italic");
                    rb.SetFont(new Font("Times-Roman", 100, FontEmphasis.Bold | FontEmphasis.Italic));
                    rb.AddLabel(0, 11, 20, "Times-Roman bold italic");
                    rb.SetFont(new Font("Symbol", 100, FontEmphasis.None));
                    rb.AddLabel(0, 12, 20, "Symbol");
                    rb.SetFont(new Font("ZapfDingbats", 100, FontEmphasis.None));
                    rb.AddLabel(0, 13, 20, "ZapfDingbats");
                },
                null);
        
            Render render = report.Render();
            render.SavePDF("Standard14Fonts.pdf");            
            render.SaveTiff("Standard14Fonts.tiff");
        }
    }
}