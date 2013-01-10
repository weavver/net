using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Xml.Serialization;

namespace Weavver.Sockets
{
	public class SocketServer
	{
		public	ArrayList		connections			= new ArrayList();
		private   WaitCallback	_OnAccept;
		private   bool			listening				= false;
		private	Socket		listenersocket			= null;
//-------------------------------------------------------------------------------------------
		public SocketServer()
		{
			_OnAccept = new WaitCallback(OnAccept);
		}
//-------------------------------------------------------------------------------------------
		public bool Listening
		{
			get
			{
				return listening;
			}
		}
//-------------------------------------------------------------------------------------------
		public void Listen(string ipaddress, int port)
		{
			IPEndPoint ipendpoint	= new IPEndPoint(IPAddress.Parse(ipaddress), port);
			Socket Listener		= new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			listenersocket			= Listener;
			try
			{
				Listener.Bind(ipendpoint);
				Listener.Listen((int) SocketOptionName.MaxConnections);
				
				DebugOut("Listening on: " + Listener.LocalEndPoint.ToString(), false);
			}
			catch(Exception e)
			{
				DebugOut("Failed to bind: " + e.Message, true);
				return;
			}
			
			while (!Listening && !Environment.HasShutdownStarted)
			{
				try 
				{
					ThreadPool.QueueUserWorkItem(_OnAccept, Listener.Accept());
				}
				catch 
				{
					Thread.Sleep(100);
				}
			}

			Listener.Shutdown(SocketShutdown.Both);
			Listener.Close();
		}
//-------------------------------------------------------------------------------------------
		public virtual void OnAccept(Object pSock)
		{
			Socket socket		= (Socket) pSock;
			SocketClient client = new SocketClient(socket);
			try
			{
				connections.Add(client);
			}
			catch (Exception e)
			{
				DebugOut("Unknown error:\r\n\r\n" + e.ToString(), true);
				if (client != null)
				{
					client.Disconnect();
				}
				try
				{
					client = null;
				}
				catch
				{
				}
			}
		}
//-------------------------------------------------------------------------------------------
		public void StopListening()
		{
			listening = false;
			try
			{
				listenersocket.Close();
			}
			catch {}
		}
//-------------------------------------------------------------------------------------------
		public void DebugOut(string data, bool error)
		{
               Console.WriteLine(data);
		}
//-------------------------------------------------------------------------------------------
     }
}