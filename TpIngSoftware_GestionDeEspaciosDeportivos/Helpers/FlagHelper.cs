using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TpIngSoftware_GestionDeEspaciosDeportivos.Helpers
{
    public static class FlagHelper
    {
        public static Bitmap GetFlag(string cultureCode)
        {
            Bitmap bmp = new Bitmap(24, 16);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.White);

                if (string.IsNullOrEmpty(cultureCode) || cultureCode.StartsWith("es"))
                {
                    // Mexico: Green, White, Red Vertical Stripes
                    float w = bmp.Width / 3f;
                    g.FillRectangle(Brushes.Green, 0, 0, w, bmp.Height);
                    g.FillRectangle(Brushes.White, w, 0, w, bmp.Height);
                    g.FillRectangle(Brushes.Red, w * 2, 0, w, bmp.Height);

                    // Eagle (simplified as a brown circle)
                    g.FillEllipse(Brushes.Brown, w + 2, 4, w - 4, 8);
                }
                else if (cultureCode.StartsWith("en"))
                {
                    // USA: Blue Canton, Red/White Stripes
                    // Stripes
                    float stripeHeight = bmp.Height / 7f; // 13 stripes too small, simplify to 7
                    for (int i = 0; i < 7; i++)
                    {
                        if (i % 2 == 0)
                            g.FillRectangle(Brushes.Red, 0, i * stripeHeight, bmp.Width, stripeHeight);
                        else
                            g.FillRectangle(Brushes.White, 0, i * stripeHeight, bmp.Width, stripeHeight);
                    }
                    // Blue Canton
                    g.FillRectangle(Brushes.Blue, 0, 0, bmp.Width * 0.4f, bmp.Height * 0.5f);
                }
                else
                {
                    // Generic
                    g.DrawRectangle(Pens.Black, 0, 0, bmp.Width - 1, bmp.Height - 1);
                    g.DrawString("?", new Font("Arial", 8), Brushes.Black, 4, 0);
                }
            }
            return bmp;
        }
    }
}
