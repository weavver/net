using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace Weavver.Net.HTTP
{
     public class FormUploadCertPolicy : ICertificatePolicy
     {
          // Default policy for certificate validation.
          // public static bool DefaultValidate = false;

          public bool CheckValidationResult(ServicePoint sp, X509Certificate cert, WebRequest request, int problem)
          {
               //bool ValidationResult = false;
               //Console.WriteLine("Certificate Problem with accessing " + request.RequestUri);
               //Console.Write("Problem code 0x{0:X8},", (int)problem);
               //Console.WriteLine(GetProblemMessage((CertificateProblem)problem));
               //ValidationResult = DefaultValidate;
               return true;
          }

          //private string GetProblemMessage(System.Net.CertificateProblem Problem)
          //{
          //     CertificateProblem problemList = new CertificateProblem();
          //     string message = Enum.GetName(problemList.GetType(), Problem);
          //     if (ProblemCodeName != null)
          //          message = message + "-codename:" + ProblemCodeName;
          //     else
          //          message = "Unknown Certificate Problem";
          //     return message;
          //}
     }
   
}
