using System;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.IO;

namespace Weavver.Net.Mail
{
	public class Common
	{
//-------------------------------------------------------------------------------------------
		/// <summary>
		/// This method was retrieved from the msdn library.
		/// </summary>
		/// <param name="d"></param>
		/// <returns></returns>
		public static decimal DirSize(DirectoryInfo d) 
		{    
			decimal Size = 0;    
			
			FileInfo[] fis = d.GetFiles();
			foreach (FileInfo fi in fis) 
			{      
				Size += fi.Length;    
			}
			
			DirectoryInfo[] dis = d.GetDirectories();
			foreach (DirectoryInfo di in dis) 
			{
				Size += DirSize(di);   
			}
			return (Size);  
		}
//-------------------------------------------------------------------------------------------
		public static string formatString(string dirtystring)
		{
			return dirtystring.Trim().ToLower();
		}
//-------------------------------------------------------------------------------------------
		public static string QueryFormat(string dirtystring)
		{
               dirtystring = dirtystring.Replace("\\", "\\\\");
			return dirtystring.Replace("'", "''");
		}
//-------------------------------------------------------------------------------------------
		public static bool IsValidAddress(string address)
		{
               if (String.IsNullOrEmpty(address))
                    return false;
               if (address.IndexOf("@") <= 0 || address.IndexOf(".") <= 0)
				return false;
			else
				return true;
		}
//-------------------------------------------------------------------------------------------
          //public static string MakeComplete(string modify)
          //{
          //     modify = FixSlashes(modify);
          //     if (Weavver.Common.Common.Windows)
          //     {
          //          if (!modify.EndsWith("\\"))
          //          {
          //               return modify + "\\";
          //          }
          //          else
          //          {
          //               return modify;
          //          }
          //     }
          //     else
          //     {
          //          if (!modify.EndsWith("/"))
          //          {
          //               return modify + "/";
          //          }
          //          else
          //          {
          //               return modify;
          //          }
          //     }
          //}
//-------------------------------------------------------------------------------------------
          //public static string FixSlashes(string data)
          //{
          //     if (Weavver.Common.Common.Windows)
          //     {
          //          return data.Replace("/", "\\");
          //     }
          //     else
          //     {
          //          return data.Replace("\\", "/").ToLower();
          //     }
          //}
//-------------------------------------------------------------------------------------------
          //public static string ReturnMXRecords(string host)
          //{
          //     ProcessStartInfo info		= new ProcessStartInfo();
          //     info.UseShellExecute		= false;
          //     info.RedirectStandardInput	= true;
          //     info.RedirectStandardOutput	= true;
          //     info.FileName				= "nslookup";
          //     info.Arguments				= "-type=MX " + host;
          //     info.RedirectStandardError	= true;

          //     Process		ns			= Process.Start(info);
          //     StreamReader	sout		     = ns.StandardOutput;
          //     string         response	     = "";

          //     NameValueCollection nv		= new NameValueCollection();
          //     while (sout.Peek() >- 1) 
          //     {
          //          response			= sout.ReadLine();
          //          response			= response.ToLower();
          //          string mxpreference	= "";
          //          string mxhost			= "";
          //          if (Weavver.Common.Common.Windows && response.IndexOf("mx preference") > 1 && response.IndexOf("mail exchanger") > 1)
          //          {
          //               mxpreference		= response.Substring(response.IndexOf("preference") + 13);
          //               mxpreference		= mxpreference.Substring(0, mxpreference.IndexOf(","));

          //               mxhost			= response.Substring(response.IndexOf("exchanger") + 12);
          //               return mxhost;
          //          }
          //          else if (!Weavver.Common.Windows && response.IndexOf("mail exchanger") > -1)
          //          {
          //               response			= response.Substring(response.IndexOf("exchanger") + 11).Trim();
          //               mxpreference		= response.Substring(0, response.IndexOf(" "));
          //               mxhost			= response.Substring(response.IndexOf(" ") + 1);
          //               return mxhost;
          //          }
          //     }
          //     return "";
          //}
//-------------------------------------------------------------------------------------------
          //public static string ReturnARecords(string host)
          //{
          //     ProcessStartInfo info		= new ProcessStartInfo();
          //     info.UseShellExecute		= false;
          //     info.RedirectStandardInput	= true;
          //     info.RedirectStandardOutput	= true;
          //     info.FileName				= "nslookup";
          //     info.Arguments				= "-type=A " + host;
          //     info.RedirectStandardError	= true;

          //     Process			ns		= Process.Start(info);
          //     StreamReader   	sout		= ns.StandardOutput;
          //     string response			= "";
          //     string previousline           = "";
          //     NameValueCollection nv		= new NameValueCollection();
          //     while (sout.Peek() >- 1) 
          //     {
          //          response			= sout.ReadLine();
          //          response			= response.ToLower();

          //          if (Weavver.Common.Common.Windows && previousline.IndexOf(host.Substring(0, host.Length - 1)) > -1 && response.IndexOf("address") > -1)
          //          {
          //               return response.Substring(response.IndexOf(":") + 2).Trim();
          //          }
          //          previousline        = response;
          //     }
          //     return "";
          //}
//-------------------------------------------------------------------------------------------
          //public static bool DirectoryExists(string path)
          //{
          //     return Directory.Exists(Common.FixSlashes(path));
          //}
//-------------------------------------------------------------------------------------------
          //public static bool FileExists(string path)
          //{
          //     return File.Exists(Common.FixSlashes(path));
          //}
//-------------------------------------------------------------------------------------------
          //public static void CreateDirectory(string path)
          //{
          //     path = Common.FixSlashes(path);
          //     Directory.CreateDirectory(path);
          //}
//-------------------------------------------------------------------------------------------
	}
}
