﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.IO;
using static System.Net.WebRequestMethods;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;
using static System.Net.Mime.MediaTypeNames;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Media3D;
using System.Reflection;
using awsedfrgt3e4.src;
using System.Linq.Expressions;
using MaterialDesignThemes.Wpf;
using System.Windows.Interop;
using System.Diagnostics;

namespace awsedfrgt3e4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("Kernel32")]
        public static extern void AllocConsole();

        [DllImport("Kernel32")]
        public static extern void FreeConsole();

        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, [In] ref bool attrValue, int attrSize);

        private const int DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1 = 19;
        private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;
        private const int DWMWA_CAPTION_COLOR = 35;

        private static bool UseImmersiveDarkMode(IntPtr handle, bool enabled)
        {
            if (IsWindows10OrGreater(17763))
            {
                var attribute = DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1;
                if (IsWindows10OrGreater(18985))
                {
                    attribute = DWMWA_USE_IMMERSIVE_DARK_MODE;
                }

                bool value = enabled;

                int useImmersiveDarkMode = enabled ? 1 : 0;
                if (DwmSetWindowAttribute(handle, (int)attribute, ref value, Marshal.SizeOf<bool>()) == 1) return true;
                else return false;
            }

            return false;
        }

        private static bool IsWindows10OrGreater(int build = -1)
        {
            return Environment.OSVersion.Version.Major >= 10 && Environment.OSVersion.Version.Build >= build;
        }

        public MainWindow()
        {
            InitializeComponent();
            IntPtr hWnd = new WindowInteropHelper(this).EnsureHandle();
            UseImmersiveDarkMode(hWnd, true);
            this.SetValue(TextOptions.TextFormattingModeProperty, TextFormattingMode.Ideal);
            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.HighQuality);
        }

        FileInfo[] Images;
        List<string> ImageList = new List<string>();
        int CurrentNumb = 0;
        BitmapImage resizedImage;
        BitmapImage realorigin;
        bool isMouseDown = false;
        string CurrentShowingImageName;
        public bool isExistRect = false;
        public System.Drawing.Point pointStart;
        public System.Drawing.Point pointEnd;
        public System.Drawing.Point ipointStart;
        public System.Drawing.Point ipointEnd;
        Bitmap newBitmap = null;
        System.Drawing.Image newSysImg = null;
        bool IsOpen = false;
        uint Versionf = 0;

        public bool ShowImage(int y)
        {
            if (ImageList != null)
            {
                realorigin = new BitmapImage(new Uri(ImageList[y]));
                CurrentShowingImageName = ImageList[y].ToString();
                resizedImage = ResizeImage(598, realorigin);
                CurrentShowingImage.Source = new BitmapImage(new Uri(ImageList[y]));
                CurrentNumb++;
                return true;
            }
            else
            {
                return false;
            }
        }

        public void startRect(int x, int y)
        {
            switch (Versionf) {
                case 0:
                    isExistRect = true;
                    pointStart.X = x;
                    pointStart.Y = y - 140;
                    pointEnd.X = x;
                    pointEnd.Y = y - 140;
                    //Console.WriteLine($"Startx: {pointStart.X} Starty: {pointStart.Y} Endx: {pointEnd.X} Endy: {pointEnd.Y}");
                    break;
                case 1:
                    isExistRect = true;
                    pointStart.X = x;
                    pointStart.Y = y - 140;
                    pointEnd.X = x;
                    pointEnd.Y = y - 140;
                    //Console.Write($"Startx: {pointStart.X} Starty: {pointStart.Y}");
                    break;
                case 2:
                    isExistRect = true;
                    pointStart.X = x;
                    pointStart.Y = y - 140;
                    pointEnd.X = x;
                    pointEnd.Y = y - 140;
                    //Console.WriteLine($"Startx: {pointStart.X} Starty: {pointStart.Y} Endx: {pointEnd.X} Endy: {pointEnd.Y}");
                    break;
            }
        }

        public void endRect(int x, int y)
        {
            switch (Versionf) {
                case 0:
                    pointEnd.X = x;
                    pointEnd.Y = y - 140;
                    //Console.Write($"Endx: {pointEnd.X} Endy: {pointEnd.Y} \n");
                    break;
                case 1:
                    pointEnd.X = x;
                    if (pointStart.X - pointEnd.X >= 0)
                    {
                        pointEnd.Y = pointStart.Y + pointStart.X - pointEnd.X;
                    }
                    else
                    {
                        pointEnd.Y = pointStart.Y + pointEnd.X - pointStart.X;
                    }
                    //Console.Write($"Endx: {pointEnd.X} Endy: {pointEnd.Y} \n");
                    break;
                case 2:
                    pointEnd.X = x;
                    pointEnd.Y = y - 140;
                    //Console.Write($"Endx: {pointEnd.X} Endy: {pointEnd.Y} \n");
                    break;
            }
        }

        public System.Drawing.Rectangle getRect(int ver)
        {
            if (ver == 1)
            {
                int x = pointStart.X > pointEnd.X ? pointEnd.X : pointStart.X;
                int y = pointStart.Y > pointEnd.Y ? pointEnd.Y : pointStart.Y;

                int w = Math.Abs(pointStart.X - pointEnd.X);
                int h = Math.Abs(pointStart.Y - pointEnd.Y);


                if (x <= 0) x = 0;
                if (y <= 0) y = 0;

                if (w <= 1) w = 1;
                if (h <= 1) h = 1;

                return new System.Drawing.Rectangle(x, y, w, h);
            }
            else if (ver == 2)
            {
                double golden_ratio = realorigin.PixelWidth / (double)585;

                int x = pointStart.X > pointEnd.X ? (int)Math.Round(pointEnd.X * golden_ratio) : (int)Math.Round(pointStart.X * golden_ratio);
                int y = pointStart.Y > pointEnd.Y ? (int)Math.Round(pointEnd.Y * golden_ratio) : (int)Math.Round(pointStart.Y * golden_ratio);

                int w = (int)Math.Round(Math.Abs((pointStart.X - pointEnd.X) * golden_ratio));
                int h = (int)Math.Round(Math.Abs((pointStart.Y - pointEnd.Y) * golden_ratio));


                if (x <= 0) x = 0;
                if (y <= 0) y = 0;

                if (w <= 1) w = 1;
                if (h <= 1) h = 1;

                return new System.Drawing.Rectangle(x, y, w, h);
            }
            else if (ver == 3)
            {
                double golden_ratio = realorigin.PixelWidth / (double)585;

                var rectanglaa = (UIElement)FindName("RectangularIm");
                var rectangla = (System.Windows.Shapes.Rectangle)FindName("RectangularIm");
                int x = (int)Math.Round(Canvas.GetTop(rectangla) * golden_ratio);
                int y = (int)Math.Round(Canvas.GetLeft(rectangla) * golden_ratio);
                int widthheight = (int)Math.Round(512 * golden_ratio);

                //Console.WriteLine($"{x} {y}");

                return new System.Drawing.Rectangle(y, x, widthheight, widthheight);
            }
            else
            {
                throw new ArgumentOutOfRangeException("Parameter index is out of range.");
            }
        }

        float Blend(float time, float startValue, float change, float duration)
        {
            time /= duration / 2;
            if (time < 1)
            {
                return change / 2 * time * time + startValue;
            }

            time--;
            return -change / 2 * (time * (time - 2) - 1) + startValue;
        }

        public void Update(int ver)
        {
            double x = 0;

            if (ver == 0)
            {
                while (x < 1)
                {
                    x = x + 0.01;
                    this.Dispatcher.Invoke(() =>
                    {
                        this.Height = 180 + Blend((float)x, 0, 820, 1f);
                        CurrentShowingImage.Opacity = x;
                        RectangularIm.Opacity = x;
                    });
                    Thread.Yield();
                }
            }
            else if (ver == 1)
            {
                while (x < 1)
                {
                    x = x + 0.01;
                    this.Dispatcher.Invoke(() =>
                    {
                        this.Height = 1000 - Blend((float)x, 0, 820, 1f);
                        CurrentShowingImage.Opacity = 1 - x;
                        RectangularIm.Opacity = 1 - x;
                    });
                    Thread.Yield();
                }
            }
            else if (ver == 2)
            {
                while (x < 1)
                {
                    x = x + 0.02;
                    this.Dispatcher.Invoke(() =>
                    {
                        CurrentShowingImage.Opacity = 1 - x;
                        RectangularIm.Opacity = 1 - x;
                    });
                    Thread.Sleep(1);
                }


                this.Dispatcher.Invoke(() =>
                {
                    ShowImage(CurrentNumb);
                });

                x = 0;

                Thread.Sleep(100);

                while (x < 1)
                {
                    x = x + 0.01;
                    this.Dispatcher.Invoke(() =>
                    {
                        CurrentShowingImage.Opacity = x;
                        RectangularIm.Opacity = x;
                    });
                    Thread.Sleep(1);
                }
            }
        }

        private void Find_Click(object sender, RoutedEventArgs e)
        {
            string PathLocation = PathLoc.Text;
            if (Directory.Exists(PathLocation)) {
                string[] files = Directory.GetFiles(PathLocation); // Get all files in the directory

                ImageList.Clear();
                CurrentNumb = 0;

                foreach (string file in files)
                {
                    if (System.IO.Path.GetExtension(file) == ".png" | System.IO.Path.GetExtension(file) == ".jpg")
                    {
                        ImageList.Add(file); // Add the file to the list
                        //AllocConsole();
                        //Console.WriteLine(file);
                    }
                }
            }
            else {
                MessageBox.Show("The Intended path was not found!");
                return;
            }

            if (ImageList.Count > 0) {
                if (IsOpen == false) ShowImage(CurrentNumb);
                InfoText.Text = "You're watching '" + System.IO.Path.GetFileNameWithoutExtension(CurrentShowingImageName) + "'";
            }
            else
            {
                MessageBox.Show("Nothing was in the folder!");
            }

            if (this.Height == 180)
            {
                Thread thread = new Thread(delegate ()
                {
                    Update(0);
                });

                thread.Start();

                IsOpen = true;
            }
            else
            {
                CurrentNumb = 0;
                Thread thread = new Thread(delegate ()
                {
                    Update(2);
                });

                thread.Start();
            }
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentNumb + 1 <= ImageList.Count)
            {
                Thread thread = new Thread(delegate ()
                {
                    Update(2);
                });

                thread.Start();

                InfoText.Text = "You're watching '" + System.IO.Path.GetFileNameWithoutExtension(CurrentShowingImageName) + "'";
            }
            else
            {
                MessageBox.Show("We reached the end of the road.");
            }
        }

        private void Prev_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentNumb - 2 >= 0)
            {
                CurrentNumb -= 2;
                Thread thread = new Thread(delegate ()
                {
                    Update(2);
                });

                thread.Start();

                InfoText.Text = "You're watching '" + System.IO.Path.GetFileNameWithoutExtension(CurrentShowingImageName) + "'";
            }
            else
            {
                MessageBox.Show("We reached the end of the road.");
            }
        }

        public static Bitmap BitmapImageToBitmap(BitmapImage bitmapImage)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder encoder = new BmpBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
                encoder.Save(outStream);
                Bitmap bitmap = new Bitmap(outStream);
                return new Bitmap(bitmap);
            }
        }

        public static BitmapImage BitmapToBitmapImage(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                return bitmapImage;
            }
        }

        public BitmapImage TransformedBitmapToBitmapImage(TransformedBitmap transformedBitmap)
        {
            BitmapImage bitmapImage = new BitmapImage();

            using (MemoryStream memoryStream = new MemoryStream())
            {
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(transformedBitmap));
                encoder.Save(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = memoryStream;
                bitmapImage.EndInit();
            }

            return bitmapImage;
        }

        public BitmapImage ResizeImage(int swidth, BitmapImage image_)
        {
            Bitmap newImage = BitmapImageToBitmap(image_);
            double goldenratio = (swidth / image_.Width); // set the desired width of the resized image
            var resizedImage = new TransformedBitmap(image_, new ScaleTransform(goldenratio, goldenratio));

            return TransformedBitmapToBitmapImage(resizedImage);
        }

        public Bitmap cropAtRect(Bitmap orgImg, System.Drawing.Rectangle sRect)
        {
            System.Drawing.Rectangle destRect = new System.Drawing.Rectangle(System.Drawing.Point.Empty, sRect.Size);

            var cropImage = new Bitmap(destRect.Width, destRect.Height);
            using (var graphics = Graphics.FromImage(cropImage))
            {
                graphics.DrawImage(orgImg, destRect, sRect, GraphicsUnit.Pixel);
            }
            return cropImage;
        }

        private void saveModifiedBitmap()
        {
            if (realorigin != null)
            {
                Bitmap cropBitmap;
                if (Versionf == 2)
                {
                    var rectanglaa = (UIElement)FindName("RectangularIm");
                    var rectangla = (System.Windows.Shapes.Rectangle)FindName("RectangularIm");
                    cropBitmap = cropAtRect(BitmapImageToBitmap(realorigin), getRect(3));

                }
                else
                {
                    if (getRect(1).Width < 5)
                    {
                        return;
                    }
                    cropBitmap = cropAtRect(BitmapImageToBitmap(realorigin), getRect(2));
                }
                string PathLocation = PathLoc.Text;
                var PPath = System.IO.Directory.GetParent(PathLocation);

                //Console.WriteLine("parent path: " + PPath);

                if (!Directory.Exists(PPath.ToString() + "\\CroppedImages"))
                {
                    Directory.CreateDirectory(PPath.ToString() + "\\CroppedImages");
                }

                try
                {
                    cropBitmap.Save(PPath + "\\CroppedImages\\" + System.IO.Path.GetFileNameWithoutExtension(CurrentShowingImageName) + ".png", ImageFormat.Png);
                    cropBitmap.Dispose();
                    cropBitmap = null;
                }
                catch (Exception e)
                {
                    InfoText.Text = e.Message;

                    return;
                }


                InfoText.Text = "Successfully Saved '" + System.IO.Path.GetFileNameWithoutExtension(CurrentShowingImageName) + "'";
            }
        }

        public static System.Drawing.Image ConvertBitmapImageToDrawingImage(BitmapImage bitmapImage)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                // Save the bitmap image to a memory stream.
                BitmapEncoder encoder = new BmpBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
                encoder.Save(stream);

                // Create a new Bitmap object and draw the bitmap onto it.
                using (Bitmap bitmap = new Bitmap(stream))
                {
                    System.Drawing.Image image = new Bitmap(bitmap.Width, bitmap.Height);
                    using (Graphics graphics = Graphics.FromImage(image))
                    {
                        graphics.DrawImage(bitmap, 0, 0);
                    }

                    return image;
                }
            }
        }

        public static BitmapImage ConvertImageToBitmapImage(System.Drawing.Image image)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                // Save the image to a memory stream in PNG format.
                image.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);

                // Create a new BitmapImage object and load the memory stream into it.
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = new MemoryStream(memoryStream.ToArray());
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                return bitmapImage;
            }
        }

        private void CurrentShowingImage_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isMouseDown)
            {
                return;
            }
            System.Windows.Point mousePos = e.GetPosition(this);
            if (isExistRect && Versionf != 2)
            {
                var rectanglaa = (UIElement)FindName("RectangularIm");
                var rectangla = (System.Windows.Shapes.Rectangle)FindName("RectangularIm");

                Canvas.SetLeft(rectanglaa, getRect(1).Left);
                Canvas.SetTop(rectanglaa, getRect(1).Top);
                rectangla.Width = getRect(1).Width;
                rectangla.Height = getRect(1).Height;
            }
            else if (isExistRect && Versionf == 2)
            {
                var rectanglaa = (UIElement)FindName("RectangularIm");
                var rectangla = (System.Windows.Shapes.Rectangle)FindName("RectangularIm");

                int x = pointEnd.X;
                int y = pointEnd.Y;

                if (x - 256 >= 0 && x + 256 <= 584) x -= 256;
                else if (x + 256 > 584) x = 72;
                else if (x - 256 < 0) x = 0;
                if (y - 256 >= 0 && y + 256 <= 788) y -= 256;
                else if (y + 256 > 788) y = 276;
                else if (y - 256 < 0) y = 0;
                Canvas.SetLeft(rectanglaa, x);
                Canvas.SetTop(rectanglaa, y);
                rectangla.Width = 512;
                rectangla.Height = 512;
            }

            endRect((int)mousePos.X, (int)mousePos.Y);
        }

        private void CurrentShowingImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            newBitmap = BitmapImageToBitmap(resizedImage);

            isMouseDown = true;
            System.Windows.Point mousePos = e.GetPosition(this);
            startRect((int)mousePos.X, (int)mousePos.Y);
        }

        private void CurrentShowingImage_MouseUp(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Point mousePos = e.GetPosition(this);

            if (isMouseDown)
            {
                endRect((int)mousePos.X, (int)mousePos.Y);
                MediaControl mEdia = new MediaControl();
                mEdia.PlaySC();

                isMouseDown = false;

                saveModifiedBitmap();
            }
        }

        private void CurrentShowingImage_MouseLeave(object sender, MouseEventArgs e)
        {
            System.Windows.Point mousePos = e.GetPosition(this);

            if (isMouseDown)
            {
                endRect((int)mousePos.X, (int)mousePos.Y);
                MediaControl mEdia = new MediaControl();
                mEdia.PlaySC();

                isMouseDown = false;

                saveModifiedBitmap();
            }
        }

        private void ToggleOpen_Click(object sender, RoutedEventArgs e)
        {
            if (IsOpen == false && this.Height == 180)
            {
                Thread thread = new Thread(delegate ()
                {
                    Update(0);
                });

                thread.Start();

                IsOpen = true;
            }
            else if(IsOpen == true)
            {
                Thread thread = new Thread(delegate ()
                {
                    Update(1);
                });

                thread.Start();

                IsOpen = false;
            }
        }

        private void Continue_Click(object sender, RoutedEventArgs e)
        {
            saveModifiedBitmap();

            if (CurrentNumb + 1 <= ImageList.Count)
            {
                Thread thread = new Thread(delegate ()
                {
                    Update(2);
                });

                thread.Start();

                MediaControl mEdia = new MediaControl();
                mEdia.PlayNext();

                InfoText.Text = "You're watching '" + System.IO.Path.GetFileNameWithoutExtension(CurrentShowingImageName) + "'";
            }
        }

        private void ToolBarAbout_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(new ProcessStartInfo
            {
                FileName = "https://github.com/Evestir/ICR",
                UseShellExecute = true
            });
        }

        private void ToolBarSquareRatio_Click(object sender, RoutedEventArgs e)
        {
            Versionf = 1;
        }

        private void ToolBarDragCut_Click(object sender, RoutedEventArgs e)
        {
            Versionf = 0;
        }

        private void ToolBarFiveOneTwo_Click(object sender, RoutedEventArgs e)
        {
            Versionf = 2;
        }
    }
}
