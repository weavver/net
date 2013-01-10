using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Text.RegularExpressions;

namespace TitaniumSoft.Mail.Admin
{
	public class AdminConnection
	{
		Socket		connection		= new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		EndPoint	ep				= null;
          public static string newline = "\r\n"; // newline

		private PostOfficeCollection postoffices = new PostOfficeCollection();

		public	event	EventHandler AddressesReceived;
		public	event	EventHandler PostOfficesReceived;
		public	event	EventHandler MailBoxesReceived;
		public	event	EventHandler DomainsReceived;
		public	event	EventHandler Addresses;

		public	event	EventHandler Connected;
		public	event	EventHandler Authorized;

		public	event	EventHandler Disconnected;

		private string	username		= "";
		private string	password		= "";

		private bool	disconnect		= false;
		private bool	ignoreinput 	= false;

		private bool	listaddresses	= false;
		private bool	listdomains		= false;
		private bool	listpostoffices = false;
		private bool	listmailboxes	= false;
		private bool	loggedin		= false;

		private bool	watchforeof		= false;

		private string	remotehost		= "";
		
		public string RemoteHost
		{
			get { return remotehost; }
		}

		private WaitCallback _HandleConnection;
		
		public void Login(string _username, string _password, string host, int port)
		{
			username		= _username;
			password		= _password;

			listpostoffices = true;

			IPAddress ip	= null;
			try
			{
				ip			= IPAddress.Parse(host);
			}
			catch
			{
				IPHostEntry iphe	= Dns.Resolve(host);
				ip					= iphe.AddressList[0];
			}
			this.ep			= new IPEndPoint(ip, port);
			
			remotehost		= ip.ToString();

			_HandleConnection	= new WaitCallback(HandleConnection);
			ThreadPool.QueueUserWorkItem(_HandleConnection);
		}
		
		#region Handle Connection
		public void HandleConnection(object unused)
		{		
			Thread.Sleep(10);

			LingerOption lingerOption = new LingerOption(true, 10);
			connection.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Linger, lingerOption);

			try
			{			
				connection.Connect(ep);
			}
			catch {}

			int		ret;
			decimal dTimeOut	= DateTime.Now.ToFileTime();

			string	aData		= "";
			string	bData		= "";

			byte[]	RecvBytes;

			if (connection.Connected)
				Connected.BeginInvoke(null, null, null, null);

			while (connection.Connected && !disconnect)
			{
				if (connection.Available > 0)
				{
					RecvBytes	= new byte[connection.Available];
					ret			= connection.Receive(RecvBytes, 0, RecvBytes.Length, SocketFlags.None);
					aData		= aData + Encoding.ASCII.GetString(RecvBytes).Substring(0, ret);

								
					while (aData.IndexOf(newline) > -1)
					{
						bData	= Regex.Split(aData, newline)[0].ToString();
						HandleData(bData);
						aData	= aData.Substring((aData.IndexOf(newline) + 2));
					}
					dTimeOut = DateTime.Now.ToFileTime();
				}
				else
				{
					/*
					 * myData.bStayConnected = ((dTimeOut + 9000000000 /* 30 minutes )/* < DateTime.Now.ToFileTime() ? false : true);
					/*if (!myData.bStayConnected)
					{
						Send("451 Timeout" + newline);
					}
					*/
					Thread.Sleep(1);
				}
			}
			//mySocket.Shutdown(SocketShutdown.Both);
			connection.Close();
			try
			{
				Disconnected.BeginInvoke(null, null, null, null);
			}
			catch {}
		}
		#endregion

