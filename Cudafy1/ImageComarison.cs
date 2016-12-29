using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cudafy1
{
    class ImageComarison
    {
        static readonly ColorMatrix ColorMatrix = new ColorMatrix(new float[][] 
        {
            new float[] {.3f, .3f, .3f, 0, 0},
            new float[] {.59f, .59f, .59f, 0, 0},
            new float[] {.11f, .11f, .11f, 0, 0},
            new float[] {0, 0, 0, 1, 0},
            new float[] {0, 0, 0, 0, 1}
        });
        public static Image GetGrayScaleVersion(Image original)
        {
            //http://www.switchonthecode.com/tutorials/csharp-tutorial-convert-a-color-image-to-grayscale
            //create a blank bitmap the same size as original
            Bitmap newBitmap = new Bitmap(original.Width, original.Height);

            //get a graphics object from the new image
            using (Graphics g = Graphics.FromImage(newBitmap))
            {
                //create some image attributes
                ImageAttributes attributes = new ImageAttributes();

                //set the color matrix attribute
                attributes.SetColorMatrix(ColorMatrix);

                //draw the original image on the new image
                //using the grayscale color matrix
                g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height),
                   0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);
            }
            return newBitmap;

        }
        public static Image Resize(Image originalImage, int newWidth, int newHeight)
        {
            Image smallVersion = new Bitmap(newWidth, newHeight);
            using (Graphics g = Graphics.FromImage(smallVersion))
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.DrawImage(originalImage, 0, 0, newWidth, newHeight);
            }

            return smallVersion;
        }
        private static bool CheckIfFileExists(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("File '" + filePath + "' not found!");
            }
            return true;
        }
        public static float GetPercentageDifference(string image1Path, string image2Path, byte threshold = 1)
        {
            if (CheckIfFileExists(image1Path) && CheckIfFileExists(image2Path))
            {
                Image img1 = Image.FromFile(image1Path);
                Image img2 = Image.FromFile(image2Path);

                float difference = PercentageDifference(img1,img2, threshold);

                img1.Dispose();
                img2.Dispose();

                return difference;
            }
            else return -1;
        }
        public static float PercentageDifference(Image img1, Image img2, byte threshold = 3)
        {
            byte[,] differences = GetDifferences(img1,img2);

            int diffPixels = 0;

            foreach (byte b in differences)
            {
                if (b >= threshold) { diffPixels++; }
            }

            return diffPixels / 48000f;
        }
        public static byte[,] GetGrayScaleValues(Image img)
        {
            using (Bitmap thisOne = (Bitmap)GetGrayScaleVersion(Resize(img, 800, 600)))
            {
                byte[,] grayScale = new byte[800, 600];

                for (int x = 0; x < 800; x++)
                {
                    for (int y = 0; y < 600; y++)
                    {
                        grayScale[x, y] = (byte)Math.Abs(thisOne.GetPixel(x, y).R);
                    }
                }
                return grayScale;
            }
        }
        public static byte[,] GetDifferences(Image img1, Image img2)
        {
            Image thisOne = GetGrayScaleVersion(Resize(img1, 800, 600));
            Image theOtherOne = GetGrayScaleVersion(Resize(img2, 800, 600));

            byte[,] differences = new byte[800, 600];
            byte[,] firstGray = GetGrayScaleValues(thisOne);
            byte[,] secondGray = GetGrayScaleValues(theOtherOne);

            for (int x = 0; x < 800; x++)
            {
                for (int y = 0; y < 600; y++)
                {
                    differences[x, y] = (byte)Math.Abs(firstGray[x, y] - secondGray[x, y]);
                }
            }
            thisOne.Dispose();
            theOtherOne.Dispose();
            return differences;
        }
    }
}
