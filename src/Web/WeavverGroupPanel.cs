using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;

namespace Weavver.Web
{
     public class WeavverGroupPanel : Panel
	{
          private string title;
          private Unit   titlewidth;
//------------------------------------------------------------------------------------------
          public WeavverGroupPanel()
		{
		}
//------------------------------------------------------------------------------------------
          [Category("Weavver")]
          [PersistenceMode(PersistenceMode.Attribute)]
          [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
          public string Title
          {
               get
               {
                    return title;
               }
               set
               {
                    title = value;
               }
          }
//------------------------------------------------------------------------------------------
          [Category("Weavver")]
          [PersistenceMode(PersistenceMode.Attribute)]
          [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
          public Unit TitleWidth
          {
               get
               {
                    return titlewidth;
               }
               set
               {
                    titlewidth = value;
               }
          }
//------------------------------------------------------------------------------------------
          protected override void Render(HtmlTextWriter writer)
          {
               writer.AddAttribute(HtmlTextWriterAttribute.Cellpadding, "0");
               writer.AddAttribute(HtmlTextWriterAttribute.Cellspacing, "0");
               writer.AddAttribute(HtmlTextWriterAttribute.Width, Width.ToString());
               writer.AddAttribute(HtmlTextWriterAttribute.Border, "0px");
               writer.RenderBeginTag("table");
                    writer.AddAttribute(HtmlTextWriterAttribute.Valign, "bottom");
                    writer.AddAttribute(HtmlTextWriterAttribute.Height, "20px");
                    writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                         RenderColumn(writer, "20px", "20px", "20px", "images/topleft.jpg");
                         RenderTopMiddleColumn(writer);
                         RenderColumn(writer, "20px",  "20px", "20px", "images/topright.jpg");
                    writer.RenderEndTag();
                    writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                         RenderColumn(writer, "100%", "5px", "100%", "images/leftbar.jpg");
                         RenderBaseColumn(writer);
                         RenderColumn(writer, "100%", "20px", "100%", "images/rightbar.jpg");
                    writer.RenderEndTag();
                    writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                         RenderColumn(writer, "20px", "20px", "20px", "images/bottomleft.jpg");
                         RenderColumn(writer, "20px", "100%", "20px", "images/bottombar.jpg");
                         RenderColumn(writer, "20px", "20px", "20px", "images/bottomright.jpg");
                    writer.RenderEndTag();
               writer.RenderEndTag();
          }
//------------------------------------------------------------------------------------------
          private void RenderTopMiddleColumn(HtmlTextWriter writer)
          {
               writer.AddAttribute(HtmlTextWriterAttribute.Width, "10px");
               writer.RenderBeginTag(HtmlTextWriterTag.Td);
                    writer.AddAttribute(HtmlTextWriterAttribute.Width,  "20px");
                    writer.AddAttribute(HtmlTextWriterAttribute.Height, "20px");
                    writer.AddAttribute(HtmlTextWriterAttribute.Src,    "images/topbar.jpg");
                    writer.RenderBeginTag(HtmlTextWriterTag.Img);
                    writer.RenderEndTag();
               writer.RenderEndTag();
               writer.AddAttribute(HtmlTextWriterAttribute.Width,  TitleWidth.ToString() + "px");
               writer.AddAttribute(HtmlTextWriterAttribute.Height, "20px");
               writer.AddAttribute(HtmlTextWriterAttribute.Valign, "middle");
               writer.RenderBeginTag(HtmlTextWriterTag.Td);
                    writer.Write("&nbsp;" + Title + "&nbsp;");
               writer.RenderEndTag();
               writer.RenderBeginTag(HtmlTextWriterTag.Td);
                    writer.AddAttribute(HtmlTextWriterAttribute.Height, "20px");
                    double width = (Width.Value - TitleWidth.Value);
                    writer.AddAttribute(HtmlTextWriterAttribute.Width,  width.ToString() + "px");
                    writer.AddAttribute(HtmlTextWriterAttribute.Src,    "images/topbar.jpg");
                    writer.RenderBeginTag(HtmlTextWriterTag.Img);
                    writer.RenderEndTag();
               writer.RenderEndTag(); 
          }
//------------------------------------------------------------------------------------------
          private void RenderBaseColumn(HtmlTextWriter writer)
          {
               writer.AddAttribute(HtmlTextWriterAttribute.Valign, "top");
               writer.AddAttribute(HtmlTextWriterAttribute.Colspan,   "3");
               writer.RenderBeginTag(HtmlTextWriterTag.Td);
                    base.Render(writer);
               writer.RenderEndTag();
          }
//------------------------------------------------------------------------------------------
          private void RenderColumn(HtmlTextWriter writer, string height, string imagewidth, string imageheight, string image)
          {
               if (image == "images/leftbar.jpg")
               {
                    writer.AddAttribute(HtmlTextWriterAttribute.Align, "middle");
               }
               if (image == "images/bottombar.jpg")
               {
                    writer.AddAttribute(HtmlTextWriterAttribute.Colspan, "3");
               }
               writer.AddAttribute(HtmlTextWriterAttribute.Height, height);
               writer.RenderBeginTag(HtmlTextWriterTag.Td);
                    writer.AddAttribute(HtmlTextWriterAttribute.Width, imagewidth);
                    writer.AddAttribute(HtmlTextWriterAttribute.Height, imageheight);
                    writer.AddAttribute(HtmlTextWriterAttribute.Src, image);
                    writer.RenderBeginTag(HtmlTextWriterTag.Img);
               writer.RenderEndTag();
               writer.RenderEndTag();
          }
//------------------------------------------------------------------------------------------
	}
}
//                    <tr>
//                         <td valign=\"top\" align=\"middle\" height=\"100%\">
//                              <img src=\"images/leftbar.jpg\" width=\"5px\" height=\"100%\">
//                         </td>
//                         <td valign=\"top\" colspan=\"3\">";
//                              Options: <a href="taskadd.aspx">Add Task</a>, <asp:LinkButton id="TasksShowCompleted" runat="server">Show Completed Tasks</asp:LinkButton><br>
//                              <br>
//                              <table width="100%">
//                              <tr>
//                                   <td>1. 02/14/05 HEC - Finish making valentine's day forms...</td>
//                                   <td width="70px" align="center" bgcolor="orange">In Progress</td>
//                              </tr>
//                              <tr>
//                                   <td>2. 02/14/05 HEC - Get tax information ready for IRS visit Tuesday.</td>
//                                   <td width="70px" align="center" bgcolor="orange">In Progress</td>
//                              </tr>
//                              </table>
//                         </td>
//                         <td valign="top" align="middle" height="100%">
//                              <img src="images/rightbar.jpg" width="20px" height="100%">
//                         </td>
//                    </tr>
//                    <tr valign="bottom">
//                         <td width="20px"><img src="images/bottomleft.jpg"></td>
//                         <td colspan="3"><img width="100%" height="20px" src="images/bottombar.jpg"></td>
//                         <td><img src="images/bottomright.jpg"></td>
//                    </tr>
//                    </table>
//               </td>"







//                         <td width=\"10px\"><img width=\"10px\" height=\"20px\" src=\"images/topbar.jpg\"></td>
//                         <td width=\"100px\" valign="middle">&nbsp;Today's Tasks&nbsp;&nbsp;</td>
//                         <td><img width=\"400px\" height=\"20px\" src="images/topbar.jpg\"></td>
//                         <td><img src=\"images/topright.jpg\"></td>
//                    </tr>
//                    <tr>
//                         <td valign=\"top\" align=\"middle\" height=\"100%\">
//                              <img src=\"images/leftbar.jpg\" width=\"5px\" height=\"100%\">
//                         </td>
//                         <td valign=\"top\" colspan=\"3\">";
//                              Options: <a href="taskadd.aspx">Add Task</a>, <asp:LinkButton id="TasksShowCompleted" runat="server">Show Completed Tasks</asp:LinkButton><br>
//                              <br>
//                              <table width="100%">
//                              <tr>
//                                   <td>1. 02/14/05 HEC - Finish making valentine's day forms...</td>
//                                   <td width="70px" align="center" bgcolor="orange">In Progress</td>
//                              </tr>
//                              <tr>
//                                   <td>2. 02/14/05 HEC - Get tax information ready for IRS visit Tuesday.</td>
//                                   <td width="70px" align="center" bgcolor="orange">In Progress</td>
//                              </tr>
//                              </table>
//                         </td>
//                         <td valign="top" align="middle" height="100%">
//                              <img src="images/rightbar.jpg" width="20px" height="100%">
//                         </td>
//                    </tr>
//                    <tr valign="bottom">
//                         <td width="20px"><img src="images/bottomleft.jpg"></td>
//                         <td colspan="3"><img width="100%" height="20px" src="images/bottombar.jpg"></td>
//                         <td><img src="images/bottomright.jpg"></td>
//                    </tr>
//                    </table>
//               </td>";