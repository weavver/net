using System;
using System.Web;
using System.Threading;

namespace Weavver.Web
{
     public class HttpStreamThread : IAsyncResult
     {
          private HttpStreamThreadResult result;
          private bool _blnIsCompleted = false;
          private Mutex myMutex = null;
          private Object myAsynchStateObject = null;
          private bool _blnCompletedSynchronously = false;
//--------------------------------------------------------------------------------------------
          public HttpStreamThread(HttpStreamThreadResult ar)
          {
               this.result = ar;
          }
//--------------------------------------------------------------------------------------------
          public void Process()
          {
               try
               {
                    while (result.Context.Response.IsClientConnected)
                    {
                         System.Threading.Thread.Sleep(1000);
                         result.Context.Response.Write(DateTime.Now.ToString() + "</message>");
                         result.Context.Response.Flush();
                    }
               }
               finally
               {
                    // Tell ASP.NET that the request is complete
                    result.Complete();
               }
          }
//--------------------------------------------------------------------------------------------
          public bool IsCompleted
          {
               get { return _blnIsCompleted; }
          }
//--------------------------------------------------------------------------------------------
          public WaitHandle AsyncWaitHandle
          {
               get { return myMutex; }
          }
//--------------------------------------------------------------------------------------------
          public Object AsyncState
          {  
               get { return myAsynchStateObject; }
          }
//--------------------------------------------------------------------------------------------
          public bool CompletedSynchronously
          {
               get { return _blnCompletedSynchronously; }
          }
//--------------------------------------------------------------------------------------------
     }
}