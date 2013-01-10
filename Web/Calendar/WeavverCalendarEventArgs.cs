using System;
using System.Web.UI.WebControls;

namespace Weavver.Web
{
//------------------------------------------------------------------------------------------
     public class WeavverCalendarDayLoadEventArgs : EventArgs
     {
          private DateTime  datetime;
          private TableCell daycell;
//------------------------------------------------------------------------------------------
          public WeavverCalendarDayLoadEventArgs(DateTime datetime, TableCell daycell)
          {
               datetime = this.datetime;
               daycell  = this.daycell;
          }
//------------------------------------------------------------------------------------------
     }
//------------------------------------------------------------------------------------------
     public class WeavverCalendarDaySelectedEventArgs : EventArgs
     {
          private DateTime datetime;
//------------------------------------------------------------------------------------------
          public WeavverCalendarDaySelectedEventArgs(DateTime datetime)
          {
               datetime = this.datetime;
          }
//------------------------------------------------------------------------------------------
     }
//------------------------------------------------------------------------------------------
     public class WeavverCalendarMonthLoadEventArgs : EventArgs
     {
          private DateTime datetime;
//------------------------------------------------------------------------------------------
          public WeavverCalendarMonthLoadEventArgs(DateTime datetime)
          {
               datetime = this.datetime;
          }
//------------------------------------------------------------------------------------------
     }
//------------------------------------------------------------------------------------------
}
