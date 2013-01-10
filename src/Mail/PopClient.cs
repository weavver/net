using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections;

namespace Weavver.Net.Mail.POP3
{
	public class PopClient
	{
		public	string		RemoteEndPoint	= "unknown";
		public	string		User			= null;
		private Socket		pSock;
		private string		mailboxpath		= "";
		private int			iAuth			= 1;
		public	bool		bStayConnected	= true;
		private string		temp			= "";
		private ArrayList	messages		= new	ArrayList();
		private PopListener	parent			= new PopListener();
//-------------------------------------------------------------------------------------------
		public PopClient(Socket _socket, PopListener poplistener)
		{
			pSock			= _socket;
			Parent			= poplistener;

			RemoteEndPoint	= pSock.RemoteEndPoint.ToString();
		}
//-------------------------------------------------------------------------------------------
		public string MailBoxPath
		{
			get
			{
				return mailboxpath;
			}
			set
			{
				mailboxpath = value;
			}
		}
//-------------------------------------------------------------------------------------------
		public PopListener		Parent
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
		public void HandleConnection()
		{
			try
			{
				Send("+OK Weavver, Inc. - Weavver.Net.Mail POP3 Service" + Environment.NewLine);
				
				string sREP = pSock.RemoteEndPoint.ToString();

				int ret;
				decimal dTimeOut = DateTime.Now.ToFileTime();

				string pData	= "";
				string strData	= "";
				
				byte[] RecvBytes;

				while(pSock.Connected && bStayConnected)
				{
					if (pSock.Available > 0)
					{
						RecvBytes = new byte[pSock.Available];
						ret = pSock.Receive(RecvBytes, 0, RecvBytes.Length, SocketFlags.None);
						strData = strData + Encoding.ASCII.GetString(RecvBytes).Substring(0, ret);
						while (strData.IndexOf(Environment.NewLine) > 0 && iAuth != 5)
						{
							pData = Regex.Split(strData, Environment.NewLine)[0].ToString();
							switch(iAuth)
							{
								case 1:
									handleUser(pData);
									break;
								case 2:
									handlePass(pData);
									break;
								case 3:
									handleTrans(pData);
									break;
							}
							strData = strData.Substring((strData.IndexOf(Environment.NewLine) + Environment.NewLine.Length));
							pData = null;
						}
						RecvBytes = null;
						
						dTimeOut = DateTime.Now.ToFileTime();
					}
					else
					{
						bStayConnected = ((dTimeOut + 3000000000 /* 15 minutes */) < DateTime.Now.ToFileTime() ? false : true);
						if (pSock.Available < 1)
						{
							System.Threading.Thread.Sleep(5);
						}
					}
				}

				#region Clean UP
				try
				{
					if (pSock.Connected)
						pSock.Close();
				}
				catch {}
				try
				{
					pSock = null;
				}
				catch {}
				#endregion
			}
			catch(Exception e)
			{
				Parent.DebugOut(this, "Error: " + e.Message, POPDebugType.Connection);
			}
		}
//-------------------------------------------------------------------------------------------
		private void handleUser(string strData)
		{
			#region Parse Int
			int iSub = 0;
			if (strData.Length > 4)
				iSub = 4;
			else
				iSub = strData.Length;
			#endregion
			switch (strData.Substring(0, iSub).ToUpper())
			{
				case "USER":
					temp = strData.Substring(4).Trim();
					User = temp;
					iAuth = 2;
					Send("+OK" + Environment.NewLine);
					break;
				
				case "QUIT":
					HandleQuit();
					break;
				
				default:
					Send("-ERR Invalid Command" + Environment.NewLine);
					break;
			}
		}
//-------------------------------------------------------------------------------------------
		private void handlePass(string data)
		{
			int iSub = (data.Length > 4) ? 4 : data.Length;
			switch (data.Substring(0, iSub).ToUpper())
			{
				case "PASS":
					MailBoxPath = Parent.AuthorizeUser(this, temp, data.Substring(4).Trim());
					if (MailBoxPath.ToLower().StartsWith("error"))
					{
						iAuth = 1;
						Send("-ERR Check your password" + Environment.NewLine);
					}
					else
					{
						iAuth = 3;
						if (!Directory.Exists(MailBoxPath))
							Directory.CreateDirectory(MailBoxPath);

						DirectoryInfo dInfo = new DirectoryInfo(MailBoxPath);
							
						foreach (FileInfo fi in dInfo.GetFiles())
						{
							if (fi.Name.EndsWith(".mai"))
								messages.Add(new object[] {fi.Length, fi.Name});
						}
						Send("+OK" + Environment.NewLine);
					}
					break;

				case "QUIT":
					HandleQuit();
					break;

				default:
					Send("+ERR Invalid Command" + Environment.NewLine);
					break;
			}
		}
//-------------------------------------------------------------------------------------------
		private void handleTrans(string strData)
		{
			#region Parse Int
			int iSub = 0;
			if (strData.Length > 4)
				iSub = 4;
			else
				iSub = strData.Length;
			#endregion

			switch (strData.ToUpper().Substring(0, iSub))
			{	
				#region Stat
				case "STAT":
					int iStat = 0;
					int iStatCount = 0;
					foreach (object[] oSize in messages)
					{
						if (oSize[0].ToString() != "delete")
							iStat += Int32.Parse(oSize[0].ToString());
						else
							iStatCount = iStatCount + 1;
					}
					Send("+OK " + (messages.Count - iStatCount) + " " + iStat.ToString() + Environment.NewLine);
					iStat = 0;
					break;
				#endregion

				#region List
				case "LIST":
					int iList = 0;
					int iListCount = 0;
					foreach (object[] oList in messages)
					{
						if (oList[0].ToString() != "delete")
							iList += Int32.Parse(oList[0].ToString());
						else
							iListCount = iListCount + 1;
					}

					Send("+OK " + (messages.Count - iListCount) + " " + iList.ToString() + Environment.NewLine);

					object[] oTemp;
					for (iList = 0; iList < messages.Count; iList++)
					{
						oTemp = (object[])messages[iList];
						iListCount = iList + 1;
						if (oTemp[0].ToString() != "delete")
							Send(iListCount.ToString() + " " + oTemp[0].ToString() + Environment.NewLine);

						oTemp = null;
					}
					iList = 0;
					iListCount = 0;
					Send("." + Environment.NewLine);
					break;					

				#endregion

				#region Retr

				case "RETR":
					try
					{
						object[] oRetr	= (object[]) messages[Int32.Parse(strData.Substring(4).Trim()) - 1];
						Send("+OK " + oRetr[0].ToString() + " octets" + Environment.NewLine);
						
						StreamReader file = File.OpenText(MailBoxPath + oRetr[1].ToString());
						while (file.Peek() > -1 && pSock.Connected == true) 
						{	
							Send(file.ReadLine() + Environment.NewLine);
						}
						file.Close();
						
						oRetr			= null;
						file			= null;
						if (pSock.Connected == false)
							break;

						Send(Environment.NewLine + "." + Environment.NewLine);

					}
					catch
					{
						Send("-ERR The message does not exist." + Environment.NewLine);
					}
					break;

				#endregion

				#region Uidl
				case "UIDL":
					try
					{
						object[] oUidl	= (object[])messages[Int32.Parse(strData.Substring(4).Trim()) - 1];
						if (oUidl[0].ToString() == "delete")
						{
							Send("-ERR No such message");
							break;
						}
						else
						{
							Send("+OK " + strData.Substring(4).Trim() + " " + oUidl[1].ToString() + Environment.NewLine);
							oUidl		= null;
						}
					}
					catch(Exception e)
					{
						if (strData.Trim().Length <= 5)
						{
							
							Send("+OK following is a unique id for each e-mail" + Environment.NewLine);
							object[] uTemp;
							for (iList = 0; iList < messages.Count; iList++)
							{
								uTemp = (object[])messages[iList];
								iListCount = iList + 1;
								if (uTemp[0].ToString() != "delete")
									Send(iListCount.ToString() + " " + uTemp[1].ToString() + Environment.NewLine);

								oTemp = null;
							}
							Send("." + Environment.NewLine);
						}
						else
						{
							Parent.DebugOut(this, e.Message, POPDebugType.Critical);
							Send("-ERR no such message" + Environment.NewLine);
						}
					}

					break;

				#endregion

				#region Dele

				case "DELE":
					try
					{
						int iDel = Int32.Parse(Regex.Split(strData, " ")[1].ToString()) - 1;
						object[] oDele = (object[])messages[iDel];
						oDele[0] = "delete";
						messages[iDel] = oDele; //new object[] {"delete", oDele[1]};
						Send("+OK Message Deleted" + Environment.NewLine);
						oDele = null;
					}
					catch
					{
						Send("-ERR Invalid message" + Environment.NewLine);
					}
					break;

				#endregion

				#region Top
				case "TOP ":
					try
					{
						int iTop		= Int32.Parse(Regex.Split(strData, " ")[1].ToString()) - 1;
						object[] oTop	= (object[])messages[iTop];
						string line		= "";

						if (oTop[0].ToString() == "delete")
						{
							Send("+ERR No such message");
							break;
						}
						StreamReader fTop = File.OpenText(MailBoxPath + oTop[1].ToString());
						while (fTop.Peek() > -1) 
						{	
							line = fTop.ReadLine();
							if (line == "")
								break;
							Send(line + Environment.NewLine);
						}

						fTop.Close();
						fTop = null;

						Send(Environment.NewLine + "." + Environment.NewLine);
					}
					catch(Exception e)
					{
                              Parent.DebugOut(this, e.Message, POPDebugType.Error);
						Send("-ERR No such message" + Environment.NewLine);
					}
					break;

				#endregion
					
				#region NOOP
				case "NOOP":
					Send("+OK" + Environment.NewLine);
					break;
					#endregion
					
				#region Quit
				case "QUIT":
					HandleQuit();
					break;
				#endregion

				#region Default
				default:
					Send("-ERR Unknown command" + Environment.NewLine);
					break;
				#endregion
			}
		}
//-------------------------------------------------------------------------------------------
		private void HandleQuit()
		{
			try
			{
				Send("+OK " + Parent.ServerName + " service closing transmission channel" + Environment.NewLine);
				pSock.Close();
			}
			catch{}
			foreach (object[] oList in messages)
			{
				if (oList[0].ToString() == "delete")
				{
					try
					{
						File.Delete(MailBoxPath + oList[1].ToString());
					}
					catch(Exception e)
					{
						Parent.DebugOut(this, "Error: " + e.Message, POPDebugType.Disconnection);								
					}
				}
			}
			messages.Clear();
		}
//-------------------------------------------------------------------------------------------
		private void Send(string strSend)
		{
			try
			{
				Byte[] bSend = System.Text.Encoding.ASCII.GetBytes(strSend);
				pSock.Send(bSend, 0, bSend.Length, SocketFlags.None);
			}
			catch
			{
                    Parent.DebugOut(this, "Error: Unable to send data", POPDebugType.Error);
			}
		}  
//-------------------------------------------------------------------------------------------
	}
}
