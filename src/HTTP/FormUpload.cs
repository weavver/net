using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Weavver.Net.HTTP
{
//--------------------------------------------------------------------------------------------
     public struct UploadStatus
     {
          public HttpStatusCode StatusCode;
          public string StatusDescription;
          public string ResponseText;
     }
//--------------------------------------------------------------------------------------------
     /// <summary>
     /// Original example provided from http://stackoverflow.com/questions/219827/multipart-forms-from-c-client
     /// </summary>
     public class FormUpload
     {
//--------------------------------------------------------------------------------------------
          public static Encoding encoding = Encoding.UTF8;

          /// <summary>
          /// Post the data as a multipart form
          /// postParameters with a value of type byte[] will be passed in the form as a file, and value of type string will be
          /// passed as a name/value pair.
          /// </summary>
          public static UploadStatus MultipartFormDataPost(string postUrl, string username, string password, string userAgent, Dictionary<string, object> postParameters)
          {
               string formDataBoundary = "-----------------------------28947758029299";
               string contentType = "multipart/form-data; boundary=" + formDataBoundary;

               byte[] formData = GetMultipartFormData(postParameters, formDataBoundary);

               return PostForm(postUrl, username, password, userAgent, contentType, formData);
          }
//--------------------------------------------------------------------------------------------
          public static bool Validator(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
          {
               return true;
          }
//--------------------------------------------------------------------------------------------
          /// <summary>
          /// Post a form
          /// </summary>
          private static UploadStatus PostForm(string postUrl, string username, string password, string userAgent, string contentType, byte[] formData)
          {
               ServicePointManager.CertificatePolicy = new FormUploadCertPolicy();
               ServicePointManager.ServerCertificateValidationCallback = Validator;
               ServicePointManager.Expect100Continue = true;
               
               HttpWebRequest request = WebRequest.Create(postUrl) as HttpWebRequest;
               if (username != null && password != null)
               {
                    string usernamePassword = username + ":" + password;
                    CredentialCache mycache = new CredentialCache();
                    mycache.Add(new Uri(postUrl), "Basic", new NetworkCredential(username, password));
                    request.Credentials = mycache;
                    request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(new ASCIIEncoding().GetBytes(usernamePassword)));
               }
               if (request == null)
               {
                    throw new NullReferenceException("request is not a http request");
               }

               request.Method = "POST";
               request.ContentType = contentType;
               request.CookieContainer = new CookieContainer();
               request.UserAgent = userAgent;
               // We need to count how many bytes we're sending. 
               request.ContentLength = formData.Length;

               using (Stream requestStream = request.GetRequestStream())
               {
                    // Push it out there
                    requestStream.Write(formData, 0, formData.Length);
                    requestStream.Close();
               }
               HttpWebResponse resp = (HttpWebResponse)request.GetResponse();
               StreamReader reader = new StreamReader(resp.GetResponseStream());
               string responseText = reader.ReadToEnd();
               UploadStatus status = new UploadStatus();
               status.StatusCode = resp.StatusCode;
               status.StatusDescription = resp.StatusDescription;
               status.ResponseText = responseText;
               return status;
          }
//--------------------------------------------------------------------------------------------
          /// <summary>
          /// Turn the key and value pairs into a multipart form.
          /// See http://www.ietf.org/rfc/rfc2388.txt for issues about file uploads
          /// </summary>
          private static byte[] GetMultipartFormData(Dictionary<string, object> postParameters, string boundary)
          {
               Stream formDataStream = new System.IO.MemoryStream();
               byte[] newLine = encoding.GetBytes("\r\n");
               foreach (var param in postParameters)
               {
                    if (param.Value is byte[])
                    {
                         string header = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"; filename=\"{2}\";\r\nContent-Type: audio/x-wav\r\n\r\n", boundary, param.Key, "audio.wav");
                         byte[] headerBytes = encoding.GetBytes(header);
                         formDataStream.Write(headerBytes, 0, headerBytes.Length);
                         byte[] content = param.Value as byte[];
                         formDataStream.Write(content, 0, content.Length);
                         formDataStream.Write(newLine, 0, newLine.Length);
                    }
                    else
                    {
                         string postData = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}", boundary, param.Key, param.Value);
                         byte[] postDataBytes = encoding.GetBytes(postData);
                         formDataStream.Write(postDataBytes, 0, postDataBytes.Length);
                         formDataStream.Write(newLine, 0, newLine.Length);
                    }
               }

               // Add the end of the request
               string footer = "--" + boundary + "--\r\n";
               formDataStream.Write(encoding.GetBytes(footer), 0, footer.Length);

               // Dump the Stream into a byte[]
               formDataStream.Position = 0;
               byte[] formData = new byte[formDataStream.Length];
               formDataStream.Read(formData, 0, formData.Length);
               formDataStream.Close();
               return formData;
          }
//--------------------------------------------------------------------------------------------
     }
}
