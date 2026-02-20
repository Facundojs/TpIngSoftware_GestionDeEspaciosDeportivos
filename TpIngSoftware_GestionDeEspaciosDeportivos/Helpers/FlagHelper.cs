using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace TpIngSoftware_GestionDeEspaciosDeportivos.Helpers
{
    public static class FlagHelper
    {
        public static Image DrawFlag(string cultureCode, int width, int height)
        {
            Bitmap bmp = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                // Clear
                g.Clear(Color.LightGray);

                // Anti-alias
                g.SmoothingMode = SmoothingMode.AntiAlias;

                switch (cultureCode.ToLower())
                {
                    case "es-mx":
                        DrawMexico(g, width, height);
                        break;
                    case "en-us":
                        DrawUSA(g, width, height);
                        break;
                    case "pt-br":
                        DrawBrazil(g, width, height);
                        break;
                    default:
                        // Draw code text
                        using (Font f = new Font("Arial", 6))
                        {
                            // Draw centered text
                            SizeF textSize = g.MeasureString(cultureCode, f);
                            g.DrawString(cultureCode, f, Brushes.Black, (width - textSize.Width)/2, (height - textSize.Height)/2);
                        }
                        break;
                }

                // Border
                g.DrawRectangle(Pens.Black, 0, 0, width - 1, height - 1);
            }
            return bmp;
        }

        private static void DrawMexico(Graphics g, int w, int h)
        {
            float third = (float)w / 3;
            g.FillRectangle(Brushes.Green, 0, 0, third, h);
            g.FillRectangle(Brushes.White, third, 0, third, h);
            g.FillRectangle(Brushes.Red, third * 2, 0, w - (third * 2), h);

            // Eagle (simplified as a brown circle)
            float r = h * 0.25f;
            g.FillEllipse(Brushes.SaddleBrown, (w/2) - r, (h/2) - r, r*2, r*2);
        }

        private static void DrawUSA(Graphics g, int w, int h)
        {
            // Stripes
            float stripeHeight = (float)h / 13;
            for (int i = 0; i < 13; i++)
            {
                Brush b = (i % 2 == 0) ? Brushes.Red : Brushes.White;
                g.FillRectangle(b, 0, i * stripeHeight, w, stripeHeight + 1); // +1 to avoid gaps
            }

            // Canton
            g.FillRectangle(Brushes.Navy, 0, 0, w * 0.4f, h * 0.54f); // 7 stripes high roughly

            // Stars (simplified as white dots)
            g.FillRectangle(Brushes.White, w * 0.1f, h * 0.1f, 2, 2);
            g.FillRectangle(Brushes.White, w * 0.3f, h * 0.1f, 2, 2);
            g.FillRectangle(Brushes.White, w * 0.2f, h * 0.25f, 2, 2);
        }

        private static void DrawBrazil(Graphics g, int w, int h)
        {
            g.FillRectangle(Brushes.Green, 0, 0, w, h);

            // Yellow Rhombus
            Point[] rhombus = {
                new Point(w/2, 4),
                new Point(w-4, h/2),
                new Point(w/2, h-4),
                new Point(4, h/2)
            };
            g.FillPolygon(Brushes.Yellow, rhombus);

            // Blue Circle
            int r = h / 4;
            g.FillEllipse(Brushes.Navy, (w/2) - r, (h/2) - r, r*2, r*2);
        }
    }
}
