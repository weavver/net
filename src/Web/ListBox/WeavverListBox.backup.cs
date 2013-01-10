using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Drawing;

namespace TitaniumSoft.WebControls
{
	[ParseChildren(true, "Items")]
	[PersistChildren(false)]
	public class TSListBox : WebControl
	{
		private TSListBoxItemCollection	tslistboxitemcollection;
		private bool					usedefaultstyles;
		private string					listboxcss;
		private string					innerboxcss;
		private string					overitemcss;
		private string					selecteditemcss	= "tslistboxselected";
		private string					normalitemcss;
//-------------------------------------------------------------------------------------------
		public TSListBox()
		{
			UseDefaultStyles = true;
		}
//-------------------------------------------------------------------------------------------
		[Category("Titanium Soft.")]
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
		[Category("Titanium Soft.")]
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
		[Category("Titanium Soft.")]
		[PersistenceMode(PersistenceMode.InnerDefaultProperty)]
		[DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content)]
		public TSListBoxItemCollection Items
		{
			get
			{
				if (tslistboxitemcollection == null)
					tslistboxitemcollection = new TSListBoxItemCollection();
				return tslistboxitemcollection;
			}
		}
//-------------------------------------------------------------------------------------------
		[Category("Titanium Soft.")]
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
		[Category("Titanium Soft.")]
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
		[Category("Titanium Soft.")]
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
		[Category("Titanium Soft.")]
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
		[Category("Titanium Soft.")]
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
		[Category("Titanium Soft.")]
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
			tslistboxscript			+= "function TSListBoxOver(obj, over, click)		{ if (obj.className != click)	obj.className = over; }";
			tslistboxscript			+= "function TSListBoxOut(obj, out, click)			{ if (obj.className != click)	obj.className = out; }";
			tslistboxscript			+= "function TSListBoxClick(obj, over, out, click)	{ if (obj.className == click) {	obj.className = over; } else { obj.className = click;  }}";
			tslistboxscript			+= "<" + "/" + "script>";

			if (!Page.IsStartupScriptRegistered("TSListBoxScript"))
				Page.RegisterStartupScript("TSListBoxScript", tslistboxscript);


			if (UseDefaultStyles && !Page.IsStartupScriptRegistered("TSListBoxStyle"))
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

				Page.RegisterStartupScript("TSListBoxStyle", "");
			}
               
               writer.AddAttribute(HtmlTextWriterAttribute.Cellpadding,    	"0");
               writer.AddAttribute(HtmlTextWriterAttribute.Cellspacing,    	"0");
               writer.AddAttribute(HtmlTextWriterAttribute.Width,			"0");
               writer.AddAttribute(HtmlTextWriterAttribute.Class,			ListBoxCss);
               Attributes.AddAttributes(writer);
               writer.RenderBeginTag(HtmlTextWriterTag.Table);
			writer.RenderBeginTag(HtmlTextWriterTag.Tr);
				writer.RenderBeginTag(HtmlTextWriterTag.Td);
                         writer.AddAttribute(HtmlTextWriterAttribute.Id,     UniqueID);
					writer.AddAttribute(HtmlTextWriterAttribute.Height, "10");
					writer.AddStyleAttribute("overflow",                "auto");
					writer.AddStyleAttribute("height",                  Height.ToString());
					writer.RenderBeginTag(HtmlTextWriterTag.Div);
						writer.AddAttribute(HtmlTextWriterAttribute.Class,			InnerBoxCss);
						writer.AddAttribute(HtmlTextWriterAttribute.Width,			Width.ToString());
						writer.AddAttribute(HtmlTextWriterAttribute.Cellspacing,	"0");
						writer.AddAttribute(HtmlTextWriterAttribute.Cellpadding,	"0");
//                              
//                              //writer.AddAttribute(HtmlTextWriterAttribute.Disabled,		(!Enabled).ToString());
//						writer.AddAttribute(HtmlTextWriterAttribute.Background,     ColorTranslator.ToHtml(BackColor));
//						writer.RenderBeginTag(HtmlTextWriterTag.Table);
                                   writer.Write("<script language=javascript> var listbox = Array();\r\n");
                                   for (int i = 0; i < tslistboxitemcollection.Count; i++)
                                   {
                                        writer.Write("listbox[" + i.ToString() + "] = new Array(\"" + tslistboxitemcollection[i].Text + "\",\"" + tslistboxitemcollection[i].Selected.ToString() + "\",\"" + NormalItemCss + "\",\"" + SelectedItemCss + "\",\"" + OverItemCss + "\");\r\n");
//                                      writer.Write("$listbox[0][1] = " + tslistboxitemcollection[i].Text);
//                                      writer.AddAttribute(HtmlTextWriterAttribute.Class,		((tslistboxitemcollection[i].Selected) ? SelectedItemCss : NormalItemCss));	
//                                      writer.AddAttribute("onmouseover",						"TSListBoxOver(this, '" +  OverItemCss + "', '" + SelectedItemCss + "')");
//                                      writer.AddAttribute("onmouseout",						"TSListBoxOut(this, '" + NormalItemCss + "', '" + SelectedItemCss + "')");
//                                      writer.AddAttribute("onclick",                              "window.location = \"" + tslistboxitemcollection[i].URL + "\"");
//                                      //writer.AddAttribute("onmousedown",						"TSDropDownMenuDrop(this, 'ddmcommands', 0, 15); event.cancelBubble=true; return false;");
//                                      writer.RenderBeginTag(HtmlTextWriterTag.Tr);
//                                      //writer.AddStyleAttribute(HtmlTextWriterStyle.Color,         ColorTranslator.ToHtml(ForeColor));
//                                      writer.RenderBeginTag(HtmlTextWriterTag.Td);
//                                      writer.Write(tslistboxitemcollection[i].Text);
//                                      writer.RenderEndTag();
//                                      writer.RenderEndTag();
                                   }
                                   writer.Write("</script>");
//							for (int i = 0; i < tslistboxitemcollection.Count; i++)
//							{
//								writer.AddAttribute(HtmlTextWriterAttribute.Class,		((tslistboxitemcollection[i].Selected) ? SelectedItemCss : NormalItemCss));	
//								writer.AddAttribute("onmouseover",						"TSListBoxOver(this, '" +  OverItemCss + "', '" + SelectedItemCss + "')");
//								writer.AddAttribute("onmouseout",						"TSListBoxOut(this, '" + NormalItemCss + "', '" + SelectedItemCss + "')");
//                                        writer.AddAttribute("onclick",                              "window.location = \"" + tslistboxitemcollection[i].URL + "\"");
//                                        //writer.AddAttribute("onmousedown",						"TSDropDownMenuDrop(this, 'ddmcommands', 0, 15); event.cancelBubble=true; return false;");
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
               writer.AddAttribute("onfocus",				     "TSListBoxSearchFocus(this)");
               writer.AddAttribute("onblur",				          "TSListBoxSearchFocusLost(this)");
               writer.AddAttribute("value",						"Search...");
               writer.AddStyleAttribute(HtmlTextWriterStyle.Width,	"100%"); //Width.ToString());
               writer.RenderBeginTag(HtmlTextWriterTag.Input);
               writer.RenderEndTag();
		}
//-------------------------------------------------------------------------------------------
	}
}