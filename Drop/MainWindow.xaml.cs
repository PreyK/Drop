using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using Drop.Networking;
using Avalonia.Threading;
using Drop.Utils;
using Avalonia.Media;
using Avalonia.Controls.Shapes;
using Avalonia.Platform;

namespace Drop
{
    public class MainWindow : Window
    {

        public string MyIp;

        List<String> ips;
        List<DropUser> users;
        private Image UserImg;
        // for transistions stuff
        private int cycles;
        private float fcycle;
        public List<string> userIps=new List<string>();

        public MainWindow()
        {
            InitializeComponent();
           
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {


            this.CanResize = false;
            AvaloniaXamlLoader.Load(this);

            UserImg = this.Get<Image>("MeUser");
            Utils.TickManager.InitMe();
            Utils.TickManager.OnTickEvent += new Utils.TickManager.OnTickDelegate(Update);

            SetMyPic();

            ips = new List<string>();

            this.FindControl<Button>("updateButton").Click += delegate
            {
                BtnClick();
            };


            Drop.Networking.NetworkManager.InitMe();
            NetworkManager.OnMessageEvent += new NetworkManager.OnMessageRecived(OnMessage);
            SendDiscovery();


           // this.Get<Avalonia.Controls.Shapes.Ellipse>("asdwek").SetValue(Canvas.TopProperty, 10f);
      

        }



        void SetMyPic()
        {
            System.Drawing.Bitmap f = new System.Drawing.Bitmap("Assets\\profile.jpg");

            System.Drawing.Image i = Utils.GraphicsExtensions.MakeSquarePhoto(f, 512);
            System.Drawing.Image dst = Utils.GraphicsExtensions.CropToCircle(i, System.Drawing.Color.Transparent);

            dst.Save("Assets\\profile_ca.jpg");


            Bitmap aw = new Bitmap("Assets\\profile_ca.jpg");

            var u = this.Get<Image>("MeUser");
            u.Width = 0;
            u.Height = 0;
            u.Margin = new Avalonia.Thickness(20, 20, 20, 0);
            u.Source = aw;
            u.ZIndex = 10;

            Bitmap ak = new Bitmap("Assets\\e.png");

            var ub = this.Get<Image>("MeBg");

           //n this.Get<RotateTransform>("asd").Angle = 180;
            //rotator = this.Get<RotateTransform>("asd");
            ub.ZIndex = -10;
            ub.Width = 130;
            ub.Height = 130;
            ub.Margin = new Avalonia.Thickness(20, 20, 20, 0);
            ub.Source = ak;

        }
       
     

       private async void Update()
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                cycles++;
                fcycle = cycles;
                if(UserImg.Width<120)
                UserImg.Width = Mathf.BounceEaseOut(fcycle, 0, 120, 1.5f/0.016f);
            });

        }
        int c=0;
        public void BtnClick()
        {
            c++;
           //  ClearUi();
          //   SendDiscovery();
            AddUserToUi("127.0.0."+c.ToString());
            //DrawCircles(6);
        }


        private void DrawCircles(int numCircles)
        {
            var canvas = this.Get<Canvas>("mainCanvas");
           
            float angle = (360 / numCircles);

            var Hyp = 130;

            for (int i = 0; i <= numCircles; i++)
            {
                var currentAngle = i * angle;
                var circlePosXOpp = Math.Sin(currentAngle.ToDegrees()) * Hyp;
                var circlePosYAdj = Math.Cos(currentAngle.ToDegrees()) * Hyp;

                Ellipse e = new Ellipse
                {
                    Width = 80,
                    Height = 80,
                    Fill = new SolidColorBrush(Colors.Red)
                };
                DropUser u = new DropUser();
                u.InitMe("dummy");
                u.Width = 80;
                u.Height = 80;
                canvas.Children.Add(u);
                u.SetValue(Canvas.TopProperty, circlePosYAdj+170);
                u.SetValue(Canvas.LeftProperty, circlePosXOpp+160);
            }
        }

        private void ClearUi()
        {
            ips.Clear();
            this.Get<WrapPanel>("panel_images").Children.Clear();
        }

        public void OnMessage(string s)
        {
            string msg = s.Split('_')[0];
            string ip = s.Split('_')[1];
            switch (msg)
            {
                case "ruthere":
                    //reply
                    ReplyToDiscovery();
                    break;

                case "iamthere":
                    //add ui
                     AddUserToUi(ip);
                    break;
            }
        }



       async void AskReciveFile(string fn, string ip)
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                var d = new GetFileDialog(fn + " from " + ip);
                d.thatFile = fn;
                d.thatIp = ip;
                d.Show();
            });
        }



       async void AddUserToUi(string ip)
       {
            if (ip == NetworkManager.myIp)
            {
                return;
            }
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                if (!ips.Contains(ip))
                {
                   // DropUser u = new DropUser();
                    //u.InitMe(ip);
                    //this.Get<WrapPanel>("panel_images").Children.Add(u);
                    ips.Add(ip);
                    var canvas = this.Get<Canvas>("mainCanvas");
                    canvas.Children.Clear();
                    DrawCircles(ips.Count);
                    
                }
            });

        }
      
        #region udpPackets

        void ReplyToDiscovery()
        {
            NetworkManager.SendUdpPacket("iamthere_" +NetworkManager.myIp+"_");
        }
        void SendDiscovery()
        {
            NetworkManager.SendUdpPacket("ruthere_"+NetworkManager.myIp+"_");
        }

        #endregion

    }
}
