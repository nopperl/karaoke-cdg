using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Threading;

using CDG;

namespace KaraokeCDG
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private CdgFile file;

        private BitmapSource source;

        private DispatcherTimer timer;

        private Task updateTask;

        private CancellationTokenSource cancellationToken;

        public MainWindow()
        {
            this.InitializeComponent();
            this.file = new CdgFile("ASK1402A-01 - Ellie Goulding - Burn.cdg");
            this.timer = new DispatcherTimer(
                TimeSpan.FromMilliseconds(1D),
                DispatcherPriority.Send,
                this.TimerElapsed,
                this.Dispatcher);
            this.cancellationToken = new CancellationTokenSource();
            this.updateTask = new Task(() => this.Update(this.cancellationToken.Token));
        }

        private void TimerElapsed(object sender, EventArgs e)
        {
            this.testImage.Source = this.source;
        }

        private void Update(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                BitmapSource nextBitmap = this.CreateBitmapSourceFromBitmap(this.file.Next());
                this.source = nextBitmap;
            }

            ////for (int second = 0; second < 5; second++)
            ////{
            ////    for (int milliSecond = 0; milliSecond < (300 / 20) * 1000; milliSecond++)
            ////    {

            ////    }
            ////}
        }

        private BitmapSource CreateBitmapSourceFromBitmap(Bitmap bitmap)
        {
            if (bitmap == null)
            {
                throw new ArgumentNullException(nameof(bitmap));
            }

            using (MemoryStream memoryStream = new MemoryStream())
            {
                try
                {
                    bitmap.Save(memoryStream, ImageFormat.Png);
                    memoryStream.Seek(0, SeekOrigin.Begin);

                    BitmapDecoder bitmapDecoder = BitmapDecoder.Create(
                        memoryStream,
                        BitmapCreateOptions.PreservePixelFormat,
                        BitmapCacheOption.OnLoad);

                    WriteableBitmap writable = new WriteableBitmap(bitmapDecoder.Frames.Single());
                    writable.Freeze();

                    return writable;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        private void LoadButtonClick(object sender, RoutedEventArgs e)
        {
            this.timer.Start();
            this.updateTask.Start();
        }

        private void StopButtonClick(object sender, RoutedEventArgs e)
        {
            this.timer.Stop();
            this.cancellationToken.Cancel();
        }

        private void SaveButtonClick(object sender, RoutedEventArgs e)
        {
            this.file.SaveAsVideo(@"video.wmv");
        }
    }
}
