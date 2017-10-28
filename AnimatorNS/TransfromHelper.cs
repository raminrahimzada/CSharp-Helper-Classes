namespace AnimatorNS
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Runtime.InteropServices;

    public static class TransfromHelper
    {
        private const int bytesPerPixel = 4;
        private static Random rnd = new Random();

        public static void CalcDifference(Bitmap bmp1, Bitmap bmp2)
        {
            PixelFormat format = PixelFormat.Format32bppArgb;
            Rectangle rect = new Rectangle(0, 0, bmp1.Width, bmp1.Height);
            BitmapData bitmapdata = bmp1.LockBits(rect, ImageLockMode.ReadWrite, format);
            IntPtr source = bitmapdata.Scan0;
            BitmapData data2 = bmp2.LockBits(rect, ImageLockMode.ReadOnly, format);
            IntPtr ptr2 = data2.Scan0;
            int length = (bmp1.Width * bmp1.Height) * 4;
            byte[] destination = new byte[length];
            byte[] buffer2 = new byte[length];
            Marshal.Copy(source, destination, 0, length);
            Marshal.Copy(ptr2, buffer2, 0, length);
            for (int i = 0; i < length; i += 4)
            {
                if (((destination[i] == buffer2[i]) && (destination[i + 1] == buffer2[i + 1])) && (destination[i + 2] == buffer2[i + 2]))
                {
                    destination[i] = 0xff;
                    destination[i + 1] = 0xff;
                    destination[i + 2] = 0xff;
                    destination[i + 3] = 0;
                }
            }
            Marshal.Copy(destination, 0, source, length);
            bmp1.UnlockBits(bitmapdata);
            bmp2.UnlockBits(data2);
        }

        public static void DoBlind(NonLinearTransfromNeededEventArg e, Animation animation)
        {
            if (animation.BlindCoeff != PointF.Empty)
            {
                byte[] pixels = e.Pixels;
                int width = e.ClientRectangle.Width;
                int height = e.ClientRectangle.Height;
                int stride = e.Stride;
                float x = animation.BlindCoeff.X;
                float y = animation.BlindCoeff.Y;
                int num6 = (int) (((width * x) + (height * y)) * (1f - e.CurrentTime));
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        int num9 = (j * stride) + (i * 4);
                        float num10 = ((i * x) + (j * y)) - num6;
                        if (num10 >= 0f)
                        {
                            pixels[num9 + 3] = 0;
                        }
                    }
                }
            }
        }

        public static void DoBlur(NonLinearTransfromNeededEventArg e, int r)
        {
            byte[] pixels = e.Pixels;
            byte[] sourcePixels = e.SourcePixels;
            int stride = e.Stride;
            int height = e.ClientRectangle.Height;
            int width = e.ClientRectangle.Width;
            int num4 = sourcePixels.Length - 4;
            for (int i = r; i < (width - r); i++)
            {
                for (int j = r; j < (height - r); j++)
                {
                    int index = (j * stride) + (i * 4);
                    int num8 = 0;
                    int num9 = 0;
                    int num10 = 0;
                    int num11 = 0;
                    int num12 = 0;
                    for (int k = i - r; k < (i + r); k++)
                    {
                        for (int m = j - r; m < (j + r); m++)
                        {
                            int num15 = (m * stride) + (k * 4);
                            if (((num15 >= 0) && (num15 < num4)) && (sourcePixels[num15 + 3] > 0))
                            {
                                num10 += sourcePixels[num15];
                                num9 += sourcePixels[num15 + 1];
                                num8 += sourcePixels[num15 + 2];
                                num11 += sourcePixels[num15 + 3];
                                num12++;
                            }
                        }
                    }
                    if ((index < num4) && (num12 > 5))
                    {
                        pixels[index] = (byte) (num10 / num12);
                        pixels[index + 1] = (byte) (num9 / num12);
                        pixels[index + 2] = (byte) (num8 / num12);
                        pixels[index + 3] = (byte) (num11 / num12);
                    }
                }
            }
        }

        public static void DoBottomMirror(NonLinearTransfromNeededEventArg e)
        {
            byte[] sourcePixels = e.SourcePixels;
            byte[] pixels = e.Pixels;
            int stride = e.Stride;
            int num2 = 1;
            int num3 = e.SourceClientRectangle.Bottom + num2;
            int height = e.ClientRectangle.Height;
            int left = e.SourceClientRectangle.Left;
            int right = e.SourceClientRectangle.Right;
            int num7 = height - num3;
            for (int i = left; i < right; i++)
            {
                for (int j = num3; j < height; j++)
                {
                    int num10 = ((num3 - 1) - num2) - (j - num3);
                    if (num10 < 0)
                    {
                        break;
                    }
                    int num11 = i;
                    int index = (num10 * stride) + (num11 * 4);
                    int num13 = (j * stride) + (i * 4);
                    pixels[num13] = sourcePixels[index];
                    pixels[num13 + 1] = sourcePixels[index + 1];
                    pixels[num13 + 2] = sourcePixels[index + 2];
                    pixels[num13 + 3] = (byte) ((1f - ((1f * (j - num3)) / ((float) num7))) * 90f);
                }
            }
        }

        public static void DoLeaf(NonLinearTransfromNeededEventArg e, Animation animation)
        {
            if (animation.LeafCoeff != 0f)
            {
                byte[] pixels = e.Pixels;
                int width = e.ClientRectangle.Width;
                int height = e.ClientRectangle.Height;
                int stride = e.Stride;
                int num4 = (int) ((width + height) * (1f - (e.CurrentTime * e.CurrentTime)));
                int length = pixels.Length;
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        int index = (j * stride) + (i * 4);
                        if ((i + j) >= num4)
                        {
                            int num9 = num4 - j;
                            int num10 = num4 - i;
                            int num11 = (num4 - i) - j;
                            if (num11 < -20)
                            {
                                num11 = -20;
                            }
                            int num12 = (num10 * stride) + (num9 * 4);
                            if ((((num9 >= 0) && (num10 >= 0)) && ((num12 >= 0) && (num12 < length))) && (pixels[index + 3] > 0))
                            {
                                pixels[num12] = (byte) Math.Min(0xff, (num11 + 250) + (pixels[index] / 10));
                                pixels[num12 + 1] = (byte) Math.Min(0xff, (num11 + 250) + (pixels[index + 1] / 10));
                                pixels[num12 + 2] = (byte) Math.Min(0xff, (num11 + 250) + (pixels[index + 2] / 10));
                                pixels[num12 + 3] = 230;
                            }
                            pixels[index + 3] = 0;
                        }
                    }
                }
            }
        }

        public static void DoMosaic(NonLinearTransfromNeededEventArg e, Animation animation, ref Point[] buffer, ref byte[] pixelsBuffer)
        {
            if ((animation.MosaicCoeff != PointF.Empty) && (animation.MosaicSize != 0))
            {
                int num9;
                byte[] pixels = e.Pixels;
                int width = e.ClientRectangle.Width;
                int height = e.ClientRectangle.Height;
                int stride = e.Stride;
                float currentTime = e.CurrentTime;
                int length = pixels.Length;
                float num6 = 1f - e.CurrentTime;
                if (num6 < 0f)
                {
                    num6 = 0f;
                }
                if (num6 > 1f)
                {
                    num6 = 1f;
                }
                float x = animation.MosaicCoeff.X;
                float y = animation.MosaicCoeff.Y;
                if (buffer == null)
                {
                    buffer = new Point[pixels.Length];
                    for (num9 = 0; num9 < pixels.Length; num9++)
                    {
                        buffer[num9] = new Point((int) (x * (rnd.NextDouble() - 0.5)), (int) (y * (rnd.NextDouble() - 0.5)));
                    }
                }
                if (pixelsBuffer == null)
                {
                    pixelsBuffer = (byte[]) pixels.Clone();
                }
                num9 = 0;
                while (num9 < length)
                {
                    pixels[num9] = 0xff;
                    pixels[num9 + 1] = 0xff;
                    pixels[num9 + 2] = 0xff;
                    pixels[num9 + 3] = 0;
                    num9 += 4;
                }
                int mosaicSize = animation.MosaicSize;
                float num11 = animation.MosaicShift.X;
                float num12 = animation.MosaicShift.Y;
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        int num15 = i / mosaicSize;
                        int num16 = j / mosaicSize;
                        num9 = (i * stride) + (j * 4);
                        int index = (num15 * stride) + (num16 * 4);
                        int num18 = j + ((int) (currentTime * (buffer[index].X + (num16 * num11))));
                        int num19 = i + ((int) (currentTime * (buffer[index].Y + (num15 * num12))));
                        if (((num18 >= 0) && (num18 < width)) && ((num19 >= 0) && (num19 < height)))
                        {
                            int num20 = (num19 * stride) + (num18 * 4);
                            pixels[num20] = pixelsBuffer[num9];
                            pixels[num20 + 1] = pixelsBuffer[num9 + 1];
                            pixels[num20 + 2] = pixelsBuffer[num9 + 2];
                            pixels[num20 + 3] = (byte) (pixelsBuffer[num9 + 3] * num6);
                        }
                    }
                }
            }
        }

        public static void DoRotate(TransfromNeededEventArg e, Animation animation)
        {
            Rectangle clientRectangle = e.ClientRectangle;
            PointF tf = new PointF((float) (clientRectangle.Width / 2), (float) (clientRectangle.Height / 2));
            e.Matrix.Translate(tf.X, tf.Y);
            if (e.CurrentTime > animation.RotateLimit)
            {
                e.Matrix.Rotate((360f * (e.CurrentTime - animation.RotateLimit)) * animation.RotateCoeff);
            }
            e.Matrix.Translate(-tf.X, -tf.Y);
        }

        public static void DoScale(TransfromNeededEventArg e, Animation animation)
        {
            Rectangle clientRectangle = e.ClientRectangle;
            PointF tf = new PointF((float) (clientRectangle.Width / 2), (float) (clientRectangle.Height / 2));
            e.Matrix.Translate(tf.X, tf.Y);
            float num = 1f - (animation.ScaleCoeff.X * e.CurrentTime);
            float num2 = 1f - (animation.ScaleCoeff.X * e.CurrentTime);
            if (Math.Abs(num) <= 0.001f)
            {
                num = 0.001f;
            }
            if (Math.Abs(num2) <= 0.001f)
            {
                num2 = 0.001f;
            }
            e.Matrix.Scale(num, num2);
            e.Matrix.Translate(-tf.X, -tf.Y);
        }

        public static void DoSlide(TransfromNeededEventArg e, Animation animation)
        {
            float currentTime = e.CurrentTime;
            e.Matrix.Translate((-e.ClientRectangle.Width * currentTime) * animation.SlideCoeff.X, (-e.ClientRectangle.Height * currentTime) * animation.SlideCoeff.Y);
        }

        public static void DoTransparent(NonLinearTransfromNeededEventArg e, Animation animation)
        {
            if (animation.TransparencyCoeff != 0f)
            {
                float num = 1f - (animation.TransparencyCoeff * e.CurrentTime);
                if (num < 0f)
                {
                    num = 0f;
                }
                if (num > 1f)
                {
                    num = 1f;
                }
                byte[] pixels = e.Pixels;
                for (int i = 0; i < pixels.Length; i += 4)
                {
                    pixels[i + 3] = (byte) (pixels[i + 3] * num);
                }
            }
        }
    }
}

