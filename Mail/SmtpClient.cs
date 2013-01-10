using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;

using TitaniumSoft;
using Weavver.Net.Mail;

namespace Weavver.Net.Mail
{
     public enum AddressEndPointStatus
     {
          Bad,
          Blocked,
          SMS,
          Forward,
          Plugin,
          Local,
          Remote,
          AutoRespond,
          Disposable,
          Invoke
     }
//-------------------------------------------------------------------------------------------
     public class SmtpClient
	{
          public static string newline = "\r\n"; // newline
		public _myData myData;
		private Socket client;
		private SmtpListener parent;
//-------------------------------------------------------------------------------------------
		public bool Connected
		{
			get
			{
				return client.Connected;
			}
		}
//-------------------------------------------------------------------------------------------
		public SmtpListener		Parent
		{
			get
			{
				return parent;
			}
			set
			{
				parent = value;
			}
		}
//-------------------------------------------------------------------------------------------
		public SmtpClient(SmtpListener parent, Socket socket)
		{
			client = socket;
			Parent = parent;
			myData.myEndPoint = client.RemoteEndPoint.ToString();
			myData.bStayConnected = true;
			myData.RCPT = new ArrayList();
		}
//-------------------------------------------------------------------------------------------
		private void Reset()
		{
			if (myData.IncomingFile != null)
			{
				try
				{
					myData.IncomingFile.Close();
					File.Delete(myData.IncomingFile.Name);
				}
				catch
				{
				}
			}

			myData.AuthPass = string.Empty;
			myData.AuthUser = string.Empty;
			myData.IncomingFile = null;
			myData.iState = 1;
			myData.Temp = "";
			myData.RCPT.Clear();
		}
//-------------------------------------------------------------------------------------------
		public void HandleConnection()
		{		
			Thread.Sleep(5);

			LingerOption lingerOption = new LingerOption(true, 5);
			client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Linger, lingerOption);

			Send("220 " + Parent.ServerName + " - WeavverLib ESMTP Server " + DateTime.Now.ToString("mm/dd/yy hh:mm:ss") + "\r\n");

			myData.iState = 1;
			myData.bForward = Parent.CheckIPRelay(this, client.RemoteEndPoint);

			int ret;
			decimal dTimeOut = DateTime.Now.ToFileTime();
			
			string aData = string.Empty;
			string bData = string.Empty;

			byte[] RecvBytes;
               
