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
        List<String> ips;
        WrapPanel imagePanel;
        public MainWindow()
        {
            InitializeComponent();
           
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            
            AvaloniaXamlLoader.Load(this);
            this.CanResize = false;
            imagePanel = this.Get<WrapPanel>("panel_images");
            imagePanel.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center;
            imagePanel.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;
            ips = new List<string>();
            this.FindControl<Button>("updateButton").Click += delegate
            {
                BtnClick();
            };
            Drop.Networking.NetworkManager.InitMe();
            NetworkManager.OnMessageEvent += new NetworkManager.OnMessageRecived(OnMessage);
            SendDiscovery();
        }
        int c=0;
        public void BtnClick()
        {
           // c++;
             ClearUi();
             SendDiscovery();
            //AddUserToUi("127.0.0."+c.ToString());
            //DrawCircles(6);
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
            string name = s.Split('_')[2];
            switch (msg)
            {
                case "ruthere":
                    //reply
                    ReplyToDiscovery();
                    break;

                case "iamthere":
                    //add ui
                    AddUserToUi(ip, name);
                    break;
            }
        }
        
       async void AddUserToUi(string ip, string name)
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
                    
                    imagePanel.Children.Add(u);
                    u.InitMe(ip, name);
                    ips.Add(ip);
                }
            });

        }
      
        #region udpPackets

        void ReplyToDiscovery()
        {
            NetworkManager.SendUdpPacket("iamthere_" +NetworkManager.myIp+"_"+NetworkManager.userName);
        }
        void SendDiscovery()
        {
            NetworkManager.SendUdpPacket("ruthere_"+NetworkManager.myIp+"_"+NetworkManager.userName);
        }

        #endregion

    }
}
