using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Weavver.Web
{
     public class WeavverMenuItem
     {
          public string Name { get; set; }
          public string Link { get; set; }
          public WeavverMenuItem parent;
          public List<WeavverMenuItem> Items = new List<WeavverMenuItem>();
//-------------------------------------------------------------------------------------------
          public WeavverMenuItem()
          {
          }
//-------------------------------------------------------------------------------------------
          public WeavverMenuItem(string name, string link)
          {
               this.Name = name;
               this.Link = link;
          }
//-------------------------------------------------------------------------------------------
     }
}