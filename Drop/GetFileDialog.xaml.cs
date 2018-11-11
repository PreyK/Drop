using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Drop
{
    public class GetFileDialog : Window
    {
        string dialogText;
        public string thatFile;
        public string thatIp;
        public GetFileDialog(string s)
        {
            dialogText = s;
            this.Width = 500;
            this.Height = 100;
            this.CanResize = false;
            
            this.InitializeComponent();

            this.FindControl<Button>("acceptBtn").Click += delegate
            {
                System.Diagnostics.Debug.WriteLine(thatIp + " " + thatFile);


               // Networking.NetworkManager.SendTcpPacket(thatIp, "sendfileaccept" + "_" + Networking.NetworkManager.myIp + "_" + thatFile);
                
                System.Diagnostics.Debug.WriteLine("Accept packet sent");
                //send accept tcp
                //close this
                Close();

            };
            this.FindControl<Button>("declineBtn").Click += delegate
            {
                //send decline tcp
              //  Networking.NetworkManager.SendTcpPacket(thatIp, "sendfiledecline_" + Networking.NetworkManager.myIp + "_");
                //clise this
                Close();
            };

#if DEBUG
            this.AttachDevTools();
#endif
            
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            var t = this.Get<TextBlock>("dialogText");
            t.FontSize = 20;
            t.Text = dialogText;
            t.TextAlignment = Avalonia.Media.TextAlignment.Center;
        }
    }
}
