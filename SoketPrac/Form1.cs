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

namespace SoketPrac
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Control.CheckForIllegalCrossThreadCalls = false;
        }

        private void Call(string str)
        {
            Action<string> action = ShowMsg;
            this.Invoke(action, str);
        }

        Socket socketSend;
        void Listen(object o)
        {
            Socket socketWatch = o as Socket;
            while(true)
            {
                socketSend = socketWatch.Accept();
                //ShowMsg(socketSend.RemoteEndPoint.ToString() + ":" + "連線成功");
                Call(socketSend.RemoteEndPoint.ToString() + ":" + "連線成功");
                Thread th = new Thread(Receive);
                th.IsBackground = true;
                th.Start(socketSend);
            }
        }

        void ShowMsg(string txt)
        {
            textBox1.AppendText(txt);
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            string txt = textBox2.Text;
            byte[] buffer = Encoding.UTF8.GetBytes(txt);
            socketSend.Send(buffer);
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            Socket socketWatch = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ip = IPAddress.Any;
            IPEndPoint point = new IPEndPoint(ip, 50000);
            socketWatch.Bind(point);
            ShowMsg("監聽成功");
            socketWatch.Listen(10);

            Thread th = new Thread(Listen);
            th.IsBackground = true;
            th.Start(socketWatch);
        }

        void Receive(object o)
        {
            socketSend = o as Socket;
            while(true)
            {
                byte[] buffer = new byte[1024 * 1024 * 2];
                try
                {
                    int r = socketSend.Receive(buffer);
                    string str = Encoding.UTF8.GetString(buffer, 0, r);
                    //ShowMsg(socketSend.RemoteEndPoint + ":" + str);
                     Call(socketSend.RemoteEndPoint + ":" + str);
                    if (r == 0)
                    {
                        break;
                    }
                }
                catch(Exception)
                {
                    MessageBox.Show("接收訊息錯誤");
                }                
            }
        }
    }
}