		public void HandleData(string line)
		{
			if (ignoreinput)
				return;

			line = line.Replace("\t", " ");

			string	scode = (line.IndexOf(" ") > 0) ? line.Substring(0, line.IndexOf(" ")) : line;
			
			int	icode = 411;
			if (scode.IndexOf("-") != 3)
				icode = Int32.Parse(scode);

			switch (icode)
			{
				case 100:
					if (!loggedin)
					{
						Send("user " + username + "\r\n");
						Send("pass " + password + "\r\n");
						
						Send("postoffices\r\n");

						loggedin = true;
					}					
					break;

				case 200:
					if (watchforeof && line.StartsWith("200 EOF:"))
					{

						if (listpostoffices)
						{
							PostOfficesReceived.BeginInvoke(null, null, null, null);
							listpostoffices = false;
						}
						else if (listmailboxes)
						{
							MailBoxesReceived.BeginInvoke(this, null, null, null);
							listmailboxes	= false;
						}
						else if (listdomains)
						{
							DomainsReceived.BeginInvoke(this, null, null, null);
							listdomains		= false;
						}
						else if (listaddresses)
						{
							AddressesReceived.BeginInvoke(this, null, null, null);
							listaddresses = false;
						}
						watchforeof = false;
					}
					break;

				case 500:
					//if (listpostoffices)
					//	Send("list detailed\r\n");
					break;

				case 411:

					break;

				#region Mode 510
				case 510:
					if (listpostoffices)
					{
						watchforeof			= true;
						PostOffice po		= new PostOffice();

						line				= line.Substring(line.IndexOf(" ") + 1).Trim();
						string[] pa			= line.Split(";".ToCharArray(), 5);
						for (int i = 0; i < pa.Length; i++)
						{
							line = pa[i];
							switch (pa[i].Substring(0, ((pa[i].IndexOf(":") > 0)) ? pa[i].IndexOf(":") : pa[i].Length).Trim())
							{
								case "ID":
									po.ID			= line.Substring(line.IndexOf(":") + 1).Trim();
									break;

								case "Default":
									po.IsDefault	= (Int32.Parse(pa[i].Substring(pa[i].IndexOf(":") + 1).Trim()) == 1) ? true : false;
									break;

								case "Status":
									po.Status		= Int32.Parse(pa[i].Substring(pa[i].IndexOf(":") + 1).Trim());
									break;

								case "Description":
									po.Description = pa[i].Substring(pa[i].IndexOf(":") + 1).Trim();
									break;

								default:
									if (i == 1)
									{
										po.Name		= line.Substring(line.IndexOf("\\\\") + 2).Trim();
									}
									break;
							}
						}
						if (postoffices.findByID(po.ID) == null)
						{
							postoffices.Add(po);
						}
					}
					break;
				#endregion

				#region Mode 610
				case 610:
					if (listdomains)
					{
						string domainpostoffice = null;
						Domain d			= new Domain();

						line				= line.Substring(line.IndexOf(" ") + 1).Trim();
						string[] da			= line.Split(";".ToCharArray(), 5);
						for (int i = 0; i < da.Length; i++)
						{
							switch (da[i].Substring(0, ((da[i].IndexOf(":") > 0)) ? da[i].IndexOf(":") : da[i].Length).Trim())
							{
								case "ID":
									d.ID		= da[i].Substring(da[i].IndexOf(":") + 1).Trim();
									break;

								case "Status":
									d.Status	= Int32.Parse(da[i].Substring(da[i].IndexOf(":") + 1).Trim());
									break;

								case "Misc":
									d.Misc		= da[i].Substring(da[i].IndexOf(":") + 1).Trim();
									break;

								case "Description":
									d.Description	= da[i].Substring(da[i].IndexOf(":") + 1).Trim();
									break;

								default:
									if (i == 1)
									{
										line				= da[i];
										line				= line.Substring(line.IndexOf("\\\\") + 2);
										domainpostoffice	= line.Substring(0, line.LastIndexOf("\\"));
										d.Name				= line.Substring(line.LastIndexOf("@") + 1).Trim();
									}
									break;
							}
						}
						if (postoffices.findByName(domainpostoffice).Domains.findByID(d.ID) == null)
						{
							postoffices.findByName(domainpostoffice).Domains.Add(d);
						}
					}
					break;
					#endregion

				#region Mode 710
				case 710:
					if (listmailboxes)
					{
						string mailboxpostoffice = "";
						MailBox mb			= new MailBox();
						
						line				= line.Substring(line.IndexOf(" ") + 1).Trim();
						string[] mba		= line.Split(";".ToCharArray(), 6);
						for (int i = 0; i < mba.Length; i++)
						{
							line = mba[i];
							switch (mba[i].Substring(0, ((mba[i].IndexOf(":") > 0)) ? mba[i].IndexOf(":") : mba[i].Length).Trim())
							{
								case "ID":
									mb.ID			= mba[i].Substring(mba[i].IndexOf(":") + 1).Trim();
									break;

								case "Quota":
									mb.Quota		= Int32.Parse(mba[i].Substring(mba[i].IndexOf(":") + 1).Trim());
									break;

								case "Status":
									mb.Status		= Int32.Parse(mba[i].Substring(mba[i].IndexOf(":") + 1).Trim());
									break;

								case "Misc":
									mb.Misc			= mba[i].Substring(mba[i].IndexOf(":") + 1).Trim();
									break;

								case "Description":
									mb.Description	= mba[i].Substring(mba[i].IndexOf(":") + 1).Trim();
									break;

								default:
									if (i == 1)
									{
										line				= line.Substring(line.IndexOf("\\\\") + 2);
										mailboxpostoffice	= line.Substring(0, line.IndexOf("\\"));
										mb.Name				= line.Substring(line.LastIndexOf("\\") + 1).Trim();
									}
									break;
							}
						}
						if (postoffices.findByName(mailboxpostoffice).MailBoxes.findByID(mb.ID) == null)
						{
							postoffices.findByName(mailboxpostoffice).MailBoxes.Add(mb);
						}
					}
					break;
					#endregion

				#region Mode 810
				case 810:
					if (listaddresses)
					{
						Address a					= new Address();
						string addresspostofficename= String.Empty;
						string addressmailboxname	= String.Empty;

						line						= line.Substring(line.IndexOf(" ") + 1).Trim();
						string[] aa					= line.Split(";".ToCharArray(), 5);

						for (int i = 0; i < aa.Length; i++)
						{
							switch (aa[i].Substring(0, ((aa[i].IndexOf(":") > 0)) ? aa[i].IndexOf(":") : aa[i].Length).Trim())
							{
								case "ID":
									a.ID		= aa[i].Substring(aa[i].IndexOf(":") + 1).Trim();
									break;

								case "Status":
									a.Status	= Int32.Parse(aa[i].Substring(aa[i].IndexOf(":") + 1).Trim());
									break;

								case "Misc":
									a.Misc		= aa[i].Substring(aa[i].IndexOf(":") + 1).Trim();
									break;

								case "SMS ID":
									a.SMSID		= aa[i].Substring(aa[i].IndexOf(":") + 1).Trim();
									break;

								default:
									if (i == 1)
									{
										line					= aa[i].Trim();
										a.Name					= line.Substring(line.LastIndexOf("\\") + 1);
										line					= line.Substring(0, line.LastIndexOf("\\"));
										addressmailboxname		= line.Substring(line.LastIndexOf("\\") + 1);
										line					= line.Substring(0, line.LastIndexOf("\\"));
										//line					= line.Substring(line.IndexOf("\\"));
										addresspostofficename	= line.Substring(2);
									}
									break;
							}
						}
						postoffices.findByName(addresspostofficename).MailBoxes.findByName(addressmailboxname).Addresses.Add(a);
					}
					break;
					#endregion

				case 900:
					
					break;

				default:

					break;
			}
		}