			while (client.Connected && myData.bStayConnected)
			{
				if (client.Available > 0)
				{
					RecvBytes	= new byte[client.Available];
					ret			= client.Receive(RecvBytes, 0, RecvBytes.Length, SocketFlags.None);
					aData		= aData + Encoding.ASCII.GetString(RecvBytes).Substring(0, ret);
							
					// Redirects incoming data to a file stream when the DATA command has been issued.
					if (myData.iState == 4)
					{
						SaveData(aData);
						aData = "";
					}
					else
					{
						while (aData.IndexOf(newline) > -1)
						{
                                   bData = Regex.Split(aData, newline)[0].ToString();
							HandleData(bData);
                                   aData = aData.Substring((aData.IndexOf(newline) + 2));
						}
					}
					RecvBytes	= null;
					dTimeOut	= DateTime.Now.ToFileTime();
				}
				else
				{
					myData.bStayConnected = ((dTimeOut + 9000000000 /* 30 minutes */) < DateTime.Now.ToFileTime() ? false : true);
					if (!myData.bStayConnected)
					{
                              Send("451 Timeout" + newline);
					}
					Thread.Sleep(1);
				}
			}
			//mySocket.Shutdown(SocketShutdown.Both);
			client.Close();
		}
//-------------------------------------------------------------------------------------------
		public void HandleData(string pData)
		{
			#region Parse Int
			int iSub	= (pData.Length > 4) ? 4 : pData.Length;
			#endregion

			switch (pData.Substring(0, iSub).ToUpper())
			{
				#region Case HELO
				case "HELO":
					myData.iState = 1;
                         Send("250 Welcome to our server, spam is not welcome here." + newline);
					break;
				#endregion
		
				#region Case EHLO
				case "EHLO":	
					myData.iState = 3;
                         Send("250-" + Parent.ServerName + " Weavver.Net.Mail - www.weavver.com" + newline);
                         Send("250-AUTH LOGIN" + newline);
                         Send("250 AUTH=LOGIN" + newline);
					break;
				#endregion

				#region Case AUTH
				case "AUTH":
					myData.iState = 10;
					Send("334 VXNlcm5hbWU6" + newline);
					break;
					#endregion

				#region Case MAIL
				case "MAIL":
					if (myData.iState == 1 || myData.iState == 3 || myData.iState > 9)
					{
						if (pData.IndexOf("<") > 0)
						{
							myData.MailFrom = pData.Substring(pData.IndexOf("<") + 1);
							myData.MailFrom = myData.MailFrom.Trim().Replace(">", "");

							myData.iState	= 2;
							bool allow		= Parent.CheckMAIL(this, myData.MailFrom);

							if (!allow)
							{
								Send("503 Permission denied.\r\n");
							}
							else
							{
								if (myData.MailFrom == "")
                                             Send("250 2.1.5 NULL SENDER OK:" + newline);
								else
                                             Send("250 2.1.5 Continue:" + newline);
							}
						}
						else
						{
							Send("503 Invalid MAIL command, please use this format -> MAIL FROM: <you@domain> <CRLF>" + newline);
						}
					}
					else
					{
						Send("503 Out of sequence" + newline);
					}
					break;
				#endregion

				#region Case RCPT
				case "RCPT":
					if (myData.iState > 1 && myData.iState < 10)
					{
						if (pData.IndexOf("<") > 0)
						{
							Address address		= new Address();

							myData.Temp			= pData.Substring(pData.IndexOf("<") + 1);
							
							address.SetAddress(myData.Temp.Replace(">", "").Trim());
						
							ArrayList al		= Parent.CheckRCPT(this, address, myData.bForward);
							if (al.Count == 0)
							{
								address			= new Address();
								address.EndPoint	= AddressEndPointStatus.Bad;
							}
							for (int i = 0; i < al.Count; i++)
							{
								address						= (Address) al[i];
								if (	address.EndPoint	== AddressEndPointStatus.Bad
									||	address.EndPoint	== AddressEndPointStatus.Blocked)
								{	
									address = (Address) al[i];

									if (i == al.Count && al.Count > 0)
										address = (Address) al[0];
									break;
								}
							}

							switch (address.EndPoint)
							{
								case AddressEndPointStatus.Bad:
								case AddressEndPointStatus.Blocked:
									if (address.Path == "ignore" && al.Count > 1)
									{
										for (int i = 0; i < al.Count; i++)
										{
											myData.RCPT.Add(al[i]);
										}
										Send("250 2.1.5 Command good - Continue:" + newline);
									}
									else
									{
										Send("503 2.1.5 " + address.Error + newline);
									}
									break;

								default:
									for (int i = 0; i < al.Count; i++)
									{
										myData.RCPT.Add(al[i]);
									}
									Send("250 2.1.5 Command good - Continue:" + newline);
									break;
							}
							myData.Temp = "";

							myData.iState = (myData.iState == 2 ? 3 : myData.iState);
						}
						else
						{
							Send("501 5.5.4 Invalid Address, please use this syntax: \"MAIL TO: <you@domain> <CRLF>\"" + newline);
						}
					}
					else
					{
						Send("503 Invalid state, please use the MAIL command first." + newline);
					}
					break;
				#endregion

				#region Case DATA
				case "DATA":
					if (myData.iState == 3 && myData.RCPT.Count > 0)
					{
						myData.iState = 4;
						try
						{
							if (!Directory.Exists(Parent.SmtpDirectory))
								Directory.CreateDirectory(Parent.SmtpDirectory);

							string filepath = Parent.SmtpDirectory + "in" + Path.DirectorySeparatorChar + FileName();

							myData.IncomingFile = File.Open(filepath, FileMode.Append, FileAccess.Write, FileShare.Read);
							string Received = "Received: from [{0}] by {1} via SMTP; {2}\r\n";
							Received = String.Format(Received, myData.myEndPoint, Parent.ServerName, DateTime.Now.ToString("g"));

							System.Byte[] lBytes = System.Text.Encoding.ASCII.GetBytes(Received);
							myData.IncomingFile.Write(lBytes, 0, lBytes.Length);
						}
						catch(Exception e)
						{
							Send("500 Local error, unable to process." + newline);
							Parent.DebugOut("Unable to access hard drive. \r\n\r\n A possible cause may be incorrectly set NTFS permissions. \r\n\r\n" + e.Message, true);
							break;
						}
						Send("354 Start mail input; end with <CRLF> . <CRLF>" + newline);
					}
					else
					{
						Send("503 Command is out of sequence, please make sure you have specified recipients." + newline);
					}
					break;
				
				#endregion

				#region Case HELP
				case "HELP":
					Send("214 Titanium Mail http://www.titaniumsoft.net" + newline);
					break;
				#endregion
					
				#region Case QUIT
				case "QUIT":
					Send("221 2.2.0 " + Parent.ServerName + " service closing transmission channel" + newline);
					myData.bStayConnected = false;
					break;
				#endregion

				#region Case NOOP
				case "NOOP":
					Send("OK" + newline);
					break;
				#endregion
			
				#region Case RSET
				case "RSET":
					Reset();
					Send("250 Command good - Continue:" + newline);
					break;
				#endregion

				#region Default
				default:
					if (myData.iState > 9)
					{
						HandleAuthorizationState(pData);
					}
					else
					{
						Send("503 Bad command or out of sequence." + newline);
					}
					break;
				#endregion
			}
		}
//-------------------------------------------------------------------------------------------
		private void HandleAuthorizationState(string strData)
		{
			switch (myData.iState)
			{
				case 11:
					myData.iState = 1;
					myData.AuthPass = Encoding.ASCII.GetString(Convert.FromBase64String(strData));
					if (Parent.CheckUserRelay(this, myData.AuthUser, myData.AuthPass))
					{
						myData.bForward = true;
						Send("235 Authenticated" + newline);
					}
					else
					{
						Send("504 Invalid username or password\r\n");
						myData.bStayConnected = false;
					}
					return;

				case 10:
					try
					{
						myData.AuthUser = Encoding.ASCII.GetString(Convert.FromBase64String(strData));
						myData.iState = 11;
						Send("334 UGFzc3dvcmQ6" + newline);
					}
					catch
					{
						Send("504 Invalid username" + newline);
					}		
					return;

				default:
					Send("221 Closing connection");
					myData.bStayConnected = false;
					return;
			}
		}
//-------------------------------------------------------------------------------------------
		private void SaveData(string strData)
		{
               // bug in this code.. should parse based on lines.. if the entire smtp sequence is sent at once the code misses it.. i.e. \r\n.\r\nQUIT\r\n will fail
               // Console.WriteLine(strData);
			myData.Temp = myData.Temp + strData;
			if (myData.Temp.Length > 5)
			{
				if (myData.Temp.Substring(myData.Temp.Length - 5, 5) == (newline + "." + newline))
				{
					try
					{
						FileFinish(myData.Temp.Substring(0, myData.Temp.Length - 5));
					}
					catch(Exception e)
					{
						Parent.DebugOut("Incoming file was not saved.\r\n\r\n" + e.Message, true);
					}
				}
				else
				{
					try
					{
						Byte[] sBytes = System.Text.Encoding.ASCII.GetBytes(myData.Temp.Substring(0, myData.Temp.Length - 5));
						myData.Temp = myData.Temp.Substring(myData.Temp.Length - 5);
						myData.IncomingFile.Write(sBytes, 0, sBytes.Length);
					}
					catch(Exception e)
					{
						Parent.DebugOut("Incoming file from " + myData.MailFrom + " was not saved due to an error.\r\n\r\n" + e.Message, true);
					}
				}
			}
		}
//-------------------------------------------------------------------------------------------
		private void FileFinish(string strData)
		{
			System.Byte[] lBytes = System.Text.Encoding.ASCII.GetBytes(strData);
			myData.IncomingFile.Write(lBytes, 0, lBytes.Length);
			myData.IncomingFile.Close();
					
			strData				= "";

			ArrayList Forward	= new ArrayList();
			ArrayList SMS		= new ArrayList();

			decimal dirsize = 0;
               string filename = Path.GetFileName(myData.IncomingFile.Name);

			foreach (Address address in myData.RCPT)
			{
				try
				{
					if (!File.Exists(myData.IncomingFile.Name))
						break;

					switch (address.EndPoint)
					{
						case AddressEndPointStatus.Local:
                                   if (!Directory.Exists(address.Path + "Inbox" + Path.DirectorySeparatorChar))
								Directory.CreateDirectory(address.Path + "Inbox\\");

							dirsize = Common.DirSize(new DirectoryInfo(address.Path)) / 1024;

							if (dirsize < address.quota || address.quota == -1)
							{
                                        while (File.Exists(address.Path + "Inbox" + Path.DirectorySeparatorChar + filename))
								{
									filename = FileName();
								}
                                        File.Copy(myData.IncomingFile.Name, address.Path + "Inbox" + Path.DirectorySeparatorChar + FileName(), false);
								Parent.DebugOut("Message delivered from " + myData.MailFrom + " to \"" + address.Path + "Inbox\\\".", false);
							}
							else
							{
								Send("554 delivery error: Your mail to " + address.address + " was not delivered, this user's mail box has exceeded it's quota." + newline);
							}
							break;	
		
                              case AddressEndPointStatus.Invoke:
                                   Parent.ProcessHandleMessageReceived(this, myData.IncomingFile.Name);
                                   break;

						case AddressEndPointStatus.Plugin:
							try
							{
                                        if (File.Exists(address.Path))
                                             Process.Start(address.Path, myData.IncomingFile.Name);
								Parent.DebugOut("Plugin executed for " + address.address + ":\r\n" + address.Path, false);
							}
							catch (Exception e)
							{
								Parent.DebugOut("Unable to execute \"" + address.Path + "\".\r\n" + e.Message, true);
							}
							break;

						case AddressEndPointStatus.Forward:
							Parent.DebugOut("Mail from " + myData.MailFrom + " is being forwarded to: " + address.Path, false);
							Forward.Add("Recipient: " + address.Path + "|2|0");
							break;

						case AddressEndPointStatus.SMS:
							Forward.Add("Recipient: " + address.Path + "|6|0");				
							break;

						case AddressEndPointStatus.Remote:
							Parent.DebugOut("Mail from " + myData.MailFrom + " is being forwarded to remote host: " + address.Server + ":" + address.port.ToString(), false);
							Forward.Add("Recipient: " + address.Path + "|5|0");
							break;
					}
				}
				catch(Exception e)
				{
					Parent.DebugOut("An error occured while attempting to deliver e-mail. \r\n\r\n" + e.Message + "\r\n" + e.StackTrace, true);
				}
			}

			if (Forward.Count > 0 || SMS.Count > 0)
			{	
				while (File.Exists(Parent.SmtpDirectory + "out" + Path.DirectorySeparatorChar + filename))
				{
					filename = FileName();
				}
				filename = filename.Substring(0, filename.Length - 4);

                    if (!Directory.Exists(Parent.SmtpDirectory + "out" + Path.DirectorySeparatorChar))
                         Directory.CreateDirectory(Parent.SmtpDirectory + "out" + Path.DirectorySeparatorChar);

                    File.Copy(myData.IncomingFile.Name, Parent.SmtpDirectory + "out" + Path.DirectorySeparatorChar + filename + ".mai", false);

                    StreamWriter Receipt = File.AppendText(Parent.SmtpDirectory + "out" + Path.DirectorySeparatorChar + filename + ".smtp");
				Receipt.WriteLine(
								"// Do not modify the contents of this file.\r\n" +
								"// This is an automatically generated forwarding receipt.\r\n" +
								"//				Weavver - Weavver.Net.Mail\r\n" +
								"From: " + myData.MailFrom
								);

				foreach (string temp in Forward)
				{
                         Parent.DebugOut("Queuing e-mail from " + myData.MailFrom + " for remote delivery.", false);
					Receipt.WriteLine(temp);
				}
				Receipt.Close();

                    Parent.QueueMailOut(this, Parent.SmtpDirectory + "out" + Path.DirectorySeparatorChar + filename + ".smtp");
			}

               if (File.Exists(myData.IncomingFile.Name))
			     File.Delete(myData.IncomingFile.Name);

			myData.iState = 5;
			Send("250 2.6.0 Continue:" + newline);
		}
//-------------------------------------------------------------------------------------------
		/// <summary>
		/// This method may be used to send custom commands to the client. Unrecognized commands will be passed back through the unknown event.
		/// </summary>
		/// <param name="data">This should contain the custom command, depending on the command it may be necessary to include a carriage return/new line at the end of your string (CRLF).</param>
		private void Send(string data)
		{
			try
			{
				Byte[] bSend = System.Text.Encoding.ASCII.GetBytes(data);
				client.Send(bSend, 0, bSend.Length, SocketFlags.None);
			}
			catch
			{
				// Errors may occur here whenever a client does not shutdown the connection properly.
				// These can be ignored. The connection will be automatically handled properly.
			}
		}
//-------------------------------------------------------------------------------------------
		/// <summary>
		/// This method returns a file name determined by a date time stamp followed by three random characters.
		/// Using this method is useful for generating message ids that are random and have a minimal chance of
		/// collision with other files (simultaneously being received).
		/// </summary>
		private string FileName()
		{			
			string format = "{0}.mai";
			return String.Format(format, Guid.NewGuid().ToString());
		}
//-------------------------------------------------------------------------------------------
	}
}
