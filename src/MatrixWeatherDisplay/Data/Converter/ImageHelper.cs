namespace MatrixWeatherDisplay.Data.Converter;
internal static class ImageHelper {
    public static void ScaleDown<TBigPixel, TSmallPixel>(Image<TBigPixel> big,  Image<TSmallPixel> small) where TBigPixel : unmanaged, IPixel<TBigPixel> where TSmallPixel : unmanaged, IPixel<TSmallPixel> {
        int stepsWidth = big.Width / small.Width;
        int stepsHeight = big.Height / small.Height;


        for (int y = 0; y < small.Height; y++) {
            for (int x = 0; x < small.Width; x++) {
                Color avg = Avg(big, x * stepsWidth, y * stepsHeight, stepsWidth, stepsHeight);

                small[x, y] = avg.ToPixel<TSmallPixel>();
            }
        }
    }

    private static Color Avg<TPixel>(Image<TPixel> img, int x, int y, int width, int height) where TPixel: unmanaged, IPixel<TPixel> {
        long rSum = 0,
             gSum = 0,
             bSum = 0;

        var buffer = new Rgba32();
        for (int dy = 0; dy < height; dy++) {
            for (int dx = 0; dx < width; dx++) {
                TPixel pixel = img[x + dx, y + dy];
                pixel.ToRgba32(ref buffer);

                rSum += buffer.R;
                gSum += buffer.G;
                bSum += buffer.B;
            }
        }

        long pixelCount = width * height;
        byte r = (byte)(rSum / pixelCount);
        byte g = (byte)(gSum / pixelCount);
        byte b = (byte)(bSum / pixelCount);
        return Color.FromRgb(r, g, b);
    }

    public static void AdjustFrameLengths<T>(Image<T> image) where T : unmanaged, IPixel<T> {
        if(image.Frames.Count == 0) {
            return;
        }

        foreach (ImageFrame<T> frame in image.Frames) {
            SixLabors.ImageSharp.Formats.Gif.GifFrameMetadata metadata = frame.Metadata.GetGifMetadata();
            if(metadata.FrameDelay < 2)
                metadata.FrameDelay = 2;
        }
    }

    public static void SetColor<T>(Image<T> image, Color color) where T : unmanaged, IPixel<T> {
        if(image.Frames.Count > 0) {
            foreach(ImageFrame<T> frame in image.Frames) {
                SetColor(frame, color);
            }
        }
    }

    private static void SetColor<T>(ImageFrame<T> frame, Color color) where T : unmanaged, IPixel<T> {
        for (int y = 0; y < frame.Height; y++) {
            for (int x = 0; x < frame.Width; x++) {
                var pixel = new Rgba32();
                frame[x, y].ToRgba32(ref pixel);
                int sum = (pixel.R + pixel.G + pixel.B) * pixel.A;
                if (sum > 0) {
                    frame[x, y] = color.ToPixel<T>();
                }
            }
        }
    }
}
