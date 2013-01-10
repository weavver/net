using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Collections.Specialized;
using System.IO;

namespace Weavver.Net.HTTP
{
     public static class FormPost
     {
//-------------------------------------------------------------------------------------------
          // pulled code from this example: http://www.experts-exchange.com/Programming/Languages/.NET/Q_24173618.html
          public static bool HttpUploadFile(string url, byte[] fileData, string paramName, string contentType, NameValueCollection nvc)
          {
               string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
               byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

               HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);
               wr.ContentType = "multipart/form-data; boundary=" + boundary;
               wr.Method = "POST";
               wr.KeepAlive = true;
               wr.Credentials = System.Net.CredentialCache.DefaultCredentials;

               Stream rs = wr.GetRequestStream();

               string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
               foreach (string key in nvc.Keys)
               {
                    rs.Write(boundarybytes, 0, boundarybytes.Length);
                    string formitem = string.Format(formdataTemplate, key, nvc[key]);
                    byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
                    rs.Write(formitembytes, 0, formitembytes.Length);
               }
               rs.Write(boundarybytes, 0, boundarybytes.Length);

               string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
               string header = string.Format(headerTemplate, paramName, paramName, contentType);
               byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
               rs.Write(headerbytes, 0, headerbytes.Length);

               //FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
               //byte[] buffer = new byte[4096];
               //int bytesRead = 0;
               //while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
               //{
               //     rs.Write(buffer, 0, bytesRead);
               //}
               //fileStream.Close();

               rs.Write(fileData, 0, fileData.Length);

               byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
               rs.Write(trailer, 0, trailer.Length);
               rs.Close();

               WebResponse wresp = null;
               wresp = wr.GetResponse();
               Stream receiveStream = wresp.GetResponseStream();
               Encoding encode = System.Text.Encoding.GetEncoding("utf-8"); // Gets the stream associated with the response.
               StreamReader readStream = new StreamReader(receiveStream, encode); // Pipes the stream to a higher level stream reader with the required encoding format. 
               Char[] read = new Char[256];
               int count = readStream.Read(read, 0, 256); // Reads 256 characters at a time.
               string response = "";
               while (count > 0)
               {
                    String str = new String(read, 0, count); // Dumps the 256 characters on a string and displays the string to the console.
                    response += str;
                    count = readStream.Read(read, 0, 256);
               }
               wresp.Close(); // Releases the resources of the response.
               readStream.Close(); // Releases the resources of the Stream.

               if (((System.Net.HttpWebResponse)(wresp)).StatusCode == HttpStatusCode.OK)
                    return true;

               return false;
          }
//-------------------------------------------------------------------------------------------
     }
}