		#region AddPostOffice(string postoffice, string status, bool setdefault, string description)
		public void AddPostOffice(string postoffice, string status, bool setdefault, string description)
		{
			ignoreinput = true;

			Send("status 100\r\n");
			Send("postoffices\r\n");
			Send("add\r\n");
			Send(postoffice + "\r\n");
			Send(Normalize(description) + "\r\n");
			Send("status 100\r\n");

			ignoreinput = false;
		}
		#endregion
		#region AddMailBox(string postoffice, string mailbox, string password, string description)
		public void AddMailBox(string postoffice, string mailbox, string password, string description)
		{
			ignoreinput = true;

			Send("status 500\r\n");
			Send("postoffices\r\n");
			Send("postoffice " + postoffice + "\r\n");
			Send("mailboxes\r\n");
			Send("add\r\n");
			Send(mailbox + "\r\n");
			Send(password + "\r\n");
			Send(Normalize(description) + "\r\n");
			Send("status 100\r\n");

			ignoreinput = false;
		}
		#endregion

		#region getPostOffices()
		public void getPostOffices()
		{
			listpostoffices	= true;

			listdomains		= false;
			listmailboxes	= false;
			listaddresses	= false;
			
			Send("status 100\r\n");
			Send("postoffices\r\n");
			Send("list detailed\r\n");
		}
		#endregion

