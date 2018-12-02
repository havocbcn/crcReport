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

using System;
using System.Collections.Generic;
using System.Text;

namespace SharpReport
{
	/// <summary>
	/// Un label
	/// </summary>
	public class ReportLabel : ReportDrawable
	{
		private const float degreesToRadiant = 0.01745329252f;

		/// <summary>
		/// El texto
		/// </summary>
		private readonly List<string> m_LstText;

		/// <summary>
		/// La fuente usada
		/// </summary>
		private readonly Font m_font;

		/// <summary>
		/// Si se tiene que rotar el control, como llegar a la posición inicial X
		/// </summary>
		private readonly float m_offsetX;

		/// <summary>
		/// Si se tiene que rotar el control, como llegar a la posición inicial Y
		/// </summary>
		private readonly float m_offsetY;

		/// <summary>
		/// A new label (for now, in memory for arrange)
		/// </summary>
		/// <param name="report">Report which this labels belongs</param>
        /// <param name="render">El render</param>
		/// <param name="pos">Position and size: (x,y),(width,height)</param>
		/// <param name="text">Text rendered</param>
		/// <param name="font">Font used</param>
		public ReportLabel (Report report, Render render, Rectangle pos, string text, Font font)
		{
			float fontWidth = font.GetWidth (text);			
			
			if (fontWidth > pos.width) {
                m_LstText = SplitTextInMultiplesLines(pos.width, text, font);
            }
            else {
				m_LstText = new List<string>();
				m_LstText.Add(text);
			}
			float height = font.GetHeight() * m_LstText.Count * font.GetLineSpacing;

			// puede que el texto esté rotado (m_font.angle != 0), en este caso el recuadro que forma 
			// (x,y) - (width, height) también debe rotarse tomando como centro (x,y)
			if (Math.Abs(font.Angle) > 0.001)
            {
                float minX, minY, maxX, maxY;
                CalculateAngleDisplacement(pos.width, font, height, out minX, out minY, out maxX, out maxY);

                // Ahora se encuadra en un nuevo cuadrado pero me quedo la diferencia entre el 0,0 original
                // y el 0,0 del nuevo cuadrado
                //  x,y > --------
                //   ^    |   /\ |
                // offset |  /  \|
                //   v    | /   /|
                //  0,0 > |/   / |
                //        |\  /  |
                //        | \/   |
                //        --------

                m_offsetX = minX;
                m_offsetY = minY;

				position = new Rectangle(pos.x + minX, pos.y + minY, maxX - minX, maxY - minY);
            }
			else {
				position = new Rectangle(pos.x, pos.y, pos.width, height);
			}

            m_font = font;
            m_render = render;
			m_report = report;
			PageIndex = m_report.GiveMePage(render, position.y + position.height);
		}

        private static List<string> SplitTextInMultiplesLines(float width, string text, Font font)
        {
			List<string> lstText = new List<string>();			
            float fontWidth;

            // el texto no cabe en el máximo ancho permitido
            // partir el texto introduciendo intros
            // se buscan los espacios, si hasta ese espacio cabe, perfect, se introduce. Sino va un intro
            int i = 0;            

            StringBuilder partialText = new StringBuilder();
            string partialTextUpToLastSpace = "";

            while (i < text.Length)
            {
                if (text[i] == '\n')
                {
                    lstText.Add(partialText.ToString());
                    partialText.Clear();                    
                }
                else if (text[i] == ' ')
                {
                    fontWidth = font.GetWidth(partialText.ToString());
                    if (fontWidth < width)
                    {
                        partialTextUpToLastSpace = partialText.ToString();
                        partialText.Append(text[i]);
                    }
                    else
                    {
                        // the text with the last word doesn't fit in a line 
                        // so I take up to the last word that fit (partialTextUpToLastSpace)
                        // but only if it's something in partialTextUpToLastSpace
                        // (there will be nothing if a line is filled with letters without spaces)
                        if (partialTextUpToLastSpace != "")
                        {
                            lstText.Add(partialTextUpToLastSpace);
                            // partialtext have to start with the last word
                            partialTextUpToLastSpace = partialText.ToString().Substring(partialTextUpToLastSpace.Length + 1);
                            partialText.Clear();
                            partialText.Append(partialTextUpToLastSpace + " ");
                        }
                        else
                        {
                            lstText.Add(partialText.ToString());
                            partialText.Clear();
                            partialTextUpToLastSpace = "";
                        }
                    }
                }
                else
                {
                    partialText.Append(text[i]);
                }
                i++;
            }

            // we were done, check if all fits
            fontWidth = font.GetWidth(partialText.ToString());
            if (fontWidth < width)
            {
                lstText.Add(partialText.ToString());
            }
            else
            {
                // the text with the last word doesn't fit in a line 
                // so I take up to the last word that fit (partialTextUpToLastSpace)
                // but only if it's something in partialTextUpToLastSpace
                // (there will be nothing if a line is filled with letters without spaces)
                if (partialTextUpToLastSpace != "")
                {
                    lstText.Add(partialTextUpToLastSpace);
                    // partialtext have to start with the last word
                    lstText.Add(partialText.ToString().Substring(partialTextUpToLastSpace.Length + 1));                    
                }
                else
                {
                    lstText.Add(partialText.ToString());
                }
            }
            
            return lstText;
        }

