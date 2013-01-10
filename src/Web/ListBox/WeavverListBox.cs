using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Drawing;

namespace Weavver.Web
{
	[ParseChildren(true, "Items")]
	[PersistChildren(false)]
	public class WeavverListBox : WebControl
	{
		private WeavverListBoxItemCollection	tslistboxitemcollection;
		private bool					usedefaultstyles;
		private string					listboxcss;
		private string					innerboxcss;
		private string					overitemcss;
		private string					selecteditemcss	= "tslistboxselected";
		private string					normalitemcss;
//-------------------------------------------------------------------------------------------
		public WeavverListBox()
		{
			UseDefaultStyles = true;
		}
//-------------------------------------------------------------------------------------------
		[Category("Weavver, Inc.")]
		public override bool Enabled
		{
			get
			{
				return base.Enabled;
			}
			set
			{
				base.Enabled = value;
			}
		}
//-------------------------------------------------------------------------------------------
		[Category("Weavver, Inc.")]
		public override Color BackColor
		{
			get
			{
				return base.BackColor;
			}
			set
			{
				base.BackColor = value;
			}
		}
//-------------------------------------------------------------------------------------------
		[Category("Weavver, Inc.")]
		[PersistenceMode(PersistenceMode.InnerDefaultProperty)]
		[DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content)]
		public WeavverListBoxItemCollection Items
		{
			get
			{
				if (tslistboxitemcollection == null)
					tslistboxitemcollection = new WeavverListBoxItemCollection();
				return tslistboxitemcollection;
			}
		}
//-------------------------------------------------------------------------------------------
		[Category("Weavver, Inc.")]
		[Description("If false the developer is responsible for rendering their own styles that correspond to the css classes defined in 'ListBoxCss,' 'InnerBoxCss,' 'OverItemCss', 'SelectedItemCss,' and 'NormalItemCss.'")]
		public bool UseDefaultStyles
		{
			get
			{
				return usedefaultstyles;
			}
			set
			{
				usedefaultstyles = value;
				if (usedefaultstyles)
					SetDefaults();
			}
		}
//-------------------------------------------------------------------------------------------
		[Category("Weavver, Inc.")]
		public string ListBoxCss
		{
			get { return listboxcss; }
			set
			{
				if (!UseDefaultStyles)
					listboxcss = value;
			}
		}
//-------------------------------------------------------------------------------------------
		[Category("Weavver, Inc.")]
		public string InnerBoxCss
		{
			get { return innerboxcss; }
			set
			{
				if (!UseDefaultStyles)
					innerboxcss = value;
			}
		}
//-------------------------------------------------------------------------------------------		
		[Category("Weavver, Inc.")]
		public string NormalItemCss
		{
			get { return normalitemcss; }
			set
			{
				if (!UseDefaultStyles)
					normalitemcss = value;
			}
		}
//-------------------------------------------------------------------------------------------
		[Category("Weavver, Inc.")]
		public string OverItemCss
		{
			get { return overitemcss; }
			set
			{
				if (!UseDefaultStyles)
					overitemcss = value;
			}
		}
//-------------------------------------------------------------------------------------------
		[Category("Weavver, Inc.")]
		public string SelectedItemCss
		{
			get { return selecteditemcss; }
			set
			{
				if (!UseDefaultStyles)
					selecteditemcss = value;
			}
		}
//-------------------------------------------------------------------------------------------
		private void SetDefaults()
		{
			listboxcss		= "tslistbox";
			innerboxcss		= "tslbinnerbox";
			overitemcss		= "tslboveritem";
			normalitemcss	     = "tslbitem";
			selecteditemcss	= "tslbselecteditem";
		}
//-------------------------------------------------------------------------------------------
		protected override void Render(HtmlTextWriter writer)
		{
			string tslistboxscript	 = "<script language=JavaScript>";
			tslistboxscript			+= "function WeavverListBoxOver(obj, over, click)		{ if (obj.className != click)	obj.className = over; }";
			tslistboxscript			+= "function WeavverListBoxOut(obj, out, click)			{ if (obj.className != click)	obj.className = out; }";
			tslistboxscript			+= "function WeavverListBoxClick(obj, over, out, click)	{ if (obj.className == click) {	obj.className = over; } else { obj.className = click;  }}";
			tslistboxscript			+= "<" + "/" + "script>";

			if (!Page.ClientScript.IsStartupScriptRegistered("WeavverListBoxScript"))
				Page.RegisterStartupScript("WeavverListBoxScript", tslistboxscript);


			if (UseDefaultStyles && !Page.IsStartupScriptRegistered("WeavverListBoxStyle"))
			{
				string listbox		= ".tslistbox {border-top: 2px inset; border-left: 2px inset; border-right: 1px groove; border-bottom-width: 1px; border-bottom-style: inset; }";
				string innerbox		= ".tslbinner {font-face: verdana;	font-size: 11px; }";
				string all			= ".tslbitem, .tslboveritem, .tslbselecteditem {color: black; cursor: hand;padding-left: 5px;padding-right: 5px;}";
				string overitem		= ".tslboveritem {background-color: gainsboro;}";
				string selecteditem      = ".tslbselecteditem {color: white;background-color: #807869;}\";";

				writer.WriteLine("<style>");
				writer.WriteLine(listbox);
				writer.WriteLine(innerbox);
				writer.WriteLine(all);
				writer.WriteLine(overitem);
				writer.WriteLine(selecteditem);
				writer.WriteLine("</style>");

				Page.ClientScript.RegisterStartupScript(this.GetType(), "WeavverListBoxStyle", "");
			}
               
               writer.AddAttribute(HtmlTextWriterAttribute.Cellpadding,    	"0");
               writer.AddAttribute(HtmlTextWriterAttribute.Cellspacing,    	"0");
               writer.AddAttribute(HtmlTextWriterAttribute.Width,			"0");
               writer.AddAttribute(HtmlTextWriterAttribute.Class,			ListBoxCss);
               Attributes.AddAttributes(writer);
               writer.RenderBeginTag(HtmlTextWriterTag.Table);
			writer.RenderBeginTag(HtmlTextWriterTag.Tr);
				writer.RenderBeginTag(HtmlTextWriterTag.Td);
                         writer.AddAttribute(HtmlTextWriterAttribute.Id, UniqueID);
					writer.AddAttribute(HtmlTextWriterAttribute.Height, "10");
					writer.AddStyleAttribute("overflow", "auto");
					writer.AddStyleAttribute("height", Height.ToString());
					writer.RenderBeginTag(HtmlTextWriterTag.Div);
                              writer.WriteLine("<script type=text/javascript>");
                              writer.WriteLine("var listbox = new Array()");
                              //for (int i = 0; i < tslistboxitemcollection.Count; i++)
                              //{
                              //     writer.WriteLine("listbox[" + i.ToString() + "] = new Array('" + tslistboxitemcollection[i].Text + "','','" + NormalItemCss + "','" + SelectedItemCss + "','" + OverItemCss + "');");
                              //}
                              writer.WriteLine("</script>");
//						writer.AddAttribute(HtmlTextWriterAttribute.Class,			InnerBoxCss);
//						writer.AddAttribute(HtmlTextWriterAttribute.Width,			Width.ToString());
//						writer.AddAttribute(HtmlTextWriterAttribute.Cellspacing,	"0");
//						writer.AddAttribute(HtmlTextWriterAttribute.Cellpadding,	"0");
//                              
//                              //writer.AddAttribute(HtmlTextWriterAttribute.Disabled,		(!Enabled).ToString());
//						writer.AddAttribute(HtmlTextWriterAttribute.Background,     ColorTranslator.ToHtml(BackColor));
//						writer.RenderBeginTag(HtmlTextWriterTag.Table);
//							for (int i = 0; i < tslistboxitemcollection.Count; i++)
//							{
//								writer.AddAttribute(HtmlTextWriterAttribute.Class,		((tslistboxitemcollection[i].Selected) ? SelectedItemCss : NormalItemCss));	
//								writer.AddAttribute("onmouseover",						"WeavverListBoxOver(this, '" +  OverItemCss + "', '" + SelectedItemCss + "')");
//								writer.AddAttribute("onmouseout",						"WeavverListBoxOut(this, '" + NormalItemCss + "', '" + SelectedItemCss + "')");
//                                        writer.AddAttribute("onclick",                              "window.location = \"" + tslistboxitemcollection[i].URL + "\"");
//                                        //writer.AddAttribute("onmousedown",						"WeavverDropDownMenuDrop(this, 'ddmcommands', 0, 15); event.cancelBubble=true; return false;");
//								writer.RenderBeginTag(HtmlTextWriterTag.Tr);
//						          //writer.AddStyleAttribute(HtmlTextWriterStyle.Color,         ColorTranslator.ToHtml(ForeColor));
//                                        writer.RenderBeginTag(HtmlTextWriterTag.Td);
//										writer.Write(tslistboxitemcollection[i].Text);
//									writer.RenderEndTag();
//								writer.RenderEndTag();
//							}
//						writer.RenderEndTag();
					writer.RenderEndTag();
				writer.RenderEndTag();
			writer.RenderEndTag();
               writer.RenderEndTag();
               writer.AddAttribute("onfocus",				     "WeavverListBoxSearchFocus(this)");
               writer.AddAttribute("onblur",				          "WeavverListBoxSearchFocusLost(this)");
               writer.AddAttribute("value",						"Search...");
               writer.AddStyleAttribute(HtmlTextWriterStyle.Width,	"100%"); //Width.ToString());
               writer.RenderBeginTag(HtmlTextWriterTag.Input);
               writer.RenderEndTag();
		}
//-------------------------------------------------------------------------------------------
	}
}