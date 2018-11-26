using System;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;

namespace Drop
{
    public class DropUser : UserControl
    {
        public string ip;
        public string name;
        public DropUser()
        {
            this.InitializeComponent();

            AddHandler(DragDrop.DropEvent, Drop);
            AddHandler(DragDrop.DragOverEvent, DragOver);
        }

        private void DragOver(object sender, DragEventArgs e)
        {
        }

        private void Drop(object sender, DragEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("drop"+ string.Join(Environment.NewLine, e.Data.GetFileNames()));
            SendFileToUser(string.Join(Environment.NewLine, e.Data.GetFileNames()));
        }

        private void SendFileToUser(string file)
        {
            Networking.NetworkManager.SendFileTo(file, ip);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public void InitMe(string userName)
        {
            SetUserImage();
            SetUserIp(userName);
        }
        public void SetUserImage()
        {

            var u = this.Get<Image>("UserImage");
            u.Width = 60;
            u.Height = 60;
            u.Margin = new Avalonia.Thickness(0, 0, 0, 0);

            Bitmap b = new Bitmap("Assets\\profile_ca.jpg");

            u.Source = b;
        }
        public void SetUserIp(string s)
        {
            ip = s;
            var usert = this.Get<TextBlock>("UserName");
            usert.TextAlignment = Avalonia.Media.TextAlignment.Center;
            usert.Text = s;
            usert.Margin = new Avalonia.Thickness(0, 0, 0, 0);
        }
    }
}
