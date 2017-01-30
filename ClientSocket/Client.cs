using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientSocket
{
    public partial class Client : Form
    {
        public Client()
        {
            InitializeComponent();
        }
        Socket socketSend;

        private void btnStart_Click(object sender, EventArgs e)
        {
            socketSend = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ip = IPAddress.Parse("192.168.220.1");
            IPEndPoint point = new IPEndPoint(ip, 50000);
            socketSend.Connect(point);
            ShowMsg("連線成功");
            Thread th = new Thread(Receive);
            th.IsBackground = true;
            th.Start();
        }

        void ShowMsg(string txt)
        {
            textBox1.AppendText(txt);
        }

        private void Call(string str)
        {
            Action<string> action = ShowMsg;
            this.Invoke(action, str);
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            string txt = textBox2.Text.Trim();
            byte[] buffer = Encoding.UTF8.GetBytes(txt);
            socketSend.Send(buffer);
        }

        void Listen(object o)
        {
            Socket socketWatch = o as Socket;
            while (true)
            {
                socketSend = socketWatch.Accept();
                //ShowMsg(socketSend.RemoteEndPoint.ToString() + ":" + "連線成功");
                Call(socketSend.RemoteEndPoint.ToString() + ":" + "連線成功");
                Thread th = new Thread(Receive);
                th.IsBackground = true;
                th.Start();
            }
        }

        void Receive()
        {
            //socketSend = o as Socket;
            while (true)
            {
                byte[] buffer = new byte[1024 * 1024 * 2];
                try
                {
                    int r = socketSend.Receive(buffer);
                    if (r == 0)
                    {
                        break;
                    }
                    string str = Encoding.UTF8.GetString(buffer, 0, r);
                    //ShowMsg(socketSend.RemoteEndPoint + ":" + str);
                    Call(socketSend.RemoteEndPoint + ":" + str);
                }
                catch(Exception)
                {
                    MessageBox.Show("接收訊息錯誤");
                }
            }
        }

        private void Client_Load(object sender, EventArgs e)
        {
            //Control.CheckForIllegalCrossThreadCalls = false;
        }
    }
}
