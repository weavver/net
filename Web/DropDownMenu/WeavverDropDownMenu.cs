using System;
using System.ComponentModel;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;

using Weavver.Design;

namespace Weavver.Web
{
     [ParseChildren(true, "Items")]
     [PersistChildren(false)]
     [ToolboxItem(true)]
     [Designer(typeof(WeavverDropDownMenuDesigner))]
     [Description("This is an easy to use javascript drop down menu.")]
     public class WeavverDropDownMenu : WeavverWebControl, IPostBackEventHandler
     {
          private WeavverDropDownMenuItemCollection items = new WeavverDropDownMenuItemCollection();
          private string text = string.Empty;
          private WeavverDropDownMenuType ddmtype;
          private WeavverDropDownMenuPopupType ddmptype;
          private WeavverDropDownMenuOverflowType ddmoverflowtype;
          private int paddinglefta = 0;
          private int paddingleftb = 10;
          private int xoffset = 0;
          private int yoffset = 18;
          private Unit ddmwidth = Unit.Empty;
          private Unit ddmheight = Unit.Empty;
          private bool usedefaultcss = true;
          private bool designmode = false;
          private string backgroundimageurl = string.Empty;
          private string normalcssclass = string.Empty;
          private string highlightedcssclass = string.Empty;
          private Color ddmbackcolor = Color.Empty;

          // begin changes - 22/01/2004 - Nikki Long
          private string mouseovertext = string.Empty;
          private bool iconimages = true;
          private int hidedelay = 500; // in milliseconds
          private string alt = string.Empty;
          // end changes - 22/01/2004 - Nikki Long

          public EventHandler Click;

