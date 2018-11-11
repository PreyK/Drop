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

namespace Drop
{
    public class MainWindow : Window
    {

        public string MyIp;

        List<String> ips;
        List<DropUser> users;
        
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
            ips = new List<string>();

            this.FindControl<Button>("updateButton").Click += delegate
            {
                BtnClick();
            };


            Drop.Networking.NetworkManager.InitMe();
            NetworkManager.OnMessageEvent += new NetworkManager.OnMessageRecived(OnMessage);
            SendDiscovery();
        }


        public void BtnClick()
        {
             ClearUi();
             SendDiscovery();
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
                    DropUser u = new DropUser();
                    u.InitMe(ip);
                    this.Get<WrapPanel>("panel_images").Children.Add(u);
                    ips.Add(ip);
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
