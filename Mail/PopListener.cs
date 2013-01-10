using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Weavver.Net.Mail.POP3
{
	/// <summary>
	/// Calling this should return a path to the user's mail box. If the user is not authorized it should return "error".
	/// </summary>
	public delegate string dAuthorizePopUser (PopClient popclient, string user, string pass);
     public delegate void dDebugOutput(PopClient popclient, string message, POPDebugType debugtype);

	public class PopListener
	{
		public event			dAuthorizePopUser	AuthorizePopUser;
		public event			dDebugOutput		DebugOutput;

		private string			servername			= "unknown.server";
		public	ArrayList		connections			= new ArrayList();
		private bool			listening			= false;
		private WaitCallback	_OnAccept			= null;
		private Socket			listenersocket		= null;
//-------------------------------------------------------------------------------------------
		public PopListener()
		{
			_OnAccept = new WaitCallback(OnAccept);
		}
//-------------------------------------------------------------------------------------------
		public string ServerName
		{
			get
			{
				return servername;
			}
			set
			{
				servername = value;
			}
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
		public void Listen(IPEndPoint ipendpoint)
		{

			Socket Listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			listenersocket	= Listener;
			try
			{
				Listener.Bind(new IPEndPoint(IPAddress.Any, 110));
				Listener.Listen((int) SocketOptionName.MaxConnections);
				listening = true;
				DebugOut(null, "Pop listening on: " + Listener.LocalEndPoint.ToString(), POPDebugType.Listen);
			}
			catch(Exception e)
			{
                    DebugOut(null, "Pop failed to bind: " + e.Message, POPDebugType.Critical);
				return;
			}

			while (Listening && !Environment.HasShutdownStarted)
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
			listening = false;
		}
//-------------------------------------------------------------------------------------------
		private void OnAccept(Object sock)
		{
			try
			{
				PopClient client = new PopClient((Socket) sock, this);

				connections.Add(client);
				client.HandleConnection();
				connections.Remove(client);

				sock	= null;
				client	= null;
			}
			catch (Exception e)
			{
                    DebugOut(null, "Unknown error:\r\n\r\n" + e.Message, POPDebugType.Critical);
			}
		}
//-------------------------------------------------------------------------------------------
		public void DebugOut(PopClient popclient, string message, POPDebugType debugtype)
		{
            if (DebugOutput != null)
				DebugOutput(popclient, message, debugtype);
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
		public string AuthorizeUser(PopClient popclient, string user, string pass)
		{
			if (AuthorizePopUser != null)
				return AuthorizePopUser(popclient, user, pass);
			else
				return "Error";
		}
//-------------------------------------------------------------------------------------------
	}
}
