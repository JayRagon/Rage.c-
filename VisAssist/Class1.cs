using System;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Diagnostics;
using System.Linq;
using System.Drawing.Imaging;
using System.IO;
using Windows.UI.Input.Preview.Injection;
using System.Collections.Generic;

namespace VisAssist
{
    internal class Program
    {
        #region Initializer
        static Color targetColor = ColorTranslator.FromHtml("#FFFFFF");//FA00FA FA0002  THIS IS THE COLOR THAT IT WILL BE SEARCHING FOR, I DON'T HAVE THE HEXCODE FOR ANY VALORANT OUTLINES, SCREENSHOT IT YOURSELF AND SEE

        //cfg
        static int FoV = 3;
        static int res = 2;
        static byte silentAim = 0;

        //globals
        static double xMove;
        static double yMove;
        static double sens;

        static int startCoords;
        static int finishCoords;

        static int pixelsFound;

        static int sHeight = Screen.PrimaryScreen.Bounds.Height;
        static int sWidth = Screen.PrimaryScreen.Bounds.Width;

        static double xAvg = 0;
        static double yAvg = 0;

        private const int DCX_WINDOW = 0x01;
        private const int DCX_CACHE = 0x02;
        private const int DCX_LOCKWINDOWUPDATE = 0x0400; 

        //imports (immigrants)
        [DllImport("user32.dll")]
        private static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll")]
        private static extern IntPtr GetDCEx(IntPtr hwnd, IntPtr hrgn, uint flags);

        [DllImport("user32.dll")]
        private static extern void mouse_event(uint dwFlags, int dx, int dy, uint dwData, uint dwExtraInf);

        [DllImport("user32.dll")]
        static extern short GetAsyncKeyState(Keys vKey);

        static void Main(string[] args) // settings (might make ui for it once im done)
        {
            Console.WriteLine("monitor width: " + sWidth);
            Console.WriteLine("monitor height: " + sHeight);

            Console.Write("\nwhat is the speed of the colorbot (the higher the less resolution, minimum 1)\n> ");
            res = int.Parse(Console.ReadLine());
            Console.Write("and what is the fov (in pixels)\n> ");
            FoV = int.Parse(Console.ReadLine());
            Console.Write("do you want silent aim? (1 for on 0 for off)\n> ");
            silentAim = byte.Parse(Console.ReadLine());
            Console.Write("what is your sens\n> ");
            sens = double.Parse(Console.ReadLine());

            startCoords = sHeight / FoV;
            finishCoords = sWidth - (sWidth / FoV) - startCoords;

            PixelSearch();
        }
        #endregion

        static void PixelSearch()
        {
            Rectangle searchRect = new Rectangle((sWidth - FoV) / 2, (sHeight - FoV) / 2, FoV, FoV); // init the search rectangle

            Bitmap bitmap = new Bitmap(searchRect.Width, searchRect.Height);
            Graphics graphics = Graphics.FromImage(bitmap);

            Console.WriteLine("\nExecuted!");

            for (; ; )
            {
                //Stopwatch stopWatch = new Stopwatch(); // for debugging

                //stopWatch.Start();

                xAvg = 0;
                yAvg = 0;

                pixelsFound = 0;

                //screengrab
                //graphics.CopyFromScreen(0, 0, 0, 0, bitmap.Size);
                graphics.CopyFromScreen(searchRect.X, searchRect.Y, 0, 0, searchRect.Size, CopyPixelOperation.SourceCopy); // screenshot and savee to bitmap through graphics obj


                //pixelsearch, finds pixels that are equal to the target color and if so, add it to xAvg and yAvg, which will be divided later on line ~129
                for (int y = 0; y < bitmap.Height; y+=res)
                {
                    for (int x = 0; x < bitmap.Width; x+=res)
                    {
                        if (bitmap.GetPixel(x, y) == targetColor)
                        {
                            xAvg += x + searchRect.X;
                            yAvg += y + searchRect.Y;

                            pixelsFound++;
                        }
                    }
                }

                if (GetAsyncKeyState(Keys.K) < 0) // runs the movee and click method to test if it is detected or not by anti cheat NOT THE AIMBOT KEY
                {
                    Click();
                    Move(100, 100);
                    Thread.Sleep(100);
                }

                if (pixelsFound != 0) // if more than one pixel has been detected
                {
                    //avg out all results here
                    xAvg /= pixelsFound;
                    yAvg /= pixelsFound;

                    //aim :3

                    if (GetAsyncKeyState(Keys.T) < 0) // if key t is down AIMBOT KEY HERE
                    {
                        if (silentAim == 0)
                        {
                            xMove = xAvg - (sWidth / 2);
                            yMove = yAvg - (sHeight / 2);

                            xMove /= sens;
                            yMove /= sens;

                            xMove *= 1.07; //fov scaling
                            yMove *= 1.07; //fov scaling

                            Move((int)xMove, (int)yMove);
                            Click();
                            Thread.Sleep(1); //need to wait for click because idk its delayed
                        }
                        else
                        {
                            //silentAim here (i think this is more detectable but who cares)
                            xMove = xAvg - (sWidth / 2);
                            yMove = yAvg - (sHeight / 2);

                            xMove /= sens;
                            yMove /= sens;

                            xMove *= 1.07; //fov scaling
                            yMove *= 1.07; //fov scaling

                            Move((int)xMove, (int)yMove);
                            Click();
                            Thread.Sleep(1); //need to wait for click because idk its delayed
                            Move((int)-xMove, (int)-yMove); // move bacc to the origional position
                        }
                    }
                }

                //stopWatch.Stop();

                //if (stopWatch.ElapsedTicks != 0) //FOR SEEING HOW LONG THE METHD TAKES, ONLY USE IF OPTIMISING
                //{
                //    Int64 elapsedTime = stopWatch.ElapsedTicks;
                //    Console.WriteLine((float)elapsedTime / 10000);
                //}
            }
        }

        // BELOW THIS IS ALL THAT NEEDS CHANGING FOR THE AIMBOT TO WORK! WE ARE ALMOST THERE BOIS
        // by the way the move method needs to move by x and y in pixels, click needs to be LEFTDOWN then LEFTUP, that's it. I have measures in place to wait for click and move methods so it should be fine
        static void Move(int x, int y)
        {
            // change this to ud
            mouse_event(0x01, x, y, 0, 0); 
        }

        static void Click()
        {
            // change this to ud
            mouse_event(0x02, 0, 0, 0, 0);
            mouse_event(0x04, 0, 0, 0, 0);
        }
    }
}

