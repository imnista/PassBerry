namespace PassBerry.Library
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Text;

    internal class ImageHelper
    {
        public static string GetBase64StringFromImage(Image image)

        {
            var ms = new MemoryStream();
            image.Save(ms, ImageFormat.Jpeg);
            byte[] buffer = ms.ToArray();
            return Convert.ToBase64String(buffer);
        }

        public static Image GetImageFromBase64String(string imageText)
        {
            if (string.IsNullOrWhiteSpace(imageText))
            {
                return null;
            }
            else
            {
                var bitmapData = new Byte[imageText.Length];
                //bitmapData = Convert.FromBase64String(FixBase64ForImage(ImageText));
                bitmapData = Convert.FromBase64String(imageText);
                var streamBitmap = new MemoryStream(bitmapData);
                //Bitmap bitImage = new Bitmap((Bitmap)Image.FromStream(streamBitmap));
                return Image.FromStream(streamBitmap);
            }
        }

        private static string FixBase64ForImage(string imageText)
        {
            var text = new StringBuilder(imageText, imageText.Length);
            text.Replace("/r/n", String.Empty);
            text.Replace(" ", String.Empty);
            return text.ToString();
        }

        // 按比例缩放图片
        public Image ZoomPicture(Image SourceImage, int TargetWidth, int TargetHeight)
        {
            int IntWidth; //新的图片宽
            int IntHeight; //新的图片高

            var format = SourceImage.RawFormat;
            var SaveImage = new Bitmap(TargetWidth, TargetHeight);
            var g = Graphics.FromImage(SaveImage);
            g.Clear(Color.White);

            //计算缩放图片的大小 http://www.cnblogs.com/roucheng/

            if (SourceImage.Width > TargetWidth && SourceImage.Height <= TargetHeight)//宽度比目的图片宽度大，长度比目的图片长度小
            {
                IntWidth = TargetWidth;
                IntHeight = (IntWidth * SourceImage.Height) / SourceImage.Width;
            }
            else if (SourceImage.Width <= TargetWidth && SourceImage.Height > TargetHeight)//宽度比目的图片宽度小，长度比目的图片长度大
            {
                IntHeight = TargetHeight;
                IntWidth = (IntHeight * SourceImage.Width) / SourceImage.Height;
            }
            else if (SourceImage.Width <= TargetWidth && SourceImage.Height <= TargetHeight) //长宽比目的图片长宽都小
            {
                IntHeight = SourceImage.Width;
                IntWidth = SourceImage.Height;
            }
            else//长宽比目的图片的长宽都大
            {
                IntWidth = TargetWidth;
                IntHeight = (IntWidth * SourceImage.Height) / SourceImage.Width;
                if (IntHeight > TargetHeight)//重新计算
                {
                    IntHeight = TargetHeight;
                    IntWidth = (IntHeight * SourceImage.Width) / SourceImage.Height;
                }
            }

            g.DrawImage(SourceImage, (TargetWidth - IntWidth) / 2, (TargetHeight - IntHeight) / 2, IntWidth, IntHeight);
            SourceImage.Dispose();

            return SaveImage;
        }
    }
}