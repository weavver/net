using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Weavver.Net.Mail
{
	public delegate bool dAuthorizeIPRelay(SmtpClient smtpclient, EndPoint endpoint);
	public delegate bool dAuthorizeMAIL(SmtpClient smtpclient, string address);
	public delegate ArrayList dAuthorizeRCPT(SmtpClient smtpclient, Address address,	bool AllowRelay);
	public delegate bool dAuthorizeUserRelay(SmtpClient smtpclient, string user, string pass);
	public delegate void dDebugOutput(string message, bool error);
	public delegate void dQueueMail(SmtpClient smtpclient, string messagepath);
     public delegate void dHandleReceivedMessage(SmtpClient smtpclient, string messagepath);
	public delegate void dClientConnected(SmtpClient smtpclient);
	public delegate void dClientDisconnected(SmtpClient smtpclient);

	public class SmtpListener
	{
		public event dAuthorizeIPRelay AuthorizeIPRelay;
		public event dAuthorizeMAIL AuthorizeMAIL;
		public event dAuthorizeRCPT AuthorizeRCPT;
		public event dAuthorizeUserRelay AuthorizeUserRelay;
		public event dDebugOutput DebugOutput;
		public event dQueueMail QueueMail;
          public event dHandleReceivedMessage HandleMessageReceived;
		public event dClientConnected ClientConnected;
		public event dClientDisconnected ClientDisconnected;

		private string	servername = "servername.not.set";
		private string	smtpdirectory = @"C:\Program Files (x86)\Weavver\VoiceScribe\spool\mail\";
		private string	mailpath = "";

		public ArrayList connections	= new ArrayList();
		private WaitCallback _OnAccept;

		private bool listening = false;
		public Socket listenersocket = null;
//-------------------------------------------------------------------------------------------
		public SmtpListener()
		{
			_OnAccept = new WaitCallback(OnAccept);

               if (System.Environment.OSVersion.Platform != PlatformID.Win32NT)
               {
                    smtpdirectory = "/var/spool/voicescribe/";
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
		public string SmtpDirectory
		{
			get
			{
				return smtpdirectory;
			}
			set
			{
				smtpdirectory = value;
                    if (!smtpdirectory.EndsWith(System.IO.Path.DirectorySeparatorChar.ToString()))
                         smtpdirectory += System.IO.Path.DirectorySeparatorChar;
			}
		}
//-------------------------------------------------------------------------------------------
		public string MailPath
		{
			get
			{
				return mailpath;
			}
			set
			{
				mailpath = value;
                    if (!mailpath.EndsWith(System.IO.Path.DirectorySeparatorChar.ToString()))
                         mailpath += System.IO.Path.DirectorySeparatorChar;
			}
		}
//-------------------------------------------------------------------------------------------
		public void Listen(IPEndPoint ipendpoint)
		{
			Socket Listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			listenersocket	= Listener;
			try
			{
				Listener.Bind(ipendpoint);
				Listener.Listen((int) SocketOptionName.MaxConnections);
				
				DebugOut("Smtp listening on: " + Listener.LocalEndPoint.ToString(), false);
			}
			catch(Exception e)
			{
				DebugOut("Smtp failed to bind: " + e.Message, true);
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
		private void OnAccept(Object pSock)
		{
			try
			{
				SmtpClient smtpclient = new SmtpClient(this, (Socket)pSock);
				
				if (ClientConnected != null)
				{
					ClientConnected(smtpclient);
				}

				connections.Add(smtpclient);
				smtpclient.HandleConnection();
				connections.Remove(smtpclient);
				
				pSock		= null;
				smtpclient	= null;
			}
			catch (Exception e)
			{
				DebugOut("Unknown error:\r\n\r\n" + e.Message, true);
			}
		}
//-------------------------------------------------------------------------------------------
		public bool CheckIPRelay(SmtpClient smtpclient, EndPoint endpoint)
		{
			if (AuthorizeIPRelay != null)
				return AuthorizeIPRelay(smtpclient, endpoint);
			else
				return false;
		}
//-------------------------------------------------------------------------------------------
		public bool CheckUserRelay(SmtpClient smtpclient, string user, string pass)
		{
			if (AuthorizeUserRelay != null)
				return AuthorizeUserRelay(smtpclient, user, pass);
			else
				return false;
		}
//-------------------------------------------------------------------------------------------
		public bool CheckMAIL(SmtpClient smtpclient, string address)
		{
			if (AuthorizeMAIL != null)
				return AuthorizeMAIL(smtpclient, address);
			else
				return false;
		}
//-------------------------------------------------------------------------------------------
		public ArrayList CheckRCPT(SmtpClient smtpclient, Address address, bool AllowRelay)
		{
			if (AuthorizeRCPT != null)
				return AuthorizeRCPT(smtpclient, address, AllowRelay);
			else
				return null;
		}
//-------------------------------------------------------------------------------------------
		public void DebugOut(string message, bool error)
		{
			if (DebugOutput != null)
				DebugOutput(message, error);
		}
//-------------------------------------------------------------------------------------------
		public void ProcessHandleMessageReceived(SmtpClient smtpclient, string mailpath)
		{
               if (HandleMessageReceived != null)
                    HandleMessageReceived(smtpclient, mailpath);
		}
//-------------------------------------------------------------------------------------------
		public void QueueMailOut(SmtpClient smtpclient, string mailpath)
		{
			 if (QueueMail != null)
				 QueueMail(smtpclient, mailpath);
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
	}
}