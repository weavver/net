using System;
using System.Configuration;
using System.Web;
using System.Threading;

namespace Weavver.Web
{
	public class HttpStreamThreadResult : IAsyncResult
	{
		private HttpContext context;
		private AsyncCallback callback;

		private ManualResetEvent completeEvent = null;
		private object data;
		private object objLock = new object();
		private bool isComplete = false;
		
		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="ctx">Current Http Context</param>
		/// <param name="cb">Callback used by ASP.NET</param>
		/// <param name="d">Data set by the calling thread</param>
          public HttpStreamThreadResult(HttpContext ctx, AsyncCallback cb, object d)
		{
			this.context = ctx;
			this.callback = cb;
			this.data = d;
		}

		/// <summary>
		/// Gets the current HttpContext associated with the request
		/// </summary>
		public HttpContext Context
		{
			get { return this.context; }
		}

		/// <summary>
		/// Completes the request and tells the ASP.NET pipeline that the 
		/// execution is complete.
		/// </summary>
		public void Complete()
		{
			isComplete = true;

			// Complete any manually registered events
			lock(objLock)
			{
				if(completeEvent != null)
				{
					completeEvent.Set();
				}
			}

			// Call any registered callback handers
			// (ASP.NET pipeline)
			if (callback != null)
			{
				callback(this);
			}
		}

		/// <summary>
		/// Performs basic clean up for the call
		/// </summary>
		public void CleanUp()
		{
			// Dispose of the web service and resources
               //if (weatherService != null)
               //{
               //     try
               //     {
               //          weatherService.Dispose();
               //     }
               //     finally {}
               //}
		}

		/// <summary>
		/// Gets the current weather information based on a US Zip code
		/// </summary>
		/// <param name="zipCode">City zip code</param>
		/// <returns>Location name along with current temperature.  Empty string otherwise.</returns>
		public string GetWeather(int zipCode)
		{
			string message = "";

               //try
               //{
               //     // Call the web service
               //     ExtendedWeatherInfo info = weatherService.GetExtendedWeatherInfo(zipCode);

               //     // Format the message
               //     message = string.Format("<h2>{0} - {1}</h2>Current Temperature: {2}a<br>Feels Like: {3}.", 
               //          zipCode.ToString(), info.Info.Location, info.Info.Temprature, info.Info.FeelsLike);
               //}
               //catch
               //{
               //     message = "An error was encountered while calling web service.";
               //}

			return "scooby snacks";
		}

		#region IAsyncResult Members

		/// <summary>
		/// Gets the object on which one could perform a lock
		/// </summary>
		public object AsyncState
		{
			get { return this.data; }
		}

		/// <summary>
		/// Always returns false
		/// </summary>
		public bool CompletedSynchronously
		{
			get { return false; }
		}

		/// <summary>
		/// Gets a handle that a monitor could lock on.
		/// </summary>
		public WaitHandle AsyncWaitHandle
		{
			get
			{
				lock(objLock)
				{
					if (completeEvent == null)
						completeEvent = new ManualResetEvent(false);

					return completeEvent;
				}
			}
		}

		/// <summary>
		/// Gets the current status of the request
		/// </summary>
		public bool IsCompleted
		{
			get { return this.isComplete; }
		}

		#endregion
	}
}