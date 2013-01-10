using System;
using System.Configuration;
using System.Threading;
using System.Web;
using System.Web.SessionState;

namespace Weavver.Web
{
	public class HttpStreamHandler : IHttpAsyncHandler
	{
		private HttpContext _context;
//--------------------------------------------------------------------------------------------
		public void ProcessRequest(HttpContext context) {}
//--------------------------------------------------------------------------------------------
		public bool IsReusable
		{
			get { return false; } // need to research this
		}
//--------------------------------------------------------------------------------------------
		public IAsyncResult BeginProcessRequest(HttpContext context, System.AsyncCallback cb, object extraData)
		{
               _context = context;

               context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
               context.Response.Write("bufferfillerbufferfillerbufferfillerbufferfillerbufferfillerbufferfillerbufferfillerbufferfillerbufferfillerbufferfillerbufferfillerbufferfillerbufferfillerbufferfillerbufferfillerbufferfillerbufferfillerbufferfillerbufferfillerbufferfillerbuffer</message>");
               context.Response.Flush();

               // object data = context.Request["position"]; example for getting the query string
               HttpStreamThreadResult result = new HttpStreamThreadResult(context, cb, null);
               HttpStreamThread request = new HttpStreamThread(result);
               ThreadStart start = new ThreadStart(request.Process);
               Thread workerThread = new Thread(start);
               workerThread.Start();

			// Return the AsynResult to ASP.NET
			return result;
		}
//--------------------------------------------------------------------------------------------
		public void EndProcessRequest(IAsyncResult result)
		{
               HttpStreamThreadResult requestResult = result as HttpStreamThreadResult;
               _context.Response.Write("Server connection closed.</message>");
               _context.Response.Flush();
               // do any other clean up work
		}
//--------------------------------------------------------------------------------------------
     }
}