		#region getAddresses(string postofficeid, string mailboxid)
		public void getAddresses(string postofficeid, string mailboxid)
		{
			listaddresses	= true;
			watchforeof		= true;

			listpostoffices	= false;
			listdomains		= false;
			listmailboxes	= false;


			Send("status 100\r\n");
			Send("postoffices\r\n");
			Send("postoffice " + postofficeid + "\r\n");
			Send("mailboxes\r\n");
			Send("mailbox " + mailboxid + "\r\n");
			Send("addresses\r\n");
			Send("list detailed\r\n");
		}
		#endregion
		
		#region getMailBoxes(string postofficeid)
		public void getMailBoxes(string postofficeid)
		{
			listpostoffices	= false;
			listmailboxes	= true;
			watchforeof		= true;

			Send("status 100\r\n");
			Send("postoffices\r\n");
			Send("postoffice " + postofficeid + "\r\n");
			Send("mailboxes\r\n");
			Send("list detailed\r\n");
		}
		#endregion
		#region getDomains(string postofficeid)
		public void getDomains(string postofficeid)
		{
			SetFlagsFalse();
			listdomains		= true;
			watchforeof		= true;

			Send("status 100\r\n");
			Send("postoffices\r\n");
			Send("postoffice " + postofficeid + "\r\n");
			Send("domains\r\n");
			Send("list detailed\r\n");
		}
		#endregion

		public void deletePostOffice(int postofficeid)
		{
			SetFlagsFalse();

			Send("status 100\r\n");
			Send("postoffices\r\n");
			Send("postoffice " + postofficeid.ToString() + "\r\n");
			Send("delete\r\n");
		}

		#region Send
		private void Send(string strSend)
		{
			try
			{
				Byte[] bSend = System.Text.Encoding.ASCII.GetBytes(strSend);
				this.connection.Send(bSend, 0, bSend.Length, SocketFlags.None);
			}
			catch (Exception e)
			{
				System.Diagnostics.Debug.WriteLine(e.ToString());
				//myData.pService.DebugOut("An error was encountered while trying to send data to a remote address. \r\n\r\n" + e.Message, true);
			}
		}
		#endregion

		private string Normalize(string data)
		{
			data = data.Replace("&", "&amp;");
			data = data.Replace("<", "&lt;");
			return data.Replace("\r\n", "<br>");
		}
	
		public string getStatusName(int status)
		{
			switch (status)
			{
				case 0:
					return "Blocked";

				case 1:
					return "Normal";

				case 2:
					return "Forward";

				case 3:
					return "Execute";

				case 4:
					return "Auto Respond";

				case 5:
					return "Remote Server";

				case 6:
					return "SMS";

				case 7:
					return	"Disposable";

				default:
					return "Unknown";
			}
		}

		public PostOfficeCollection PostOffices
		{
			get {return postoffices; }
			set {postoffices = value; }
		}

		private void SetFlagsFalse()
		{
			listpostoffices	= false;
			listmailboxes	= false;
			listaddresses	= false;
		}
	}
}
