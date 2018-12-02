using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Drop.Networking
{
    public static class NetworkManager
    {
        delegate void AddMessage(string message);

        public delegate void OnMessageRecived(string m);
        public static event OnMessageRecived OnMessageEvent;

        //unimplemented
        public static string userName;

        //dá ports
        const int udpPort = 54545;
        const int tcpFilesPort = 500;
        const int tcpPacketsPort = 501;

        //lö adress
        const string broadcastAddress = "255.255.255.255";

        
        
        static TcpListener tcpFileListener;

        static TcpListener tcpPacketListener;



        static UdpClient receivingClient;
        static UdpClient sendingClient;

        static TcpClient tcpPacketSender;
        static TcpClient tcpPackerTeciver;


        //reciving stuff on separete threads
        static Thread udpReciving;
        static Thread tcpReciving;
        static Thread tcpPacketRec;


        //this computer's local ip adress
        public static string myIp;


        public static void InitMe()
        {
            myIp = GetLocalIPAddress();
            InitializeSender();
            InitializeReceiver();
        }

        public static void InitializeSender()
        {
            sendingClient = new UdpClient(broadcastAddress, udpPort);


            sendingClient.EnableBroadcast = true;
        }

        public static void InitializeReceiver()
        {
            receivingClient = new UdpClient(udpPort);

            ThreadStart start = new ThreadStart(UdpReciver);
            udpReciving = new Thread(start);
            udpReciving.IsBackground = true;
            udpReciving.Start();

            ThreadStart tcprec = new ThreadStart(TcpFileReciverAsync);
            tcpReciving = new Thread(tcprec);
            tcpReciving.IsBackground = true;
            tcpReciving.Start();

            ThreadStart tcpFileRec = new ThreadStart(TcpPacketReciver);
            tcpPacketRec = new Thread(tcpFileRec);
            tcpPacketRec.IsBackground = true;
            tcpPacketRec.Start();

        }

         static void TcpPacketReciver()
        {
            tcpPacketListener = new TcpListener(IPAddress.Any, tcpPacketsPort);
            tcpPacketListener.Start();



            Socket socket = tcpPacketListener.AcceptSocket();

           var buff = new byte[1024];

            socket.Receive(buff);

            var buffs = Encoding.ASCII.GetString(buff);

            System.Diagnostics.Debug.WriteLine(buffs);
            UdpMessageRecived(buffs);
        }


        //DON'T JUST DON'T....
        static async void TcpFileReciverAsync()
        {
            tcpFileListener = new TcpListener(IPAddress.Any, tcpFilesPort);
            tcpFileListener.Start();

            Socket socket = tcpFileListener.AcceptSocket();

            int bufferSize = 1024;
            byte[] buffer = null;
            byte[] header = null;
            string headerStr = "";
            string filename = "";
            int filesize = 0;


            header = new byte[bufferSize];

            socket.Receive(header);

            headerStr = Encoding.ASCII.GetString(header);


            string[] splitted = headerStr.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            Dictionary<string, string> headers = new Dictionary<string, string>();
            foreach (string s in splitted)
            {
                if (s.Contains(":"))
                {
                    headers.Add(s.Substring(0, s.IndexOf(":")), s.Substring(s.IndexOf(":") + 1));
                }

            }
            //Get filesize from header
            filesize = Convert.ToInt32(headers["Content-length"]);

            //Get filename from header
            //string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            //filename = desktop+"/DropRecived/"+ headers["Filename"];



            //TODO: KILL THIS WITH FIRE
            string x = await SaveFileAsync(headers["Filename"]);
            if (String.IsNullOrWhiteSpace(x))
            {
                tcpFileListener.Stop();

                ThreadStart tcprec2 = new ThreadStart(TcpFileReciverAsync);
                tcpReciving = new Thread(tcprec2);
                tcpReciving.IsBackground = true;
                tcpReciving.Start();
                return;
            }


            int bufferCount = Convert.ToInt32(Math.Ceiling((double)filesize / (double)bufferSize));


            FileStream fs = new FileStream(x, FileMode.OpenOrCreate);

            while (filesize > 0)
            {
                buffer = new byte[bufferSize];

                int size = socket.Receive(buffer, SocketFlags.Partial);

                fs.Write(buffer, 0, size);

                filesize -= size;
            }


            fs.Close();

            //TODO: KILL THIS WITH FIRE

            tcpFileListener.Stop();

            //TODO: KILL THIS WITH FIRE

            ThreadStart tcprec = new ThreadStart(TcpFileReciverAsync);
            tcpReciving = new Thread(tcprec);
            tcpReciving.IsBackground = true;
            tcpReciving.Start();
        }

        static async System.Threading.Tasks.Task<string> SaveFileAsync(string name)
        {
            var sf = new SaveFileDialog();
            sf.InitialFileName = name;
            var result = await sf.ShowAsync(null);
            if (result != null)

            {
                return result;
            }
            return "";

        }

        public static void SendUdpPacket(string d)
        {
            if (!string.IsNullOrEmpty(d))
            {
                byte[] data = Encoding.ASCII.GetBytes(d);
                sendingClient.Send(data, data.Length);
            }
        }


        //rewrite
        public static void SendTcpPacket(string targetIp, string p)
        {
            if (!string.IsNullOrEmpty(targetIp) && !string.IsNullOrEmpty(p))
            {
                if (tcpPacketSender == null)
                {
                    tcpPacketSender = new TcpClient(targetIp, tcpPacketsPort);
                    tcpPacketSender.SendTimeout = 600000;
                    tcpPacketSender.ReceiveTimeout = 600000;
                }
                byte[] data = Encoding.ASCII.GetBytes(p);
                tcpPacketSender.Client.Send(data);
            }
        }


        private static void UdpReciver()
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, udpPort);
            AddMessage messageDelegate = UdpMessageRecived;

            while (true)
            {
                byte[] data = receivingClient.Receive(ref endPoint);
                string message = Encoding.ASCII.GetString(data);
                UdpMessageRecived(message);
            }
        }

        //rename stuff

        private static void UdpMessageRecived(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
            OnMessageEvent.Invoke(message);
        }


        //this looks like shit
        public static void SendFileTo(string filePath, string ip)
        {
            string Filename = filePath;


            int bufferSize = 1024;
            byte[] buffer = null;
            byte[] header = null;


            FileStream fs = new FileStream(Filename, FileMode.Open);
            bool read = true;

            int bufferCount = Convert.ToInt32(Math.Ceiling((double)fs.Length / (double)bufferSize));

            TcpClient tcpClient = new TcpClient(ip, tcpFilesPort);
            tcpClient.SendTimeout = 600000;
            tcpClient.ReceiveTimeout = 600000;

            // string headerStr = "Content-length:" + fs.Length.ToString() + "\r\nFilename:" + @"C:\Users\Administrator\Desktop\" + "test.zip\r\n";
            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string fileName = Path.GetFileName(filePath);

            string headerStr = "Content-length:" + fs.Length.ToString() + "\r\nFilename:" +fileName +"\r\n";
            header = new byte[bufferSize];
            Array.Copy(Encoding.ASCII.GetBytes(headerStr), header, Encoding.ASCII.GetBytes(headerStr).Length);

            tcpClient.Client.Send(header);

            for (int i = 0; i < bufferCount; i++)
            {
                buffer = new byte[bufferSize];
                int size = fs.Read(buffer, 0, bufferSize);

                tcpClient.Client.Send(buffer, size, SocketFlags.Partial);

            }

            tcpClient.Client.Close();

            fs.Close();
        }

        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
    }
}
