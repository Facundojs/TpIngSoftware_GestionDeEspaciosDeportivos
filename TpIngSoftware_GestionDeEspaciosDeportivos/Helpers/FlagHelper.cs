using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;

namespace TpIngSoftware_GestionDeEspaciosDeportivos.Helpers
{
    public static class FlagHelper
    {
        public static Image DrawFlag(string cultureCode, int width, int height)
        {

            Bitmap bmp = new Bitmap(width, height);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.LightGray);
                g.SmoothingMode = SmoothingMode.AntiAlias;

                using (Font f = new Font("Arial", 6))
                {
                    SizeF textSize = g.MeasureString(cultureCode, f);
                    g.DrawString(cultureCode, f, Brushes.Black, (width - textSize.Width) / 2, (height - textSize.Height) / 2);
                }
                g.DrawRectangle(Pens.Black, 0, 0, width - 1, height - 1);
            }

            return bmp;
        }
    }
}