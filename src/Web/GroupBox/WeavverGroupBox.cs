using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;

namespace Weavver.Web
{
	[ToolboxItem(true)]
	public class WeavverGroupBox : WeavverWebControl
	{
		private WeavverGroupBoxItemCollection	items				= new WeavverGroupBoxItemCollection();
//-------------------------------------------------------------------------------------------
          public WeavverGroupBox()
		{

		}
//-------------------------------------------------------------------------------------------
		[Category("Weavver, Inc.")]
		[PersistenceMode(PersistenceMode.InnerDefaultProperty)]
		[DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content)]
		[Description("This is the item collection that will be used to render the group box content.")]
		public WeavverGroupBoxItemCollection Items
		{
			get
			{
				return items;
			}
		}
//-------------------------------------------------------------------------------------------
		protected override void Render(HtmlTextWriter writer)
		{
			if (!Visible)
			{
				return;
			}
			writer.AddStyleAttribute(HtmlTextWriterStyle.BorderStyle,	BorderStyle.ToString());
			writer.AddStyleAttribute(HtmlTextWriterStyle.BorderWidth,	BorderWidth.ToString());
			writer.AddAttribute(HtmlTextWriterAttribute.Width,			Width.ToString());
			writer.AddAttribute(HtmlTextWriterAttribute.Href,			Height.ToString());
			writer.AddAttribute(HtmlTextWriterAttribute.Cellpadding,	"0");
			writer.AddAttribute(HtmlTextWriterAttribute.Cellspacing,	"0");
			writer.AddAttribute(HtmlTextWriterAttribute.Align,			"center");
			writer.RenderBeginTag("table");
				writer.AddAttribute(HtmlTextWriterAttribute.Class,	"groupboxtitle");
				writer.RenderBeginTag("tr");
					writer.AddAttribute(HtmlTextWriterAttribute.Background,	"images/group-box-bar.gif");
					writer.RenderBeginTag("td");
						writer.Write(Text);
					writer.RenderEndTag();
				writer.RenderEndTag();
				for (int i = 0; i < Items.Count; i++)
				{
					RenderItemRow(writer, Items[i]);
				}
			writer.RenderEndTag();
		}
//-------------------------------------------------------------------------------------------
		private void RenderItemRow(HtmlTextWriter writer, WeavverGroupBoxItem item)
		{
			if (!Enabled)
			{
				writer.AddAttribute(HtmlTextWriterAttribute.Disabled, "true");
			}
			writer.RenderBeginTag("tr");
				if (Enabled)
				{
					writer.AddAttribute("onclick",							item.CustomOnClick);
					writer.AddAttribute("onmouseover",						"highlight(this, 'groupboxitemover');");
					writer.AddAttribute("onmouseout",						"highlight(this, '" + item.NormalCssClass + "');");
				}
				writer.AddAttribute(HtmlTextWriterAttribute.Class,		item.NormalCssClass);
				writer.RenderBeginTag("td");					
					writer.Write("&nbsp;" + item.Text);
				writer.RenderEndTag();
			writer.RenderEndTag();
		}
//-------------------------------------------------------------------------------------------
	}
}
