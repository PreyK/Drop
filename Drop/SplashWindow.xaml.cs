using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;

namespace Drop
{
    public class SplashWindow : Window
    {

        TextBox userName;
        Image userPic;
        Button nextBtn;
        public SplashWindow()
        {
            this.InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif

            AddHandler(DragDrop.DropEvent, OnPicDrop);
        }

        private void OnPicDrop(object sender, DragEventArgs e)
        {
            string file = string.Join(Environment.NewLine, e.Data.GetFileNames());
            MakeProfilePic(file);
        }

        void MakeProfilePic(string file)
        {
            System.Drawing.Bitmap f = new System.Drawing.Bitmap(file);

            System.Drawing.Image i = Utils.GraphicsExtensions.MakeSquarePhoto(f, 512);
            System.Drawing.Image dst = Utils.GraphicsExtensions.CropToCircle(i, System.Drawing.Color.Transparent);

            dst.Save("Assets\\profile_ca.jpg");
            Bitmap aw = new Bitmap("Assets\\profile_ca.jpg");

            userPic.Source = aw;
        }
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            userName = this.Get<TextBox>("usern");
            userName.TextAlignment = Avalonia.Media.TextAlignment.Center;
            userName.FontSize = 40;
            userPic = this.Get<Image>("pic");
            nextBtn = this.Get<Button>("done");
            nextBtn.Click += NextBtn_Click;
            ShowPic();
        }
        public void ShowPic()
        {
           
            userPic.Width = 150;
            userPic.Height = 160;
            userPic.Margin = new Avalonia.Thickness(0, 0, 0, 0);

            Bitmap b = new Bitmap("Assets\\profile_ca.jpg");

            userPic.Source = b;
        }

        private void NextBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("aaa");
            new MainWindow().Show();
            this.Close();
        }
    }
}
