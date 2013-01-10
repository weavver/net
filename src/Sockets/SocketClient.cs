using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace Weavver.Sockets
{
	public class SocketClient
	{
          public	Socket         socket           = null;
		private   TimerCallback  dConnectionWatch = null;
          private   Timer          tConnectionWatch = null;
		private	byte[]         buffer	       = new byte[256];
		private   string	     data			  = "";
          public ManualResetEvent waitForAuthenticated = new ManualResetEvent(false);
//--------------------------------------------------------------------------------------------
		public SocketClient()
		{
               dConnectionWatch = new TimerCallback(ConnectionWatch);
               tConnectionWatch = new Timer(dConnectionWatch, this, Timeout.Infinite, 7500);
		}
//--------------------------------------------------------------------------------------------
          public void ConnectionWatch(object stateInfo) 
          { 
               SocketClient client = (SocketClient) stateInfo;
               if (!client.socket.Connected)
               {
                    Disconnect();
                    OnConnectionFailed();
               }
          }
//--------------------------------------------------------------------------------------------
		public SocketClient(Socket socket)
		{
			this.socket = socket;
               CallBackReceive();
		}
//--------------------------------------------------------------------------------------------
		public void Connect(string ipaddress, int port)
		{
			Connect(IPAddress.Parse(ipaddress), port);
		}
//--------------------------------------------------------------------------------------------
		public void Connect(IPAddress ipaddress, int port)
		{
			if (socket != null)
			{
                    socket.Close();
                    socket = null;
               }
               socket                        = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
               socket.Blocking               = false;
               IPEndPoint    ipendpoint      = new IPEndPoint(ipaddress, port);
			try
			{
                    AsyncCallback callback        = new AsyncCallback(OnConnectionEvent);
                    tConnectionWatch.Change(TimeSpan.FromSeconds(7.5), TimeSpan.FromSeconds(7.5));
				socket.BeginConnect(ipendpoint, callback, this);

                    waitForAuthenticated.WaitOne(3000);
			}
			catch
			{
				return;
			}
          }
//--------------------------------------------------------------------------------------------
		public void OnConnectionEvent(IAsyncResult ar)
		{
               if (!socket.Connected)
               {
                    OnConnectionFailed();
                    return;
               }
               else
               {
                    OnConnected(this);
                    CallBackReceive();
               }
		}
//--------------------------------------------------------------------------------------------
          public virtual void OnConnected(SocketClient socketclient)
          {
          }
//--------------------------------------------------------------------------------------------
          public virtual void OnConnectionFailed()
          {
          }
//--------------------------------------------------------------------------------------------
		public void OnRecievedData(IAsyncResult ar)
		{
			try
			{
                    if (socket.Connected)
                    {
                         int bytesreceived = socket.EndReceive(ar);
                         if (bytesreceived > 0)
                         {
                              string line = "";
                              data += Encoding.ASCII.GetString(buffer, 0, bytesreceived); ;

                              while (data.IndexOf("\r\n") > -1)
                              {
                                   line = Regex.Split(data, Environment.NewLine)[0].ToString();
                                   HandleLine(line);
                                   data = data.Substring((data.IndexOf("\r\n") + 2));
                              }
                              CallBackReceive();
                         }
                         else
                         {
                              Console.WriteLine("Client {0}, disconnected", socket.RemoteEndPoint);
                              Disconnect();
                         }
                    }
                    else
                    {
                         OnDisconnect();
                    }
			}
			catch (Exception e)
			{
                    Disconnect();
                    Console.WriteLine(e.ToString());
			}
		}
//--------------------------------------------------------------------------------------------
          public virtual void OnDisconnect()
          {
          }
//--------------------------------------------------------------------------------------------
		public void Disconnect()
		{
               try
               {
                    tConnectionWatch.Change(Timeout.Infinite, 500);
                    tConnectionWatch.Dispose();
                    tConnectionWatch = null;
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
               }
               catch { }
               OnDisconnect();
		}
//--------------------------------------------------------------------------------------------
		public void CallBackReceive()
		{
			try
			{
				AsyncCallback callback = new AsyncCallback(OnRecievedData);
				socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, callback, socket);
			}
			catch
			{
			}
		}
//--------------------------------------------------------------------------------------------
		public virtual void HandleLine(string line)
		{
			return;
		}
//--------------------------------------------------------------------------------------------
		public void SendLine(string data)
		{
			SendData(data + "\r\n");
		}
//--------------------------------------------------------------------------------------------
		public void SendData(string data)
		{
			try
			{
				if (socket.Connected)
				{
					Byte[] bytes	     = System.Text.Encoding.ASCII.GetBytes(data);
					socket.Send(bytes, 0, bytes.Length, SocketFlags.None);
					bytes			= null;
				}
			}
			catch {}
		}
//--------------------------------------------------------------------------------------------
	}
}
