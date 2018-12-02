using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using SharpReport;

namespace test {
	public class MainClass {
        public const int iterations = 1;

		public static void Main () {
	        DateTime dtStart = DateTime.Now;

			Console.WriteLine("            working: 0%.");
            for (int i = 0; i < iterations; i++) {
                if (i % 10 == 9) {
                    Console.WriteLine(String.Format("            working: {0:0}%.", ((float)i / (float)iterations) * 100.0f));
                }

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
					Font h1 = new Font("Times New Roman", 300, FontEmphasis.Bold);
					h1.SetColor(new Color(255, 0, 0));
					h1.SetAngle(45);
					rb.SetFont(h1);
					rb.AddLabel(6, 15, 8, "DRAFT");	
				}, null);

				report.SetHeaderFunction((rb, obj) => {
					Font h1 = new Font("Times New Roman", 80, FontEmphasis.Bold | FontEmphasis.Italic);
					//Font h1 = new Font("/home/carlos/FreeSans.ttf", 80, FontEmphasis.Bold | FontEmphasis.Italic);
					//h1.SetEmbedded(FontPlacement.Embedded);
					Font h1minor = new Font("Times New Roman", 40, FontEmphasis.Bold | FontEmphasis.Italic);
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
						rb.AddBox(yearLabelX, posY, 2, rb.position.height-posY, -1, true, new Color(colorIndex, colorIndex, 255));
						rb.AddLabel(yearLabelX, posY, 2, year.ToString());
						colorIndex -= 20;
						yearLabelX += 2;
					}
					rb.AddBox(yearLabelX, posY, 2, rb.position.height-posY, -1, true, new Color(colorIndex, colorIndex, 200));
					rb.AddLabel(yearLabelX, posY, 2, "TOTAL");
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
                render.SavePDF("file.pdf");
                render.SaveTiff("file.tiff");
            }
            DateTime dtEnd = DateTime.Now;
            Console.WriteLine(String.Format("            working: {0:0}%.", ((float)iterations / (float)iterations) * 100.0f));
            Console.WriteLine("--------------------------------");
            Console.WriteLine(String.Format("         Total time: {0:0.000} segs.", (decimal)((dtEnd - dtStart).TotalMilliseconds) / 1000.0m));
            Console.WriteLine(String.Format("Each iteration time: {0:0.000} segs.", (decimal)((dtEnd - dtStart).TotalMilliseconds) / iterations / 1000.0m));
            FileInfo f = new FileInfo("file.pdf");
            float s1 = (float)f.Length / 1024f;
						
            Console.WriteLine(String.Format("      PDF File size: {0:0.000} Kb.", s1));
		}
	}
}