          private Button button;
          private System.Web.UI.WebControls.Image image;
          private Label label;
          private LinkButton linkbutton;
//-------------------------------------------------------------------------------------------
          public WeavverDropDownMenu()
          {
          }
//------------------------------------------------------------------------------------------
          public override string UniqueID
          {
               get
               {
                    return base.UniqueID;
               }
          }
//------------------------------------------------------------------------------------------
          /// <summary>
          /// This method is a sort of "temporary" fix to the problem of calling the menu layers directly by their uniqueid. If not the id would render
          /// with a colon in it (":").
          /// </summary>
          public string JsUniqueID
          {
               get
               {
                    return base.UniqueID.Replace(":", "");
               }
          }
          //------------------------------------------------------------------------------------------
          [Category("Appearance")]
          [PersistenceMode(PersistenceMode.Attribute)]
          [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
          [Description("The path to the image that should be used for the background of the drop down menu.")]
          public string BackgroundImageUrl
          {
               get
               {
                    return backgroundimageurl;
               }
               set
               {
                    backgroundimageurl = value;
               }
          }
//------------------------------------------------------------------------------------------
          [Category("Appearance")]
          [PersistenceMode(PersistenceMode.Attribute)]
          [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
          public string NormalCssClass
          {
               get
               {
                    if (UseDefaultCss)
                         return "tsdropdownmenunormal";
                    else
                         return normalcssclass;
               }
               set
               {
                    normalcssclass = value;
               }
          }
//------------------------------------------------------------------------------------------
          [Category("Appearance")]
          [PersistenceMode(PersistenceMode.Attribute)]
          [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
          [Description("The x offset from the top left of the main object that is clicked. This can be used to line up the menu correctly. Negative numbers are allowed.")]
          public int XOffset
          {
               get
               {
                    return xoffset;
               }
               set
               {
                    xoffset = value;
               }
          }
//------------------------------------------------------------------------------------------
          [Category("Appearance")]
          [PersistenceMode(PersistenceMode.Attribute)]
          [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
          [Description("The y offset from the top left of the main object that is clicked. This can be used to line up the menu correctly. Negative numbers are allowed.")]
          public int YOffset
          {
               get
               {
                    return yoffset;
               }
               set
               {
                    yoffset = value;
               }
          }
//------------------------------------------------------------------------------------------
          [Category("Appearance")]
          [PersistenceMode(PersistenceMode.Attribute)]
          [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
          [Description("The style sheet class to be used for custom styles. This class applies to each item row of the drop down menu when the mouse hovers over it.")]
          public string HighlightedCssClass
          {
               get
               {
                    if (UseDefaultCss)
                         return "tsdropdownmenuhighlighted";
                    else
                         return highlightedcssclass;
               }
               set
               {
                    highlightedcssclass = value;
               }
          }
//------------------------------------------------------------------------------------------
          [Category("Appearance")]
          [PersistenceMode(PersistenceMode.Attribute)]
          [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
          [Description("There are two table cells that apply to an item row in the drop down menu. This is the padding for the first cell.")]
          public int PaddingLeftA
          {
               get
               {
                    return paddinglefta;
               }
               set
               {
                    paddinglefta = value;
               }
          }
//------------------------------------------------------------------------------------------
          [Category("Appearance")]
          [PersistenceMode(PersistenceMode.Attribute)]
          [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
          [Description("There are two table cells that apply to an item row in the drop down menu. This is the padding for the second cell.")]
          public int PaddingLeftB
          {
               get
               {
                    return paddingleftb;
               }
               set
               {
                    paddingleftb = value;
               }
          }
//------------------------------------------------------------------------------------------
          [Category("Appearance")]
          [PersistenceMode(PersistenceMode.Attribute)]
          [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
          [Description("Set this property to false if you want the menu to not render it's own css classes. You must change this to false first before setting a custom normalcssclass or highlightcssclass.")]
          public bool UseDefaultCss
          {
               get
               {
                    return usedefaultcss;
               }
               set
               {
                    usedefaultcss = value;
               }
          }
//------------------------------------------------------------------------------------------
          [Category("Appearance")]
          [Browsable(false)]
          [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
          [Description("This property will disable full rendering of the menu during design time so that it may be displayed correctly.")]
          public bool DesignMode
          {
               get
               {
                    return designmode;
               }
               set
               {
                    designmode = value;
               }
          }
//------------------------------------------------------------------------------------------
          [Category("Weavver, Inc.")]
          [PersistenceMode(PersistenceMode.Attribute)]
          [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
          [Description("The type of object that will be rendered as the base object to click. Possible types are image, button, link, and label. If the object is to be an image then the imageurl must be placed in the control's Text property.")]
          public WeavverDropDownMenuType Type
          {
               get
               {
                    return ddmtype;
               }
               set
               {
                    ddmtype = value;
               }
          }
//------------------------------------------------------------------------------------------
          [Category("Weavver, Inc.")]
          [PersistenceMode(PersistenceMode.Attribute)]
          [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
          [Description("You may choose 'click' or 'mouseover' as the trigger action for the popup menu.")]
          public WeavverDropDownMenuPopupType PopupType
          {
               get
               {
                    return ddmptype;
               }
               set
               {
                    ddmptype = value;
               }
          }
//-------------------------------------------------------------------------------------------
          [Category("Weavver, Inc.")]
          [PersistenceMode(PersistenceMode.Attribute)]
          [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
          [Description("This is the hard coded width of the menu that drops down. If you want it to automatically size itself leave this blank.")]
          public Unit DropDownMenuWidth
          {
               get
               {
                    return ddmwidth;
               }
               set
               {
                    ddmwidth = value;
               }
          }
//-------------------------------------------------------------------------------------------
          [Category("Weavver, Inc.")]
          [PersistenceMode(PersistenceMode.Attribute)]
          [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
          [Description("This is the hard coded height of the menu that drops down. If you want it to automatically size itself leave this blank.")]
          public Unit DropDownMenuHeight
          {
               get
               {
                    return ddmheight;
               }
               set
               {
                    ddmheight = value;
               }
          }
//-------------------------------------------------------------------------------------------
          [Category("Weavver, Inc.")]
          [PersistenceMode(PersistenceMode.Attribute)]
          [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
          [Description("If the drop down menu's height or width is hard coded then over flow may occur. Use this setting to tell the browser how to handle it.")]
          public WeavverDropDownMenuOverflowType DropDownMenuOverflowType
          {
               get
               {
                    return ddmoverflowtype;
               }
               set
               {
                    ddmoverflowtype = value;
               }
          }
//------------------------------------------------------------------------------------------
          [Category("Weavver, Inc.")]
          [PersistenceMode(PersistenceMode.Attribute)]
          [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
          [Description("This is the color that will be used for the drop down menu's background if no background image is specified.")]
          public Color DropDownMenuBackColor
          {
               get
               {
                    return ddmbackcolor;
               }
               set
               {
                    ddmbackcolor = value;
               }
          }
//-------------------------------------------------------------------------------------------
          //		[Category("Weavver, Inc.")]
          //		[PersistenceMode(PersistenceMode.Attribute)]
          //		[DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
          //		[Description("This is the text that will be used for the link, button, or label types. If the DropDownMenuType is an image then the image url must be placed here.")]
          //		public override string Text
          //		{
          //			get
          //			{
          //				return text;
          //			}
          //			set
          //			{
          //				text = value;
          //			}
          //		}
//------------------------------------------------------------------------------------------
          [Category("Weavver, Inc.")]
          [PersistenceMode(PersistenceMode.Attribute)]
          [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
          [Description("Alternate text to display when user moves the mouse cursor over the menu item.")]
          public string MouseOverText
          {
               get
               {
                    return mouseovertext;
               }
               set
               {
                    mouseovertext = value;
               }
          }
//------------------------------------------------------------------------------------------
          [Category("Appearance")]
          [PersistenceMode(PersistenceMode.Attribute)]
          [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
          [Description("Set this property to false if you want the menu to not render any icon images on the left hand side.")]
          public bool IconImages
          {
               get
               {
                    return iconimages;
               }
               set
               {
                    iconimages = value;
               }
          }
//------------------------------------------------------------------------------------------
          [Category("Weavver, Inc.")]
          [PersistenceMode(PersistenceMode.Attribute)]
          [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
          [Description("Sets the drop down menu delay before hiding it.")]
          public int HideDelay
          {
               get
               {
                    return hidedelay;
               }
               set
               {
                    hidedelay = value;
               }
          }
//-------------------------------------------------------------------------------------------
          [Category("Weavver, Inc.")]
          [PersistenceMode(PersistenceMode.InnerDefaultProperty)]
          [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content)]
          [Description("This is the item collection that will be used to render the drop down menu's content.")]
          public WeavverDropDownMenuItemCollection Items
          {
               get
               {
                    return items;
               }
          }
//-------------------------------------------------------------------------------------------
          protected override void AddAttributesToRender(HtmlTextWriter writer)
          {
               writer.AddAttribute("border", "0");
               writer.AddAttribute("style", "z-index: 1000;");

               WebControl control;
               if (CssClass != "")
               {
                    writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClass);
               }

               writer.AddStyleAttribute("cursor", "hand");
               switch (ddmtype)
               {
                    case WeavverDropDownMenuType.Button:
                         button = new Button();
                         button.Text = this.Text;
                         control = button;
                         break;

                    case WeavverDropDownMenuType.Image:
                         image = new System.Web.UI.WebControls.Image();
                         image.ImageUrl = this.Text;
                         control = image;
                         break;

                    case WeavverDropDownMenuType.Label:
                         label = new Label();
                         label.Text = this.Text;
                         control = label;
                         break;

                    case WeavverDropDownMenuType.Link:
                         linkbutton = new LinkButton();
                         linkbutton.Text = this.Text;
                         linkbutton.Attributes.Add("href", "#");
                         control = linkbutton;
                         break;

                    default:
                         return;
               }
               control.Font.CopyFrom(this.Font);
               control.CopyBaseAttributes(this);

               control.Attributes.Add("id", ClientID);
               if (ToolTip != "")
                    control.Attributes.Add("title", ToolTip);

               if (Enabled)
               {
                    control.Attributes.Add("onclick", "WeavverDropDownMenuDrop(this, '" + this.JsUniqueID + "', " + XOffset + ", " + YOffset + "); event.cancelBubble=true; return false;");

                    string MouseOver = "";
                    string MouseOut = "";
                    if (MouseOverText != "")
                    {
                         switch (Type)
                         {
                              case WeavverDropDownMenuType.Image:
                                   MouseOver = "WeavverDropDownMenuSetImage(this, '" + MouseOverText + "');";
                                   MouseOut = "WeavverDropDownMenuSetImage(this, '" + Text + "');";
                                   break;

                              case WeavverDropDownMenuType.Label:
                                   MouseOver = "WeavverDropDownMenuSetText(this, '" + MouseOverText + "');";
                                   MouseOut = "WeavverDropDownMenuSetText(this, '" + Text + "');";
                                   break;
                         }
                    }

                    if (this.PopupType == WeavverDropDownMenuPopupType.MouseOver)
                    {
                         MouseOver += "WeavverDropDownMenuDrop(this, '" + this.JsUniqueID + "', " + XOffset + ", " + YOffset + "); event.cancelBubble=true; return false;";
                         MouseOut += "return WeavverDropDownMenuHideDelay(" + HideDelay.ToString() + ");";
                    }

                    if (MouseOverText != "" || PopupType == WeavverDropDownMenuPopupType.MouseOver)
                    {
                         control.Attributes.Add("onmouseover", MouseOver);
                         control.Attributes.Add("onmouseout", MouseOut);
                    }
               }
               else
               {
                    control.Attributes.Add("disabled", "disabled");
               }
          }
//-------------------------------------------------------------------------------------------
          protected override HtmlTextWriterTag TagKey
          {
               get
               {
                    return base.TagKey;
                    //				switch (ddmtype)
                    //				{
                    //					case WeavverDropDownMenuType.Button:
                    //						return HtmlTextWriterTag.Input;
                    //
                    //					case WeavverDropDownMenuType.Label:
                    //						return HtmlTextWriterTag.Span;
                    //
                    //					case WeavverDropDownMenuType.Image:
                    //						return HtmlTextWriterTag.Img;
                    //
                    //					default:
                    //						return HtmlTextWriterTag.A;
                    //				}
               }
          }
//-------------------------------------------------------------------------------------------
          protected override void RenderContents(HtmlTextWriter writer)
          {
               switch (ddmtype)
               {
                    case WeavverDropDownMenuType.Image:
                         image.RenderControl(writer);
                         break;

                    case WeavverDropDownMenuType.Link:
                         linkbutton.RenderControl(writer);
                         break;

                    case WeavverDropDownMenuType.Label:
                         label.RenderControl(writer);
                         break;

                    case WeavverDropDownMenuType.Button:
                         button.RenderControl(writer);
                         break;
               }
          }
//-------------------------------------------------------------------------------------------
          public override void RenderEndTag(HtmlTextWriter writer)
          {
               base.RenderEndTag(writer);

               if (DesignMode)
                    return;

               if (!Enabled)
                    return;

               if (UseDefaultCss)
               {
                    string x = "<style>.tsdropdownmenuhighlighted { border: 1px solid; background-color:	#A4AEA5; cursor: \"hand\";	}\r\n";
                    x += ".tsdropdownmenunormal { cursor: \"hand\"; background-color:	\"\"; }</style>";
                    if (!Page.IsClientScriptBlockRegistered("WeavverDropDownMenuHighlighted"))
                         Page.RegisterClientScriptBlock("WeavverDropDownMenuHighlighted", "");
                    writer.WriteLine(x);
               }

               writer.AddAttribute(HtmlTextWriterAttribute.Id, JsUniqueID);
               writer.AddStyleAttribute("position", "absolute");
               writer.AddStyleAttribute("left", "0");
               writer.AddStyleAttribute("top", "0");
               writer.AddStyleAttribute("layer-background-color", "white");
               writer.AddStyleAttribute("background-color", "white");
               if (DropDownMenuWidth != Unit.Empty)
                    writer.AddStyleAttribute("width", DropDownMenuWidth.ToString());
               if (DropDownMenuHeight != Unit.Empty)
                    writer.AddStyleAttribute("height", DropDownMenuHeight.ToString());
               writer.AddStyleAttribute("visibility", "hidden");
               writer.AddStyleAttribute("border", "1px solid black");
               writer.AddStyleAttribute("padding", "0px");
               writer.AddStyleAttribute("cursor", "0px");
               writer.AddStyleAttribute("overflow", WeavverDropDownMenuOverflow.ConvertTypeToString(DropDownMenuOverflowType));
               writer.RenderBeginTag("div");
               writer.AddAttribute(HtmlTextWriterAttribute.Cellpadding, "0");
               writer.AddAttribute(HtmlTextWriterAttribute.Cellspacing, "0");
               writer.AddAttribute(HtmlTextWriterAttribute.Width, "100%");
               if (DropDownMenuBackColor != Color.Empty)
                    writer.AddAttribute("bgcolor", ColorTranslator.ToHtml(DropDownMenuBackColor));
               if (BackgroundImageUrl != "")
                    writer.AddAttribute("background", BackgroundImageUrl);
               writer.RenderBeginTag("table");
               for (int i = 0; i < Items.Count; i++)
               {
                    if (Items[i].HighlightedCssClass == "")
                         writer.AddAttribute("onmouseover", "WeavverDropDownMenuHighlight(this, '" + HighlightedCssClass + "')");
                    else
                         writer.AddAttribute("onmouseover", "WeavverDropDownMenuHighlight(this, '" + Items[i].HighlightedCssClass + "')");

                    if (Items[i].NormalCssClass == "")
                         writer.AddAttribute("onmouseout", "WeavverDropDownMenuHighlight(this, '" + NormalCssClass + "')");
                    else
                         writer.AddAttribute("onmouseout", "WeavverDropDownMenuHighlight(this, '" + Items[i].HighlightedCssClass + "')");

                    if (Items[i].CustomOnClick != "")
                         writer.AddAttribute("OnClick", Items[i].CustomOnClick);
                    else
                    {
                         string clientjs = Page.ClientScript.GetPostBackEventReference(this, i.ToString());
                         writer.AddAttribute("OnClick", clientjs);
                    }

                    if (Items[i].NormalCssClass != "")
                         writer.AddAttribute("class", Items[i].NormalCssClass);
                    else
                         writer.AddAttribute("class", NormalCssClass);

                    writer.AddAttribute("title", Items[i].ToolTip);
                    writer.RenderBeginTag("tr");
                    if (IconImages)
                    {
                         writer.AddAttribute("id", UniqueID + "c" + i.ToString());
                         writer.AddAttribute("width", "0");
                         writer.AddStyleAttribute("padding-left", PaddingLeftA.ToString());
                         writer.RenderBeginTag("td");
                         if (Items[i].IconImageUrl != "")
                              writer.Write("<img src=\"" + Items[i].IconImageUrl + "\">");
                         else
                              writer.Write("&nbsp;");
                         writer.RenderEndTag();
                         writer.AddAttribute("width", "100%");
                         writer.AddStyleAttribute("padding-left", PaddingLeftB.ToString());
                         if (Items[i].MouseOverText != "")
                         {
                              writer.AddAttribute("onmouseover", "return WeavverDropDownMenuSetText(this, '" + Items[i].MouseOverText + "');");
                              writer.AddAttribute("onmouseout", "return WeavverDropDownMenuSetText(this, '" + Items[i].Text + "');");
                         }
                    }
                    writer.RenderBeginTag("td");
                    writer.Write(Items[i].Text);
                    writer.RenderEndTag();
                    writer.RenderEndTag();
               }
               writer.RenderEndTag();
               writer.RenderEndTag();

               if (!Page.ClientScript.IsClientScriptBlockRegistered("WeavverDropDownMenu"))
               {
                    Page.RegisterClientScriptBlock("WeavverDropDownMenu", "");
                    writer.Write("<script language=JavaScript src=\"~/scripts/WeavverDropDownMenu.js\"></script>");
               }
          }
//-------------------------------------------------------------------------------------------
          /// <summary>
          /// Not being used.
          /// </summary>
          /// <param name="e"></param>
          protected virtual void OnClick(EventArgs e)
          {
          }
//-------------------------------------------------------------------------------------------
          /// <summary>
          /// This method is called by the server when a post back event occurs for the control.
          /// </summary>
          /// <param name="eventArgument">The argument that the server passes the method. This argument is taken fromt the javascript post back event.</param>
          public void RaisePostBackEvent(string eventArgument)
          {
               if (Click != null)
               {
                    for (int i = 0; i < Items.Count; i++)
                    {
                         if (i == Int32.Parse(eventArgument))
                              Click(Items[i], EventArgs.Empty);
                    }

               }
          }
//-------------------------------------------------------------------------------------------
     }
}
