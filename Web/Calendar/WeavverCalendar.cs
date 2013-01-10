using System;
using System.ComponentModel;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Weavver.Web
{
//------------------------------------------------------------------------------------------
     public delegate void WeavverCalendarDayLoadEventHandler(object sender, WeavverCalendarDayLoadEventArgs e);
     public delegate void WeavverCalendarDaySelectedEventHandler(object sender, WeavverCalendarDaySelectedEventArgs e);
     public delegate void WeavverCalendarMonthLoadEventHandler(object sender, WeavverCalendarMonthLoadEventArgs e);
//------------------------------------------------------------------------------------------
     [PersistChildren(false)]
     [ToolboxItem(true)]
     [Description("This is an easy to use calendar control.")]
     public class WeavverCalendar : WeavverWebControl, IPostBackEventHandler
     {
          private Color    fHeaderBackColor                           = ColorTranslator.FromHtml("#CCCCCC");//Color.LightCoral;
          private Color    fDaysBackColor                             = ColorTranslator.FromHtml("#E6E6E6");
          private Color    fNormalColor                               = Color.White;
          private Color    fShadeColor                                = Color.WhiteSmoke;
          private Unit     fDayColumnHeight                           = Unit.Empty;
          private DateTime fSelectedDateTime                          = DateTime.Now;

          public  event    WeavverCalendarDayLoadEventHandler DayLoad;
          public  event    WeavverCalendarDaySelectedEventHandler DaySelected;
          public  event    WeavverCalendarMonthLoadEventHandler MonthLoad;
//------------------------------------------------------------------------------------------
          public WeavverCalendar()
          {
               fDayColumnHeight = Unit.Parse("80px");
          }
//------------------------------------------------------------------------------------------
          [Category("Weavver, Inc.")]
          [PersistenceMode(PersistenceMode.Attribute)]
          [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
          [Description("This is the currently selected date.")]
          public DateTime SelectedDateTime
          {
               get
               {
                    return fSelectedDateTime;
               }
               set
               {
                    fSelectedDateTime = value;
               }
          }
//------------------------------------------------------------------------------------------
          [Category("Weavver, Inc.")]
          [PersistenceMode(PersistenceMode.Attribute)]
          [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
          [Description("The back color of the header row.")]
          public Color HeaderBackColor
          {
               get
               {
                    return fHeaderBackColor;
               }
               set
               {
                    fHeaderBackColor = value;
               }
          }
//------------------------------------------------------------------------------------------
          [Category("Weavver, Inc.")]
          [PersistenceMode(PersistenceMode.Attribute)]
          [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
          [Description("The normal color of the days that are in the current selected month.")]
          public Color NormalColor
          {
               get
               {
                    return fNormalColor;
               }
               set
               {
                    fNormalColor = value;
               }
          }
//------------------------------------------------------------------------------------------
          [Category("Weavver, Inc.")]
          [PersistenceMode(PersistenceMode.Attribute)]
          [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
          [Description("The shade color of the days that are not in the current selected month.")]
          public Color ShadeColor
          {
               get
               {
                    return fShadeColor;
               }
               set
               {
                    fShadeColor = value;
               }
          }
//------------------------------------------------------------------------------------------
          [Category("Database Works, Inc.")]
          [PersistenceMode(PersistenceMode.Attribute)]
          [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
          [Description("The back color of the day names row.")]
          public Color DaysBackColor
          {
               get
               {
                    return fDaysBackColor;
               }
               set
               {
                    fDaysBackColor = value;
               }
          }
//------------------------------------------------------------------------------------------
          [Category("Weavver, Inc.")]
          [PersistenceMode(PersistenceMode.Attribute)]
          [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
          [Description("This is the hard coded height of the day columns in the calendar. If you want it to automatically size itself leave this blank.")]
          public Unit DayColumnHeight
          {
               get
               {
                    return fDayColumnHeight;
               }
               set
               {
                    fDayColumnHeight = value;
               }
          }
          //------------------------------------------------------------------------------------------
          public void RaisePostBackEvent(string eventArgument)
          {
               // int					i		= Int32.Parse(eventArgument);
               // WeavverDropDownMenuItem	ddmi	= Items[i];
               // Items[i].TriggerClick(EventArgs.Empty);
               // if (fClick != null)
               // {
               //      fClick(ddmi, EventArgs.Empty);
               // }
          }
          //------------------------------------------------------------------------------------------
          protected override void Render(HtmlTextWriter writer)
          {
               Table table                   = new Table();
               table.BorderColor             = BorderColor;
               table.BackColor               = BackColor;
               table.BorderStyle             = BorderStyle;
               table.BorderWidth             = BorderWidth;
               table.CellPadding             = 0;
               table.CellSpacing             = 0;

               int selectedmonth             = DateTime.Now.Month;
               int selectedyear              = DateTime.Now.Year;

               WeavverCalendarMonthLoadEventArgs monthloadargs = new WeavverCalendarMonthLoadEventArgs(SelectedDateTime);
               if (MonthLoad != null)
               {
                    MonthLoad(this, monthloadargs);
               }

               DateTime datetime             = SelectedDateTime.Subtract(TimeSpan.FromDays(SelectedDateTime.Day));
               datetime                      = datetime.Subtract(TimeSpan.FromDays(DayNumber(datetime.DayOfWeek) - 1));
               table.Width                   = Width;

               table.Rows.Add(new TableRow());
               TableCell header1             = new TableCell();
               header1.BackColor             = ColorTranslator.FromHtml("#AFB29C");
               header1.Height                = Unit.Parse("30px");
               header1.ColumnSpan             = 7;
               header1.HorizontalAlign        = HorizontalAlign.Center;
               header1.Text                   = MonthText(datetime.Month);
               table.Rows[0].Controls.Add(header1);

               table.Rows.Add(new TableRow());
               table.Rows[1].BackColor       = DaysBackColor;
               table.Rows[1].HorizontalAlign = HorizontalAlign.Center;
               AddHeaderColumn(table, "Sunday");
               AddHeaderColumn(table, "Monday");
               AddHeaderColumn(table, "Tuesday");
               AddHeaderColumn(table, "Wednesday");
               AddHeaderColumn(table, "Thursday");
               AddHeaderColumn(table, "Friday");
               AddHeaderColumn(table, "Saturday");
               table.Rows.Add(new TableRow());
               while (true)
               {
                    TableCell daycell;
                    if (selectedmonth == datetime.Month)
                    {
                         daycell           = AddDayColumn(table, datetime.Day.ToString(), NormalColor);
                    }
                    else
                    {
                         daycell           = AddDayColumn(table, datetime.Day.ToString(), ShadeColor);
                         daycell.ForeColor = Color.LightGray;
                    }
                    WeavverCalendarDayLoadEventArgs dayloadargs = new WeavverCalendarDayLoadEventArgs(datetime, daycell);
                    if (DayLoad != null)
                    {
                         DayLoad(this, dayloadargs);
                    }

                    if (    selectedmonth != datetime.Month
                         && !GreaterMonth(selectedmonth, selectedyear, datetime.Month, datetime.Year)
                         && datetime.DayOfWeek == DayOfWeek.Saturday)
                    {
                         break;
                    }
                    else if (datetime.DayOfWeek == DayOfWeek.Saturday)
                    {
                         table.Rows.Add(new TableRow());
                    }
                    datetime = datetime.AddDays(1);
               }

               table.Rows.Add(new TableRow());
               table.Rows[table.Rows.Count - 1].BackColor       = HeaderBackColor;

               DropDownList monthlist        = new DropDownList();
               monthlist.Items.Add("January");
               monthlist.Items.Add("February");
               monthlist.Items.Add("March");
               monthlist.Items.Add("April");
               monthlist.Items.Add("May");
               monthlist.Items.Add("June");
               monthlist.Items.Add("July");
               monthlist.Items.Add("August");
               monthlist.Items.Add("September");
               monthlist.Items.Add("October");
               monthlist.Items.Add("November");
               monthlist.Items.Add("December");
               monthlist.Width               = Unit.Parse("100px");

               TableCell headera             = new TableCell();
               headera.ColumnSpan            = 2;
               
               TableCell headerb             = new TableCell();
               headerb.ColumnSpan            = 3;
               headerb.HorizontalAlign       = HorizontalAlign.Left;
               //headerb.Text                  = MonthText(datetime.Month);
               
               Button yearprevious           = new Button();
               yearprevious.Text             = "-";
               yearprevious.BorderWidth      = Unit.Parse("1px");
               yearprevious.BorderStyle      = BorderStyle.Solid;
               yearprevious.Width            = Unit.Parse("22px");
               yearprevious.BorderColor      = Color.DarkGray;
               
               TextBox year                  = new TextBox();
               year.Text                     = " " + SelectedDateTime.Year.ToString();
               year.BorderWidth              = Unit.Parse("1px");
               year.Height                   = Unit.Parse("22px");
               year.BorderStyle              = BorderStyle.Solid;
               year.BorderColor              = Color.DarkGray;
               year.Width                    = Unit.Parse("50px");

               Button  yearnext              = new Button();
               yearnext.Text                 = "+";
               yearnext.BorderWidth          = Unit.Parse("1px");
               yearnext.BorderStyle          = BorderStyle.Solid;
               yearnext.BorderColor          = Color.DarkGray;
               yearnext.Width                = Unit.Parse("22px");

               TableCell headerc             = new TableCell();
               headerc.ColumnSpan            = 2;
               headerc.HorizontalAlign       = HorizontalAlign.Right;
               
               headera.Controls.Add(yearprevious);
               headera.Controls.Add(year);
               headera.Controls.Add(yearnext);

               headerc.Controls.Add(monthlist);

               table.Rows[table.Rows.Count - 1].Style.Add("padding", "2");
               table.Rows[table.Rows.Count - 1].Controls.Add(headera);
               table.Rows[table.Rows.Count - 1].Controls.AddAt(1, headerb);
               table.Rows[table.Rows.Count - 1].Controls.AddAt(2, headerc);

               this.Controls.Add(table);
               base.Render(writer);
          }
          //------------------------------------------------------------------------------------------
          private void AddHeaderColumn(Table table, string text)
          {
               TableCell tc        = new TableCell();
               tc.Text             = text;
               tc.HorizontalAlign  = HorizontalAlign.Center;
               double width        = Width.Value / 7;
               width               = Math.Round(width, 0);
               tc.Width            = Unit.Parse(width.ToString() + "px");
               table.Rows[1].Cells.Add(tc);
          }
          //------------------------------------------------------------------------------------------
          private TableCell AddDayColumn(Table table, string text, Color backcolor)
          {
               TableCell tc        = new TableCell();
               tc.BorderStyle      = BorderStyle;
               tc.BorderWidth      = BorderWidth;
               tc.Text             = text + "&nbsp;";
               tc.BackColor        = backcolor;
               tc.Height           = DayColumnHeight;
               tc.HorizontalAlign  = HorizontalAlign.Right;
               tc.Style.Add("cursor", "hand");
               tc.VerticalAlign    = VerticalAlign.Top;
               table.Rows[table.Rows.Count - 1].Cells.Add(tc);
               return tc;
          }
          //------------------------------------------------------------------------------------------
          private int DayNumber(DayOfWeek day)
          {
               switch (day)
               {
                    case DayOfWeek.Saturday:
                         return 1;
                         
                    case DayOfWeek.Monday:
                         return 2;
                    
                    case DayOfWeek.Tuesday:
                         return 3;
                         
                    case DayOfWeek.Wednesday:
                         return 4;

                    case DayOfWeek.Thursday:
                         return 5;
                         
                    case DayOfWeek.Friday:
                         return 6;

                    case DayOfWeek.Sunday:
                         return 7;

                    default:
                         return -1;
               }
          }
          //------------------------------------------------------------------------------------------
          private string MonthText(int month)
          {
               switch (month)
               {
                    case 1:
                         return "January";
                         
                    case 2:
                         return "February";
                    
                    case 3:
                         return "March";
                         
                    case 4:
                         return "April";

                    case 5:
                         return "May";
                         
                    case 6:
                         return "June";

                    case 7:
                         return "July";

                    case 8:
                         return "August";

                    case 9:
                         return "September";

                    case 10:
                         return "October";
                    
                    case 11:
                         return "November";

                    case 12:
                         return "December";

                    default:
                         return "Unknown";
               }
          }
//------------------------------------------------------------------------------------------
          private bool GreaterMonth(int month, int year, int comparemonth, int compareyear)
          {
               if (year < compareyear)
               {
                    return false;
               }
               else if (year > compareyear)
               {
                    return true;
               }

               if (month < comparemonth)
               {
                    return false;
               }
               else if (month > comparemonth)
               {
                    return true;
               }
               return false;
          }
//------------------------------------------------------------------------------------------
          public void AddData(TableCell cell, string columnone, string columntwo)
          {
               AddData(cell, columnone, columntwo, "");
          }
//------------------------------------------------------------------------------------------
          public void AddData(TableCell cell, string columnone, string columntwo, string imageurl)
          {
               cell.Text += "<table><tr><td>" + columnone + columntwo + "</td></tr></table>";
          }
//------------------------------------------------------------------------------------------
     }
}
