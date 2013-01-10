using System;
using System.IO;
using System.Collections;

namespace Weavver.Net.Mail
{
//-------------------------------------------------------------------------------------------
     public struct _myData
     {
          public Service pService;

		public bool bForward;
		public bool bStayConnected;
		
		public int iState;

		public ArrayList RCPT;
		public FileStream IncomingFile;

		public string myEndPoint;
		public string Temp;

		public string AuthUser;
		public string AuthPass;

		public string MailFrom;
	}
//-------------------------------------------------------------------------------------------
	public enum Status: int
	{
		Blocked			= 0,
		Normal			= 1,
		Forward			= 2,
		Execute			= 3,
		Remote			= 5,
		SMS				= 6
	}
//-------------------------------------------------------------------------------------------
}
