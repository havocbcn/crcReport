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
using System.Collections.Generic;
//using System.Linq;
using BitMiracle.LibTiff.Classic;
using System.Drawing;
using System.Drawing.Imaging;
using SharpReport.PDF;

namespace SharpReport {
	/// <summary>
	/// Transforma un Report en un PDF o TIFF
	/// </summary>
	public class Render {
		readonly List<RenderPage> m_pages = new List<RenderPage>();

        readonly PageSize m_pageSize;

		/// <summary>
		/// For unittesting purposes
		/// </summary>
		/// <returns></returns>
        internal List<RenderPage> GetPages() {
            return m_pages;
        }

        public Render(PageSize pageSize) {
            m_pageSize = pageSize;
        }

        /// <summary>
        /// Graba el documento a disco en formato PDF
        /// </summary>
        /// <param name="fileName">fileName to create</param>
        public void SavePDF(string fileName)
        {
            byte[] pdfData = SavePDFToStream().ToArray();
            using (FileStream fs = new FileStream(fileName, FileMode.Create)) {
                fs.Write(pdfData, 0, pdfData.Length);
            }
        }

        /// <summary>
        /// Graba el documento en un stream en formato PDF
        /// </summary>
        public MemoryStream SavePDFToStream() {
            return RenderPDFToStream(m_pageSize);
        }

        /// <summary>
        /// Graba el documento a disco en formato TIFF
        /// </summary>
        /// <param name="fileName">fileName to create</param>
        public void SaveTiff(string fileName)
        {
            byte[] tiffData = SaveTiffToStream().ToArray();
            using (FileStream fs = new FileStream(fileName, FileMode.Create)) {
                fs.Write(tiffData, 0, tiffData.Length);
            }
        }

        /// <summary>
        /// Graba el documento a un stream en formato TIFF
        /// </summary>
        public MemoryStream SaveTiffToStream() {
            return RenderTiffToStream(m_pageSize);
        }

        internal void AddRenderElement(int page, RenderElement element) {
            while (m_pages.Count <= page) {
				AddPage();
			}

            m_pages[page].LstRender.Add(element);
		}

        /// <summary>
        /// Create a new page
        /// </summary>
		internal void AddPage() {
            m_pages.Add(new RenderPage());
		}

		internal int GetPageCount() {
            return m_pages.Count;
		}

		/// <summary>
		/// Render a report to a PDF Stream
		/// </summary>
		/// <returns>A MemoryStream</returns>
		/// <param name="pageSize">Pages.</param>
		private MemoryStream RenderPDFToStream (PageSize pageSize) {
            SharpPdf pdf = new SharpPdf(pageSize.GetWidthInPixels, pageSize.GetHeightInPixels);

            foreach (RenderPage page in m_pages) {
				pdf.NewPage ();

				foreach (RenderElement re in page.LstRender) {
					re.RenderPDF (pdf, pageSize);
				}
			}			

			return pdf.SaveStream();
		}             

		/// <summary>
		/// Render a report to a TIFF Stream
		/// </summary>
		/// <returns>A MemoryStream</returns>
		/// <param name="pageSize">Page dimension.</param>
        private MemoryStream RenderTiffToStream(PageSize pageSize) {           			
			MemoryStream ms = new MemoryStream();
			using (Tiff tif = Tiff.ClientOpen("in-memory", "w", ms, new TiffStream())) {
				int pageCount = 0;
                foreach (RenderPage page in m_pages) {
					byte[] raster;
					using (Bitmap bmp = new Bitmap(pageSize.GetWidthInPixels, pageSize.GetHeightInPixels, PixelFormat.Format24bppRgb)) {
						bmp.SetResolution(pageSize.GetDPI, pageSize.GetDPI);
						
						using (Graphics graphics = Graphics.FromImage(bmp)) {
							graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
							graphics.Clear(System.Drawing.Color.White);

							foreach (RenderElement re in page.LstRender) {
								re.RenderTIFF(graphics, pageSize);
							}
						}

						raster = GetImageRasterBytes(bmp, PixelFormat.Format24bppRgb);
					}

					tif.SetField(TiffTag.IMAGEWIDTH, pageSize.GetWidthInPixels);
					tif.SetField(TiffTag.IMAGELENGTH, pageSize.GetHeightInPixels);
					tif.SetField(TiffTag.COMPRESSION, Compression.LZW);
					tif.SetField(TiffTag.PHOTOMETRIC, Photometric.RGB);
					tif.SetField(TiffTag.ROWSPERSTRIP, pageSize.GetHeightInPixels);
					tif.SetField(TiffTag.RESOLUTIONUNIT, ResUnit.CENTIMETER);
					tif.SetField(TiffTag.BITSPERSAMPLE, 8);
					tif.SetField(TiffTag.SAMPLESPERPIXEL, 3);
					tif.SetField(TiffTag.PLANARCONFIG, PlanarConfig.CONTIG);
					tif.SetField(TiffTag.SUBFILETYPE, FileType.PAGE);			// specify that it's a page within the multipage file
                    tif.SetField(TiffTag.PAGENUMBER, pageCount, m_pages.Count);	// specify the page number

					int stride = raster.Length / pageSize.GetHeightInPixels;
					int offset = 0;

					for (int i = 0; i < pageSize.GetHeightInPixels; i++) {
						tif.WriteScanline(raster, offset, i, 0);
						offset += stride;
					}

					tif.WriteDirectory();

					pageCount++;
				}
			}
            return ms;
        }   

		/// <summary>
		/// Dada una imagen, devuelve un array de bytes de ella
		/// </summary>
		/// <returns>The image raster bytes.</returns>
		/// <param name="bmp">Bmp.</param>
		/// <param name="format">Format.</param>
        private byte[] GetImageRasterBytes(Bitmap bmp, PixelFormat format) {
            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height);
            
            // Lock the managed memory
            BitmapData bmpdata = bmp.LockBits(rect, ImageLockMode.ReadOnly, format);

            // Declare an array to hold the bytes of the bitmap.
			byte[] bits = new byte[bmpdata.Stride * bmpdata.Height];

            // Copy the values into the array.
            System.Runtime.InteropServices.Marshal.Copy(bmpdata.Scan0, bits, 0, bits.Length);

            // Release managed memory
            bmp.UnlockBits(bmpdata);

            return bits;
        }      
	}
}