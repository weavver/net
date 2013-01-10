using System;
using System.Data;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Security.Cryptography;
using System.IO;
using System.Diagnostics;
using System.ServiceProcess;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using System.Xml;

using Weavver.Net.Mail;

namespace Weavver.Net.Mail
{
     public class Service : System.ServiceProcess.ServiceBase
     {
          public static string sqladdressa = "select mb.mailboxid, a.prefix, a.status, a.misc, mb.quota, a.smsserviceid from addresses a, mailboxes mb, domains d where a.mailboxid = mb.mailboxid and a.domainid = d.domainid and (a.prefix = '*' or a.prefix = '{0}' or a.prefix = '{2}') and domain = '{1}' order by a.prefix desc";
          public static string sqladdressb = "select po.postoffice, mb.mailbox, mb.quota from postoffices po, mailboxes mb where po.postofficeid = mb.postofficeid and mb.mailboxid = {0}";
          public static string sqlchecka = "select mb.mailboxid from postoffices po, mailboxes mb where po.postofficeid = mb.postofficeid and po.postoffice = '{0}' and mb.mailbox = '{1}' and mb.password = '{2}'";
          public static string sqlcheckb = "select mb.mailboxid from postoffices po, mailboxes mb where po.postofficeid = mb.postofficeid and po.default = 1 and mb.mailbox = '{0}' and mb.password = '{1}'";
          public static string sqldomaincheck = "select status, misc from domains where domain = '{0}'";
          public static string sqlipaddresscheck = "select ipaddress, status, misc from relayrules where ipaddress = '{0}' or ipaddress = '{1}'";
          public static string sqlnamevalue = "select name, value from settings";
          public static string sqlsms = "select type, misc from smsservices where smsserviceid = {0}";
          public string AssemblyPath = "";
          public string RootPath = "C:\\Program Files\\Weavver\\Mail\\";
          private Container components = null;
          public ArrayList alForward_List = new ArrayList();
          private ADODB.Connection db = new ADODB.ConnectionClass();
          private SmtpListener server = new SmtpListener();
          private WaitCallback _Listen = null;
          private WaitCallback _ProcessOutgoingMail = null;
          public string vServer_Name = "WeavverLib";
          //--------------------------------------------------------------------------------------------
          /// <summary>
          /// This method loads the initial path values for the server. The initial database coonnection is initialized here as
          /// well and most importantly the server events are subscribed to.
          /// </summary>
          public Service()
          {
               components = new System.ComponentModel.Container();
               this.ServiceName = "Weavver.Net.Mail.SmtpService";

               this.RootPath = System.Reflection.Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName;
               this.AssemblyPath = this.RootPath.Substring(0, RootPath.LastIndexOf("\\") + 1);
               this.RootPath = this.RootPath.Substring(0, RootPath.LastIndexOf("\\"));
               this.RootPath = this.RootPath.Substring(0, RootPath.LastIndexOf("\\")) + "\\";
               this.server.MailPath = this.RootPath + "Mail\\";
               this.server.SmtpDirectory = this.RootPath + "Smtp\\";

               DBOpen();
               LoadVariables();

               server.AuthorizeIPRelay += new dAuthorizeIPRelay(server_AuthorizeIPRelay);
               server.AuthorizeMAIL += new dAuthorizeMAIL(server_AuthorizeMAIL);
               server.AuthorizeRCPT += new dAuthorizeRCPT(server_AuthorizeRCPT);
               server.AuthorizeUserRelay += new dAuthorizeUserRelay(server_AuthorizeUserRelay);
               server.DebugOutput += new dDebugOutput(server_DebugOutput);
               server.QueueMail += new dQueueMail(server_QueueMail);
          }
          //--------------------------------------------------------------------------------------------
          static void Main(string[] args)
          {
               if (!EventLog.Exists("Mail"))
               {
                    EventLog.CreateEventSource("Smtp", "Mail");
               }

               if (!EventLog.SourceExists("Smtp"))
               {
                    EventLog.CreateEventSource("Smtp", "Mail");
               }

               if (args.Length > 0)
               {
                    System.ServiceProcess.ServiceBase[] ServicesToRun;
                    ServicesToRun = new System.ServiceProcess.ServiceBase[] { new Service() };
                    System.ServiceProcess.ServiceBase.Run(ServicesToRun);
               }
               else
               {
                    string rootpath = System.Reflection.Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName;
                    rootpath = rootpath.Substring(0, rootpath.LastIndexOf("\\")) + "\\";

                    if (!File.Exists(rootpath + "ADODB.dll"))
                    {
                         Console.WriteLine("Could not find ADODB.dll, Automatically download it? (y/n)");
                         string answer = Console.ReadLine();
                         if (answer == "y")
                         {
                              WebClient wc = new WebClient();
                              wc.DownloadFile("http://mail.titaniumsoft.net/downloads/current-version/ADODB.dll", rootpath + "ADODB.dll");
                              Console.WriteLine("Downloaded ADODB.dll...");
                         }
                    }

                    if (!File.Exists(rootpath + "DNSLookup.dll"))
                    {
                         Console.WriteLine("Could not find DNSLookup.dll, Automatically download it? (y/n)");
                         string answer = Console.ReadLine();
                         if (answer == "y")
                         {
                              WebClient wc = new WebClient();
                              wc.DownloadFile("http://mail.titaniumsoft.net/downloads/current-version/DNSLookup.dll", rootpath + "DNSLookup.dll");
                              Console.WriteLine("Downloaded DNSLookup.dll...");
                         }
                    }

                    if (!File.Exists(rootpath + "WeavverLib.dll"))
                    {
                         Console.WriteLine("Could not find WeavverLib.dll, Automatically download it? (y/n)");
                         string answer = Console.ReadLine();
                         if (answer == "y")
                         {
                              WebClient wc = new WebClient();
                              wc.DownloadFile("http://mail.titaniumsoft.net/downloads/current-version/WeavverLib.dll", rootpath + "WeavverLib.dll");
                              Console.WriteLine("Downloaded Send.dll...");
                         }
                    }

                    Service pService = new Service();
                    pService.LoadSQL();
                    pService.OnStart(args);

                    bool bLoop = true;
                    while (bLoop)
                    {
                         switch (Console.ReadLine())
                         {
                              case "exit":
                              case "quit":
                              case "q":
                                   bLoop = false;
                                   break;

                              case "list":
                                   Console.WriteLine("Connections: " + pService.server.connections.Count.ToString());
                                   for (int i = 0; i < pService.server.connections.Count; i++)
                                   {
                                        SmtpClient client = (SmtpClient)pService.server.connections[i];
                                        Console.WriteLine(client.myData.myEndPoint + " - " + ((client.myData.MailFrom != null) ? client.myData.MailFrom : "from not known yet"));
                                   }
                                   break;

                              case "vars":
                              case "variables":
                                   Console.WriteLine("Server Name:\t" + pService.server.MailPath);
                                   Console.WriteLine("Assembly Path:\t" + pService.server.MailPath);
                                   Console.WriteLine("Mail Path:\t" + pService.server.MailPath);
                                   Console.WriteLine("Root Path:\t" + pService.RootPath);
                                   Console.WriteLine("SMTP Directory:\t" + pService.server.SmtpDirectory);
                                   break;
                         }
                         Thread.Sleep(1);
                    }
                    Console.WriteLine("SHUTTING DOWN"); //not usually triggered because it catches on readline
               }

          }
          //--------------------------------------------------------------------------------------------
          /// <summary>
          /// This method is called by the application once the service "OnStart" signal has been sent. This method will
          /// trigger the ThreadPool to start the server in a new thread and then it will load any unsent mail and process it.
          /// </summary>
          /// <param name="args"></param>
          protected override void OnStart(string[] args)
          {
               #region Set Callback
               _Listen = new WaitCallback(Listen);
               _ProcessOutgoingMail = new WaitCallback(ProcessOutgoingMail);
               #endregion

               ThreadPool.QueueUserWorkItem(_Listen);
               PickupMail();
          }
          //--------------------------------------------------------------------------------------------
          /// <summary>
          /// A seperate thread is spun off and this method is called to allow the server to run
          /// along side this application without locking the main thread because the Listen method
          /// in the SmtpListener class requires it's own thread.
          /// </summary>
          /// <param name="unused">Using the thread pool allows one to pass an object, however it is not required
          /// thus it is "unused."</param>
          private void Listen(object unused)
          {
               IPEndPoint ipendpoint = new IPEndPoint(IPAddress.Any, 25);
               server.Listen(ipendpoint);
          }
          //--------------------------------------------------------------------------------------------
          /// <summary>
          /// This method will load the variables from the database. Default values are already set and will
          /// be used if they can not be loaded from the database.
          /// </summary>
          private void LoadVariables()
          {
               try
               {
                    //To be implemented in future versions.
                    //if (!File.Exists(vAssemblyPath + "mail.config.path"))
                    //	this.SaveDefaultConfigSchema();

                    //				LoadConfig();

                    ADODB.Recordset rs = Query(sqlnamevalue);
                    while (!rs.EOF)
                    {
                         switch (rs.Fields[0].Value.ToString().ToLower().Trim())
                         {
                              case "server_name":
                                   server.ServerName = rs.Fields[1].Value.ToString().Trim();
                                   break;

                              case "mail_path":
                                   server.MailPath = rs.Fields[1].Value.ToString().Trim();
                                   break;

                              case "smtp_path":
                                   server.SmtpDirectory = rs.Fields[1].Value.ToString().Trim();
                                   break;
                         }
                         rs.MoveNext();
                    }
               }
               catch (Exception e)
               {
                    DebugOut("Unable to load settings from database.\r\n   " + e.Message, true);
               }
          }
          //--------------------------------------------------------------------------------------------
          /// <summary>
          /// This method can be used to automatically generate the smtp.xml file.
          /// </summary>
          private void SaveDefaultConfigSchema()
          {
               DataSet ds = new DataSet("configuration");

               ds.Tables.Add("general");
               ds.Tables["general"].Columns.Add("name");
               ds.Tables["general"].Columns.Add("value");

               ds.Tables["general"].Rows.Add(new object[] { "connection string", "Titanium Mail" });

               ds.Tables.Add("sql");
               ds.Tables["sql"].Columns.Add("type");
               ds.Tables["sql"].Columns.Add("name");
               ds.Tables["sql"].Columns.Add("sql");

               ds.Tables["sql"].Rows.Add(new object[] { "smtp", "addressa", sqladdressa });
               ds.Tables["sql"].Rows.Add(new object[] { "smtp", "addressb", sqladdressb });
               ds.Tables["sql"].Rows.Add(new object[] { "smtp", "checkrelaya", sqlchecka });
               ds.Tables["sql"].Rows.Add(new object[] { "smtp", "checkrelayb", sqlcheckb });
               ds.Tables["sql"].Rows.Add(new object[] { "smtp", "domaincheck", sqldomaincheck });
               ds.Tables["sql"].Rows.Add(new object[] { "smtp", "ipaddresscheck", sqlipaddresscheck });
               ds.Tables["sql"].Rows.Add(new object[] { "smtp", "namevalue", sqlnamevalue });
               ds.Tables["sql"].Rows.Add(new object[] { "smtp", "sms", sqlsms });


               ds.WriteXml(this.AssemblyPath + "mail.config.xml", XmlWriteMode.WriteSchema);
          }
          //--------------------------------------------------------------------------------------------
          public bool CheckAccount(string username, string password)
          {
               return false;
          }
          //--------------------------------------------------------------------------------------------
          public ArrayList CheckUser(AddressEndPoint aep, bool checkrelay)
          {
               return new ArrayList();
          }
          //--------------------------------------------------------------------------------------------
          public bool CheckRelay(string address)
          {
               return false;
          }
          //--------------------------------------------------------------------------------------------
          /// <summary>
          /// This method is called by the service manager and will allow the server to gracefully
          /// stop listening as well as warn any clients as to why they are being disconnected.
          /// </summary>
          protected override void OnStop()
          {
               //server.KillClients();
               server.StopListening();
               try
               {
                    db.Close();
               }
               catch { }
          }
          //--------------------------------------------------------------------------------------------
          /// <summary>
          /// This method will load receipts for any mail waiting to be sent.
          /// </summary>
          private void PickupMail()
          {
               if (!Directory.Exists(server.SmtpDirectory + "Outgoing\\"))
               {
                    Directory.CreateDirectory(server.SmtpDirectory + "Outgoing\\");
               }

               string[] file = Directory.GetFiles(server.SmtpDirectory + "Outgoing\\");
               foreach (string sFile in file)
               {
                    if (sFile.EndsWith(".smtp"))
                    {
                         server_QueueMail(null, sFile);
                    }
               }
          }
          //--------------------------------------------------------------------------------------------
          /// <summary>
          /// This method is called by the thread pool when a message needs to be re-routed outside of the
          /// server (i.e. to another server).
          /// </summary>
          /// <param name="path">The object passed in should be a string that is the valid path to the message
          /// receipt.</param>
          private void ProcessOutgoingMail(object path)
          {
               try
               {
                    //SendMail sendmail = new SendMail();

                    //string receipt = (string) path;
                    //sendmail.LoadRecipients(receipt, true, true);

                    //string mail		 = receipt.Substring(0, receipt.LastIndexOf("\\")) + "\\mail";
                    //mail			+= receipt.Substring(receipt.LastIndexOf("\\")).Replace(".smtp", ".mai");

                    //sendmail.DomainFlag = server.ServerName;
                    //sendmail.UseMailPath(mail, true);

                    //sendmail.Send(3, true);
                    //sendmail = null;
               }
               catch
               {
                    DebugOut("error sending mail", true);
               }
          }
          //--------------------------------------------------------------------------------------------
          /// <summary>
          /// This method is used to ensure database connectivity. On occasion the connection may be lost due to networking
          /// or other unknown issues that can not be accounted for.
          /// </summary>
          /// <returns></returns>
          private bool DBOpen()
          {
               string ConnectionString = "Weavver.Net.Mail";
               if (db.State == 0)
               {
                    try
                    {
                         try
                         {
                              DebugOut("Attempting to load connection string from: \r\n    " + RootPath + "bin\\connectionstring.ini", false);

                              StreamReader sr;
                              if (File.Exists(AssemblyPath + "\\connectionstring.ini"))
                                   sr = File.OpenText(AssemblyPath + "\\connectionstring.ini");
                              else
                                   throw new Exception("Connection string not found because file does not exist.");

                              ConnectionString = sr.ReadToEnd();
                              sr.Close();

                              DebugOut("Loaded connection string:\r\n   " + ConnectionString, false);
                         }
                         catch
                         {
                              DebugOut("Unable to read database information, please check the connection string\r\n   and/or your database configuration.\r\n-Defaulting to ODBC Source 'Titanium Mail'", true);
                         }
                         db.Open(ConnectionString, "", "", -1);
                    }
                    catch (Exception e)
                    {
                         DebugOut("ERROR: Database unaccessible, please check your connection string and settings.\r\n   " + e.Message, true);
                         return false;
                    }
               }
               return true;
          }
          //--------------------------------------------------------------------------------------------
          /// <summary>
          /// This will load the required sql statements from a local external file.
          /// </summary>
          private void LoadSQL()
          {
               if (!File.Exists("smtp.sql.xml"))
                    return;

               DataSet ds = new DataSet("smtp.sql.xml");
               ds.ReadXml(AssemblyPath + "smtp.sql.xml");

               for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
               {
                    string name = formatString(ds.Tables[0].Rows[i][0].ToString());
                    string sql = formatString(ds.Tables[0].Rows[i][1].ToString());

                    switch (name.ToLower())
                    {
                         case "sql address a": sqladdressa = sql; break;
                         case "sql address b": sqladdressb = sql; break;
                         case "sql check a": sqlchecka = sql; break;
                         case "sql check b": sqlcheckb = sql; break;
                         case "sql domain check": sqldomaincheck = sql; break;
                         case "sql ip address check": sqlipaddresscheck = sql; break;
                         case "sql settings": sqlnamevalue = sql; break;
                         case "sql sms": sqlsms = sql; break;
                    }
                    // debug to be finished
                    Debug.WriteLine("Loaded sql from schema: " + name, SMTPDebugType.Initialization.ToString());
                    Debug.Indent();
                    Debug.WriteLine(sql, SMTPDebugType.Initialization.ToString());
                    Debug.Unindent();
               }
          }
          //--------------------------------------------------------------------------------------------
          /// <summary>
          /// This method will clean up any strings passed to it so that they do not contain any new lines or double spaces. This
          /// is most useful in optimizing the size of sql statements so that when they are passed to the server they will conserve
          /// bandwith and processing time.
          /// </summary>
          /// <param name="ufstring"></param>
          /// <returns></returns>
          private string formatString(string ufstring)
          {
               ufstring = ufstring.Replace("\r\n", " ");
               ufstring = ufstring.Replace("\t", " ");
               while (ufstring.IndexOf("  ") > 0)
               {
                    ufstring = ufstring.Replace("  ", " ");
               }
               return ufstring.Trim();
          }
          //--------------------------------------------------------------------------------------------
          /// <summary>
          /// This method is called by the "server" object to output messages to the screen and log errors to the system event log.
          /// </summary>
          /// <param name="message">This is the mesasge that will be output to the screen or event log.</param>
          /// <param name="error">If this is true the message will be logged to the system event log as well as the screen.</param>
          public void DebugOut(string message, bool error)
          {
               try
               {
                    Console.WriteLine(message);
                    if (error)
                         EventLog.WriteEntry("Smtp", message, (error ? EventLogEntryType.Error : EventLogEntryType.Information));
               }
               catch { }
          }
          //--------------------------------------------------------------------------------------------
          /// <summary>
          /// This method simplifies some of the redundant tasks involved with quering the database.
          /// </summary>
          /// <param name="sqlquery">This is the sql statement that will be queried against the database.</param>
          /// <returns>A recordset encapsulating the results of the query is returned.</returns>
          private ADODB.Recordset Query(string sqlquery)
          {
               try
               {
                    System.Diagnostics.Debug.WriteLine(sqlquery, "SQL Query");
                    object ob;
                    return db.Execute(sqlquery, out ob, -1);
               }
               catch
               {
                    // returns a new emtpy recordset class, returning null value is not recommened as it would require much more
                    // handling code
                    return new ADODB.RecordsetClass();
               }
          }
          //--------------------------------------------------------------------------------------------
          private bool server_AuthorizeIPRelay(SmtpClient smtpclient, EndPoint endpoint)
          {
               string IPAddress = endpoint.ToString();
               if (!DBOpen())
                    return false;

               ADODB.Recordset rs = null;
               IPAddress = IPAddress.Substring(0, IPAddress.IndexOf(":"));

               string sql = String.Format(sqlipaddresscheck, IPAddress, IPAddress.Substring(0, IPAddress.LastIndexOf(".") + 1) + "*");

               rs = Query(sql);
               if (rs.EOF)
                    return false;

               if (rs.Fields["status"].Value.ToString() == "1")
                    return true;
               else
                    return false;
          }
          //--------------------------------------------------------------------------------------------
          /// <summary>
          /// Currently I am not sure how this authorization should be implemented. If you have any ideas please
          /// let me know.
          /// </summary>
          private bool server_AuthorizeMAIL(SmtpClient smtpclient, string address)
          {
               return true;
          }
          //--------------------------------------------------------------------------------------------
          /// <summary>
          /// This is where 90% of the magic happens. Once the SmtpClient is issued a recipient's address this event is
          /// triggered and handles the address to return an address list which specifies how to route the incoming message.
          /// </summary>
          /// <param name="smtpclient">This is the client triggering the authorization event.</param>
          /// <param name="address">This is the address being verified.</param>
          /// <param name="AllowRelay">This states whether or not the client is allowed to relay. (i.e. if the address is not local, then
          /// an address should be added to the address list to route the message to the remote server.</param>
          /// <returns>This event should return an address list that contains routing instructions for the message.</returns>
          private ArrayList server_AuthorizeRCPT(SmtpClient smtpclient, Address mailaddress, bool AllowRelay)
          {
               string sql = "";
               ArrayList al = new ArrayList();

               ADODB.Recordset rs = null;
               if (!DBOpen())
               {
                    Address address = new Address();
                    address.SetAddress(mailaddress.address);
                    DebugOut("Database connection error, e-mail rejected.", true);
                    address.EndPoint = AddressEndPointStatus.Bad;
                    address.Error = "There is a problem on our side, we can not accept your mail due to a database connection error.";
                    al.Add(address);
                    return al;
               }

               try
               {
                    sql = String.Format(sqldomaincheck, mailaddress.domain);
                    rs = Query(sql);

                    if (rs.EOF && AllowRelay)
                    {
                         Address address = new Address();
                         address.SetAddress(mailaddress.address);
                         address.Path = address.address;
                         address.EndPoint = AddressEndPointStatus.Forward;
                         al.Add(address);
                         return al;
                    }
                    else if (rs.EOF && !AllowRelay)
                    {
                         Address address = new Address();
                         address.SetAddress(mailaddress.address);
                         address.Error = "This server is rejecting your mail because it is not an authority for the end address to which you are trying to send to and/or you are not authenticated for relay.";
                         address.EndPoint = AddressEndPointStatus.Bad;
                         al.Add(address);
                         return al;
                    }

                    while (!rs.EOF)
                    {
                         Address address = new Address();
                         address.SetAddress(mailaddress.address);
                         switch (Int32.Parse(rs.get_Collect("status").ToString()))
                         {
                              #region Case BLOCKED
                              case (int)Status.Blocked: //domain disabled
                                   address.EndPoint = AddressEndPointStatus.Bad;
                                   address.Error = "This server is not allowed to handle mail to this address and therefore can not accept your mail.";
                                   al.Add(address);
                                   return al;
                              #endregion

                              #region Case NORMAL
                              case (int)Status.Normal:	//normal operation
                                   address = null;
                                   // address.EndPoint	= AddressEndPoint.Local;
                                   break;
                              #endregion

                              #region Case REMOTE
                              case (int)AddressEndPointStatus.Remote: // forward mail to specified server
                                   address.EndPoint = AddressEndPointStatus.Remote;
                                   address.Server = rs.get_Collect("misc").ToString();
                                   al.Add(address);
                                   return al;
                              #endregion

                              #region Case FORWARD
                              case (int)AddressEndPointStatus.Forward:
                                   address.Path = rs.get_Collect("misc").ToString();
                                   address.EndPoint = AddressEndPointStatus.Forward;
                                   al.Add(address);
                                   return al;
                              #endregion

                              #region Case DEFAULT
                              default:
                                   address.Error = "Server is misconfigured for this domain and therefore can not accept any mail.";
                                   DebugOut("Domain status settings for " + address.domain + " are not set properly please check them again.", true);
                                   al.Add(address);
                                   return al;
                              #endregion
                         }
                         rs.MoveNext();
                    }

                    string disposableprefix = "<#IGNORE#>";
                    bool prefixfound = false;
                    bool disposablefound = false;

                    if (mailaddress.prefix.IndexOf("-") > 0 && mailaddress.prefix.Length > mailaddress.prefix.IndexOf("-"))
                         disposableprefix = mailaddress.prefix.Substring(0, mailaddress.prefix.IndexOf("-"));

                    sql = String.Format(sqladdressa, mailaddress.prefix, mailaddress.domain, disposableprefix);
                    rs = Query(sql);

                    if (rs.EOF)
                    {
                         Address address = new Address();
                         address.EndPoint = AddressEndPointStatus.Bad;
                         address.Error = "The address belongs to this server but no mail box for it exists. (" + mailaddress.address + ")";
                         al.Add(address);
                         return al;
                    }
                    while (!rs.EOF)
                    {
                         string prefix = rs.Fields["prefix"].Value.ToString().ToLower();
                         string astatus = rs.Fields["status"].Value.ToString();
                         int status = (int)rs.Fields["status"].Value;
                         if (prefix == mailaddress.prefix.ToLower())
                         {
                              prefixfound = true;
                         }

                         if (prefix == disposableprefix && status == 7)
                         {
                              disposablefound = true;
                         }
                         rs.MoveNext();
                    }
                    rs.MoveFirst();

                    int rowcount = 0;
                    while (!rs.EOF)
                    {
                         rowcount++;

                         string prefix = rs.Fields["prefix"].Value.ToString();
                         string misc = rs.Fields["misc"].Value.ToString();

                         if ((prefixfound || disposablefound) && prefix == "*")
                         {
                              rs.MoveNext();
                              continue;
                         }
                         else if (prefixfound && prefix == disposableprefix)
                         {
                              rs.MoveNext();
                              continue;
                         }

                         ADODB.Recordset rsb = null;

                         Address address = new Address();
                         address.SetAddress(mailaddress.address);

                         sql = "";
                         switch (Int32.Parse(rs.get_Collect("status").ToString()))
                         {
                              case (int)Status.Blocked:
                                   if (!(misc == "ignore" && prefix == disposableprefix))
                                   {
                                        address.EndPoint = AddressEndPointStatus.Blocked;
                                        address.Error = "This user's mail box has been disabled.";
                                        al.Add(address);
                                   }
                                   else
                                   {
                                        address = null;
                                   }
                                   break;

                              case (int)Status.Normal:
                                   sql = String.Format(sqladdressb, rs.get_Collect("mailboxid").ToString());
                                   rsb = Query(sql);
                                   address.EndPoint = AddressEndPointStatus.Local;
                                   address.quota = Decimal.Parse(rsb.Fields["quota"].Value.ToString());
                                   address.Path = server.MailPath + rsb.Fields["postoffice"].Value.ToString() + "\\" + rsb.Fields["mailbox"].Value.ToString() + "\\";
                                   al.Add(address);
                                   break;

                              case (int)Status.Forward:
                                   string fmisc = rs.Fields["misc"].Value.ToString().Replace(" ", "");
                                   address.EndPoint = AddressEndPointStatus.Forward;
                                   address.Path = fmisc;
                                   al.Add(address);
                                   break;

                              case (int)Status.Execute:
                                   sql = String.Format(sqladdressb, rs.get_Collect("mailboxid").ToString());
                                   rs = Query(sql);
                                   address.quota = Decimal.Parse(rs.get_Collect("quota").ToString());
                                   address.EndPoint = AddressEndPointStatus.Plugin;
                                   address.Path = server.MailPath + rs.Fields["postoffice"].Value.ToString() + "\\" + rs.Fields["mailbox"].Value + "\\";

                                   al.Add(address);
                                   break;

                              case (int)AddressEndPointStatus.AutoRespond:
                                   break;

                              case (int)AddressEndPointStatus.Disposable:
                                   //no action - just a pointer record
                                   break;

                              case (int)Status.Remote:
                                   string rmisc = rs.Fields["misc"].Value.ToString().Trim();

                                   address.EndPoint = AddressEndPointStatus.Remote;
                                   address.Path = rmisc;

                                   al.Add(address);
                                   break;

                              case (int)Status.SMS:
                                   sql = String.Format(sqlsms, rs.Fields["smsserviceid"].Value.ToString());
                                   rsb = Query(sql);
                                   if (!rsb.EOF)
                                   {
                                        switch (Int32.Parse(rsb.Fields["type"].Value.ToString()))
                                        {
                                             case 1:
                                                  address.EndPoint = AddressEndPointStatus.SMS;
                                                  address.Path = rs.Fields["misc"].Value.ToString() + "@" + rsb.Fields["misc"].Value.ToString();

                                                  al.Add(address);
                                                  break;

                                             case 2:
                                                  address.EndPoint = AddressEndPointStatus.Forward;
                                                  address.Path = rs.Fields["misc"].Value.ToString() + "@" + rsb.Fields["misc"].Value.ToString();

                                                  al.Add(address);
                                                  break;

                                             case 3:
                                                  address.EndPoint = AddressEndPointStatus.Plugin;
                                                  address.Path = rsb.Fields["misc"].Value.ToString();

                                                  al.Add(address);
                                                  break;
                                        }
                                   }
                                   break;

                              default:
                                   DebugOut("The status set for " + address.address + " is invalid. E-mail to this address was rejected, please fix it.", true);
                                   address.EndPoint = AddressEndPointStatus.Bad;
                                   address.Error = "The mailbox you are trying to reach does exist for this user however it is incorrectly configured and we are forced to reject your e-mail.";

                                   al.Add(address);
                                   break;
                         }
                         rs.MoveNext();
                    }

                    if (rowcount == 0)
                    {
                         Address address = new Address();
                         address.SetAddress(mailaddress.address);
                         address.Error = "This server is authoritive for the domain that you are trying to send an e-mail to, however the mailbox that you are trying to reach does not exist on this server!";
                         address.EndPoint = AddressEndPointStatus.Bad;
                         al.Add(address);
                    }
                    return al;
               }
               catch (Exception e)
               {
                    Address address = new Address();
                    address.SetAddress(mailaddress.address);
                    DebugOut("User check against database failed for " + mailaddress.address + ".\r\n\r\n" + e.Message, true);
                    address.Error = "This server can not accept your e-mail due to a local database error.";
                    address.EndPoint = AddressEndPointStatus.Bad;
                    al.Add(address);
                    return al;
               }
          }
//-------------------------------------------------------------------------------------------
          private bool server_AuthorizeUserRelay(SmtpClient smtpclient, string user, string pass)
          {
               MD5 md5 = new MD5CryptoServiceProvider();
               byte[] raw = System.Text.ASCIIEncoding.ASCII.GetBytes(pass);
               byte[] hash = md5.ComputeHash(raw);
               pass = BitConverter.ToString(hash).Replace("-", "").ToUpper();

               user = user.Replace("'", "\'");
               pass = pass.Replace("'", "\'");

               string sql = "";
               if (user.IndexOf("@") > 0)
               {
                    sql = String.Format(sqlchecka, user.Substring(user.IndexOf("@") + 1), user.Substring(0, user.IndexOf("@")), pass);
               }
               else
               {
                    sql = String.Format(sqlcheckb, user, pass);
               }

               ADODB.Recordset rs = Query(sql);

               return !rs.EOF;
          }
//-------------------------------------------------------------------------------------------
          private void server_DebugOutput(string message, bool error)
          {
               DebugOut(message, error);
          }
//-------------------------------------------------------------------------------------------
          private void server_QueueMail(SmtpClient smtpclient, string messagepath)
          {
               ThreadPool.QueueUserWorkItem(_ProcessOutgoingMail, messagepath);
          }
//--------------------------------------------------------------------------------------------
          protected override void Dispose(bool disposing)
          {
               OnStop();
               if (disposing)
               {
                    if (components != null)
                    {
                         components.Dispose();
                    }
               }
               base.Dispose(disposing);
          }
//-------------------------------------------------------------------------------------------
     }
}