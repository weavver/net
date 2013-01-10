using System;
using System.Web;
using System.Drawing.Design;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.ComponentModel;
using System.Drawing;
using System.Collections;
using System.Data;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Specialized;

using TitaniumSoft;

namespace Weavver.Web
{
     [ToolboxData("<{0}:WeavverDataGrid runat='server' EnableColumnDrag='true' EnableClientSort='true'></{0}:WeavverDataGrid>")]
     public class WeavverDataGrid : DataGrid, IPostBackDataHandler
     {
          private static readonly object EventRowDblClick  = new object();
          private        int             fRowDoubleClicked = -1;
//-------------------------------------------------------------------------------------------
          public WeavverDataGrid()
          {
               PreRender += new EventHandler(WeavverDataGrid_PreRender);
          }
//------------------------------------------------------------------------------------------
          protected override void OnInit(EventArgs e)
          {
               base.OnInit(e);
               this.Page.RegisterRequiresPostBack(this);
          }
//-------------------------------------------------------------------------------------------
          [Category("Appearance")]
          [PersistenceMode(PersistenceMode.Attribute)]
          [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
          public Color HighlightColor
          {
               get 
               {
                    Color color = Color.White;
                    if (ViewState["HighlightColor"] != null)
                    {
                         color = (Color) ViewState["HighlightColor"];
                    }
                    return color;
               }
               set 
               {
                    ViewState["HighlightColor"] = value;
               }
          }
//-------------------------------------------------------------------------------------------
          [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
          public int RowDoubleClicked
          {
               get 
               {
                    return fRowDoubleClicked;
               }
          }
//-------------------------------------------------------------------------------------------
          [Category("JavaScript Action")]
          [PersistenceMode(PersistenceMode.Attribute)]
          [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
          public string OnRowClick
          {
               get
               {
                    string onrowclick = "return false;";
                    if (ViewState["OnRowClick"] != null)
                    {
                         onrowclick = (string) ViewState["OnRowClick"];
                    }
                    return "";
               }
               set
               {
                    ViewState["OnRowClick"] = value;
               }
          }
//------------------------------------------------------------------------------------------
          public void TriggerClick(EventArgs e)
          {
               EventHandler clickHandler = (EventHandler) Events[EventRowDblClick];
               if (clickHandler != null)
               {
                    clickHandler(this, e);
               }
          }
//------------------------------------------------------------------------------------------
          public event EventHandler RowDblClick
          {
               add
               {
                    Events.AddHandler(EventRowDblClick, value);
               }
               remove
               {
                    Events.RemoveHandler(EventRowDblClick, value);
               }
          }
//-------------------------------------------------------------------------------------------
          public void AddXmlAttribute(HtmlTextWriter writer)
          {
               //	         if (Designer.ControlMode == fControlMode.Design)
               //	         {
               //	            writer.AddAttribute("xml", ToXml());
               //	         }
          }
//-------------------------------------------------------------------------------------------
          public virtual void RaisePostDataChangedEvent() 
          {
               bool x = true;

               // Do nothing here
               // No need of firing server-side events
          }
//-------------------------------------------------------------------------------------------
          public virtual bool LoadPostData(string postkey, NameValueCollection postcollection)
          {
               if (postcollection[UniqueID + "_RowDoubleClicked"] != null)
               {
                    fRowDoubleClicked             = Int32.Parse(postcollection[UniqueID + "_RowDoubleClicked"].ToString());
                    if (RowDoubleClicked > -1)
                    {
                         EventHandler     clickHandler = (EventHandler) Events[EventRowDblClick];
                         TriggerClick(EventArgs.Empty);
                    }
               }
               return false;
          }
//-------------------------------------------------------------------------------------------
          //protected override void Render(HtmlTextWriter output)       
          //{
          //     if (Visible)
          //     {
          //          //if (!Page.IsClientScriptBlockRegistered("WeavverDataGrid"))
          //          //{
          //          //     output.Write("<script language=JavaScript src=\"tsdatagrid.js\"></script>");
          //          //     Page.RegisterClientScriptBlock("WeavverDataGrid", "");
          //          //}
          //          output.AddAttribute("id", this.ID);
          //          if (this.ControlStyleCreated && this.ControlStyle != null)
          //          {
          //               ControlStyle.AddAttributesToRender(output);
          //          }
          //          if (this.CssClass != string.Empty)
          //          {
          //               output.AddAttribute("class", CssClass);
          //          }
          //          //base.Render(output);
          //          output.RenderBeginTag("table");
          //          this.RenderContents(output);
          //          output.RenderEndTag();
          //     }
          //}
//-------------------------------------------------------------------------------------------
          protected override DataGridItem CreateItem(int itemIndex, int dataSourceIndex, ListItemType itemType)
          {
               // Create the DataGridItem
               DataGridItem item = new DataGridItem(itemIndex, dataSourceIndex, itemType);

               // Set the client-side onmouseover and onmouseout if RowSelectionEnabled == true
               //if (RowSelectionEnabled && itemType != ListItemType.Header &&
               //     itemType != ListItemType.Footer && itemType != ListItemType.Pager)
               //{
               //     item.Attributes["onmouseover"] = "javascript:prettyDG_changeBackColor(this, true);";
               //     item.Attributes["onmouseout"] = "javascript:prettyDG_changeBackColor(this, false);";
               //}
               if (itemType == ListItemType.Item)
               {
                    item.Attributes["cursor"] = "hand";
               }
               else if (itemType == ListItemType.AlternatingItem)
               {
                    item.Attributes["cursor"] = "hand";
               }
               //string clientjs = Page.GetPostBackClientEvent(this, Controls[0].Controls.IndexOf().ToString());
               //int index = Controls[0].Controls.IndexOf(row) - 2;
               //item.Attributes["ondblclick"] = "document.getElementById('" + this.UniqueID + "_RowDoubleClicked').value = " + (itemIndex + 1).ToString() + ";" + clientjs;

               item.Attributes["test"] = "test";
               for (int i = 0; i < Columns.Count; i++)
               {
               }
               return item;
          }
//-------------------------------------------------------------------------------------------
          protected void RenderContents(HtmlTextWriter output)
          {
               Page.ClientScript.RegisterHiddenField(this.UniqueID + "_RowDoubleClicked", "-1");
               TableItemStyle style = this.ItemStyle;
               if (HasControls())
               {
                    foreach (DataGridItem row in Controls[0].Controls)
                    {
                         bool itemrow = false;
                         switch (row.ItemType)
                         {
                              case ListItemType.Header:
                                   HeaderStyle.AddAttributesToRender(output);
                                   break;

                              case ListItemType.Footer:
                                   FooterStyle.AddAttributesToRender(output);
                                   break;

                              case ListItemType.Pager:
                                   if (PagerStyle.Position == PagerPosition.Top && Controls[0].Controls.IndexOf(row) > 2)
                                   {
                                        continue;
                                   }
                                   if (PagerStyle.Position == PagerPosition.Bottom && Controls[0].Controls.IndexOf(row) < 2)
                                   {
                                        continue;
                                   }
                                   this.PagerStyle.ForeColor     = this.PagerStyle.ForeColor;
                                   this.PagerStyle.AddAttributesToRender(output);
                                   break;

                              case ListItemType.EditItem:
                                   itemrow = true;
                                   this.EditItemStyle.AddAttributesToRender(output);
                                   break;

                              case ListItemType.SelectedItem:
                                   itemrow = true;
                                   this.SelectedItemStyle.AddAttributesToRender(output);
                                   break;

                              default:
                                   
                                   break;
                         }
                         if (itemrow)
                         {
                              //output.AddAttribute("onclick", "CheckBoxForRow(this, '" + ColorTranslator.ToHtml(HighlightColor) + "')");
                         }
                         if (row.Visible)
                         {
                              //row.RenderControl(output);
                              row.RenderBeginTag(output);
                              for (int i = 0; i < row.Controls.Count; i++) 
                              {
                                   TableCell cell = (TableCell) row.Controls[i];
                                   if (Columns[i].Visible && cell.Visible)
                                   {
                                        switch (row.ItemType)
                                        {
                                             case ListItemType.Header:
                                                  break;

                                             case ListItemType.Item:
                                                  Columns[i].ItemStyle.AddAttributesToRender(output);
                                                  break;

                                             case ListItemType.AlternatingItem:
                                                  Columns[i].ItemStyle.AddAttributesToRender(output);
                                                  break;

                                             case ListItemType.EditItem:
                                                  //Columns[i].AddAttributesToRender(output);
                                                  break;

                                             case ListItemType.Footer:
                                                  break;
                                        }
                                        cell.RenderControl(output);

                                        //if (row.ItemType == ListItemType.Item ||
                                        //     row.ItemType == ListItemType.AlternatingItem)
                                        //     output.RenderEndTag();
                                        //RenderControlTree(output, cell, !Columns[i].ItemStyle.Width.IsEmpty);
                                   }
                              }
                              row.RenderEndTag(output);
                         }
                    }
               }
          }
          //Panel flowPanel = new Panel();
          //if (!List.Columns[i].ItemStyle.Width.IsEmpty)
          //{
          //     flowPanel.Style["width"] = List.Columns[i].ItemStyle.Width.ToString();
          //     flowPanel.Style["height"] = "15px";
          //     flowPanel.Style["white-space"] = "nowrap";
          //     flowPanel.Style["text-overflow"] = "ellipsis";
          //     flowPanel.Style["overflow"] = "hidden";
          //     flowPanel.Style["word-break"] = "break-all";
          //     flowPanel.Style["word-wrap"] = "break-word";
          //}
//-------------------------------------------------------------------------------------------
          private void RenderControlTree(HtmlTextWriter writer, Control ctrl, bool renderDiv)
          {
               if (renderDiv)
               {
                    //writer.AddStyleAttribute("width", ctrl.Style["width"].ItemStyle.Width.ToString());
                    writer.RenderBeginTag(HtmlTextWriterTag.Div);
               }
               if (ctrl.HasControls())
               {
                    // Render Children in reverse order.
                    for (int i = ctrl.Controls.Count - 1; i >= 0; --i)
                    {
                         //RenderControlTree(writer, ctrl.Controls[i]);
                         ctrl.RenderControl(writer);
                    }
               }
               if (renderDiv)
                    writer.RenderEndTag();
          }
//-------------------------------------------------------------------------------------------
          void WeavverDataGrid_PreRender(object sender, EventArgs e)
          {
               //TemplateColumn tc = 
               //tc.
               System.Web.UI.WebControls.Table tbl = (System.Web.UI.WebControls.Table)Controls[0];
               for (int y = 0; y < tbl.Controls.Count; y++)
               {
                    DataGridItem row = (DataGridItem)tbl.Controls[y];
                    //Page.Response.Write(tbl.Controls[y].GetType().ToString());
               //}

                    

               //for (int r = 0; r < Items.Count; r++)
               //{
               //     DataGridItem row = Items[r];
                    if (row.ItemType == ListItemType.Item || row.ItemType == ListItemType.AlternatingItem)
                    {
                         for (int i = 0; i < row.Cells.Count; i++)
                         {
                              TableCell cell = (TableCell) row.Cells[i];
                              if (!cell.Width.IsEmpty)
                              {
                                   string div = "height:15px;";
                                   div += "white-space:nowrap;";
                                   div += "text-overflow:ellipsis;";
                                   div += "overflow:hidden;";
                                   div += "word-break:break-all;";
                                   div += "word-wrap:break-word;";
                                   div += "width:" + cell.Width.ToString() + ";";

                                   //for (int x = 0; x < e.Item.Cells[x].Controls.Count; x++)
                                   //{
                                   //     Page.Response.Write(e.Item.Cells[i].Controls[i].GetType().ToString());
                                   //}
                                   cell.Controls.AddAt(0, new LiteralControl("<div style='" + div + "'>" + cell.Text));
                                   cell.Controls.Add(new LiteralControl("</div>"));
                              }
                         }
                    }
               }
          }
//-------------------------------------------------------------------------------------------
     }
//-------------------------------------------------------------------------------------------
}
