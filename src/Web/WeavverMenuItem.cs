using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

namespace Weavver.Web
{
     public class WeavverMenuItem
     {
          public string Name { get; set; }
          public string Link { get; set; }
          public bool CanAdd { get; set; }
          public string Style { get; set; }
          public int Height { get; set; }
          public int Width { get; set; }
          public string Title { get; set; }
          public WeavverMenuItem parent;
          public List<WeavverMenuItem> Items = new List<WeavverMenuItem>();
          public object DataProperties { get; set; }
//-------------------------------------------------------------------------------------------
          public WeavverMenuItem()
          {
               CanAdd = false;
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