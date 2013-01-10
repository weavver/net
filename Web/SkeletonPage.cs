using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace Weavver.Web
{
     public class SkeletonPage : Page
     {
//-------------------------------------------------------------------------------------------
          public SkeletonPage()
          {
          }
//-------------------------------------------------------------------------------------------
		public string GetVar(string name)
		{
			string namevalue;
			if (Request[name] != null)
			{
				Session[name] = Request[name];
				return Request[name].ToString();
			}
			else if (Session[name] != null)
			{
				return Session[name].ToString();
			}
			else
			{
				return null;
			}
		}
//-------------------------------------------------------------------------------------------
     }
}