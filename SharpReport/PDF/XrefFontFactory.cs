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

using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using SharpReport.PDF.fonts;

namespace SharpReport.PDF
{
    /// <summary>
    /// Get a font
    /// </summary>
	public static class XrefFontFactory {
		private static Dictionary<string, XrefFont> m_lstFont = new Dictionary<string, XrefFont>();

		private static Dictionary<string, string> dctFontRegistered = new Dictionary<string, string>();
		private static object lck = new object();

		private static Dictionary<string,string> baseFontsNames = new Dictionary<string, string> {
			{ "timesnewroman", "Times-Roman"},
			{ "times", "Times-Roman"},
			{ "timesroman", "Times-Roman"},
			{ "times-roman", "Times-Roman"},
			{ "zapfdingbats", "ZapfDingbats"},
			{ "courier", "Courier"},
			{ "helvetica", "Helvetica"},
			{ "symbol", "Symbol"},
		};

		/*
		public static void SaveState(string filename) {
			XrefFont[] foos = new XrefFont[m_lstFont.Count];
			m_lstFont.Values.CopyTo(foos, 0);
			
			XmlSerializer serializer =  new XmlSerializer(typeof(XrefFont[] ));
      		using (TextWriter writer = new StreamWriter(filename)) {
				serializer.Serialize(writer, foos);
			}			
		}

		public static void LoadState(string filename) {
			XmlSerializer serializer =  new XmlSerializer(typeof(Dictionary<string, XrefFont>));
      		using (FileStream fs = new FileStream(filename, FileMode.Open)) {
				 m_lstFont = (Dictionary<string, XrefFont>) serializer.Deserialize(fs);
			}	
		}
		*/

		private static string GetName(string name, bool IsBold, bool IsItalic, EEmbedded embedded) {
			string normalizedName = name.ToLower().Replace(" ","");

			if (IsBold)
				normalizedName += "b";
			else
				normalizedName += "_";

			if (IsItalic)
				normalizedName += "i";
			else
				normalizedName += "_";

            if (embedded == EEmbedded.Embedded)
                normalizedName += "e";
            else
                normalizedName += "_";

			return normalizedName;
		}

		private static XrefFont GetBaseFont(string name, bool IsBold, bool IsItalic) {
			XrefFont font;

			switch (baseFontsNames[name.ToLower().Replace(" ","")]) {
                case "Times-Roman":
                    font = GetTimesRomanFont(IsBold, IsItalic);
                    break;
                case "Courier":
                    font = GetCourierFont(IsBold, IsItalic);
                    break;
                case "Helvetica":
                    font = GetHelveticaFont(IsBold, IsItalic);
                    break;
                default:
					font = new BaseFont(name);
					break;				
			}
			return font;
		}

        private static XrefFont GetHelveticaFont(bool IsBold, bool IsItalic)
        {
            if (IsBold && IsItalic)
                return new BaseFont("Helvetica-BoldOblique");
            else if (IsBold)
                return new BaseFont("Helvetica-Bold");
            else if (IsItalic)
                return new BaseFont("Helvetica-Oblique");
            else
                return new BaseFont("Helvetica");
        }

        private static XrefFont GetCourierFont(bool IsBold, bool IsItalic)
        {
            if (IsBold && IsItalic)
                return new BaseFont("Courier-BoldOblique");
            else if (IsBold)
                return new BaseFont("Courier-Bold");
            else if (IsItalic)
                return new BaseFont("Courier-Oblique");
            else
                return new BaseFont("Courier");
        }

        private static XrefFont GetTimesRomanFont(bool IsBold, bool IsItalic)
        {
            if (IsBold && IsItalic)
                return new BaseFont("Times-BoldItalic");
            else if (IsBold)
                return new BaseFont("Times-Bold");
            else if (IsItalic)
                return new BaseFont("Times-Italic");
            else
                return new BaseFont("Times-Roman");          
        }

        internal static XrefFont GetFont(string name, bool IsBold, bool IsItalic, EEmbedded embedded, bool useBase64)
		{	
			string normalizedName = GetName(name, IsBold, IsItalic, embedded);
            
			// cache
            lock (lck) {
    			if (m_lstFont.ContainsKey(normalizedName)) {
    				return m_lstFont[normalizedName];
    			}
            }

			// base fonts
			if (baseFontsNames.ContainsKey(name.ToLower().Replace(" ","")))
			{
				XrefFont font = GetBaseFont(name, IsBold, IsItalic);				

                lock (lck) {
				    m_lstFont.Add(normalizedName, font);
                }

				return font;
			}

            // unknown, or disk or systemfonts
            XrefFont ttffont = null;
			
			switch(embedded) {
				case EEmbedded.NotEmbedded:
					if (File.Exists(name)) {
						ttffont = new XrefFontTtf(name);
					} else {
						LoadSystemFonts();

						if (!dctFontRegistered.ContainsKey(name))
							throw new FontException("Font " + name + " not found");

						ttffont = new XrefFontTtf(dctFontRegistered[name]);
					}
					break;
				case EEmbedded.Embedded:
					if (File.Exists(name)) {
						ttffont = new XrefFontTtfSubset(name, useBase64);
					} else {
						LoadSystemFonts();

						if (!dctFontRegistered.ContainsKey(name))
							throw new FontException("Font " + name + " not found");

						ttffont = new XrefFontTtfSubset(dctFontRegistered[name], useBase64);
					}
					break;
            }

            lock (lck) {
			    m_lstFont.Add(normalizedName, ttffont);
            }
			return ttffont;
		}

		private static void LoadSystemFonts() {
			if (dctFontRegistered.Count == 0) 
			{
				lock (lck) {
					LoadFonts("./");
					LoadFonts("c:/windows/fonts");
					LoadFonts("c:/winnt/fonts");
					LoadFonts("d:/windows/fonts");
					LoadFonts("d:/winnt/fonts");

					LoadFonts("/usr/share/X11/fonts");
					LoadFonts("/usr/X/lib/X11/fonts");
					LoadFonts("/usr/openwin/lib/X11/fonts");
					LoadFonts("/usr/share/fonts");
					LoadFonts("/usr/X11R6/lib/X11/fonts");
					LoadFonts("/Library/Fonts");
					LoadFonts("/System/Library/Fonts");
				}
			}
		}

		private static void LoadFonts(string folder)
		{
			if (!Directory.Exists(folder))
				return;

			foreach (string file in Directory.GetFiles(folder, "*.ttf")) {
				string filenaWithoutExtension = Path.GetFileNameWithoutExtension(file);
				if (!dctFontRegistered.ContainsKey(filenaWithoutExtension))
					dctFontRegistered.Add(filenaWithoutExtension, file);
			}

			foreach (string dir in Directory.GetDirectories(folder))
				LoadFonts(dir);
			
		}
	}
}