        private void CalculateAngleDisplacement(float width, Font font, float height, out float minX, out float minY, out float maxX, out float maxY)
        {
            //  0,0 > ------- < width,0
            //        |     |
            //  h,0 > ------- < width,height
            //
            //  After font rotation (45 degrees):
            //     
            //           /\  < posWidth0X, posWidth0Y
            //          /  \
            //         /   / < posWidthHeightX, posWidthHeightY
            //  0,0 > /   /
            //        \  /
            //         \/    < pos0HeightX, pos0HeightY

            float sinus = (float)Math.Sin(font.Angle * degreesToRadiant);
            float cosinus = (float)Math.Cos(font.Angle * degreesToRadiant);

            float posWidthHeightX = width * cosinus + height * sinus;
            float posWidthHeightY = width * sinus - height * cosinus;

            float posWidth0X = width * cosinus;
            float posWidth0Y = width * sinus;

            float pos0HeightX = height * sinus;
            float pos0HeightY = -height * cosinus;

            minX = Math.Min(Math.Min(Math.Min(0, posWidthHeightX), posWidth0X), pos0HeightX);
            minY = Math.Min(Math.Min(Math.Min(0, posWidthHeightY), posWidth0Y), pos0HeightY);
            maxX = Math.Max(Math.Max(Math.Max(0, posWidthHeightX), posWidth0X), pos0HeightX);
            maxY = Math.Max(Math.Max(Math.Max(0, posWidthHeightY), posWidth0Y), pos0HeightY);
        }

        /// <summary>
        /// Create a renderable element for this in memory label
        /// </summary>
        /// <param name="pagePosition">Page position.</param>
        /// <param name="pageXPos">initial page position X</param>
        /// <param name="pageYPos">initial page position Y</param>
        /// <param name="pageZPos">Depth position </param>
        internal override void Draw(EPagePosition pagePosition, float pageXPos, float pageYPos, float pageZPos)
		{				
			pageXPos -= m_offsetX;
			pageYPos -= m_offsetY;

			float fontHeight = m_font.GetHeight() * m_font.GetLineSpacing;
			float height = 0;

			foreach (string text in m_LstText) {
				string newText = m_report.Evaluate(text);
				float xPos;

				switch (m_font.Align) {
					case EFontAlign.Right:
						float fontWidth = m_font.GetWidth(newText);
						xPos = pageXPos + position.x + position.width - fontWidth;
						break;
					case EFontAlign.Center:
						float halfTextWidth = m_font.GetWidth(newText) / 2.0f;
						xPos = pageXPos + position.x + (position.width / 2.0f) - halfTextWidth;
						break;
					default:
						xPos = pageXPos + position.x;
						break;
				}

				m_render.AddRenderElement(
					PageIndex,
					new RenderLabel(xPos, 
					                pageYPos + position.y + height, 
					                pageZPos, 
					                newText, 
					                m_font)
				);

				// puede que el texto esté rotado (m_font.angle != 0), en este caso el recuadro que forma 
				// (x,y) - (width, height) también debe rotarse tomando como centro (x,y)
				if (Math.Abs(m_font.Angle) > 0.001) {
					float sinus = (float)Math.Sin(m_font.Angle * degreesToRadiant);
					float cosinus = (float)Math.Cos(m_font.Angle * degreesToRadiant);	

					float rotatedWidth = fontHeight * sinus;
					float rotatedHeight = fontHeight * cosinus;

					pageXPos += rotatedWidth;
					height += rotatedHeight;
				} else {
					height += fontHeight;
				}
			}
		}
	}
}

