using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Gemstones
{
    public class SelectorText
    {
        public string text { get; private set; }
        public Rectangle boundsRect { get; private set; }

        public SelectorText(string text, int x, int y, int width, int height) {
            this.text = text;
            boundsRect = new Rectangle(x, y, width, height);
        }

        public void Draw(Graphics g, Font font, Brush brush) {
            //if the mouse is hovering, draw a selection around the text
            if (Helpers.IsWithin(boundsRect, GameMain.cursorLocation))
                g.FillRectangle(Brushes.LightGray, boundsRect);

            //draw text
            g.DrawString(text, font, Brushes.Black, boundsRect.X + 3, boundsRect.Y + 3);
            g.DrawString(text, font, brush, boundsRect.X, boundsRect.Y);
        }
    }
}
