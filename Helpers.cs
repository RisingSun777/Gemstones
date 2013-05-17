using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Gemstones
{
    public class Helpers
    {
        /*
         Determine if a point is within a rectangle
         */
        public static bool IsWithin(Rectangle rect, Point point) {
            if (point.X > rect.X && point.X <= (rect.X + rect.Width) &&
                point.Y > rect.Y && point.Y <= (rect.Y + rect.Height))
                return true;
            return false;
        }

        public static Bitmap LoadImage(string location) {
            Bitmap temp = null;
            try
            {
                temp = new Bitmap(location);
            }
            catch (Exception)
            {
                MessageBox.Show(location + " is missing.", GameMain.title, MessageBoxButtons.OK, MessageBoxIcon.Error);
                //Application.Exit();
            }
            return temp;
        }
    }
}
