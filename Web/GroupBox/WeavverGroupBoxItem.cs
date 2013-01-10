using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Weavver.Web
{
	[ToolboxItem(false)]
	[DefaultProperty("Text")]
	public class WeavverGroupBoxItem
	{
		private string normalcssclass		= "tsgroupboxitem";
		private string highlightedcssclass	= string.Empty;
		private string customonclick		= string.Empty;
		private string iconimageurl			= string.Empty;
		private string text					= string.Empty;
		private string tooltip				= string.Empty;
		private	string mouseovertext		= string.Empty;
//-------------------------------------------------------------------------------------------
		[Category("Weavver, Inc.")]
		[PersistenceMode(PersistenceMode.Attribute)]
		[DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
		[Description("This class can be used to override the default class used for this item when it is not being highlighted.")]
		public string NormalCssClass
		{
			get
			{
				return normalcssclass;
			}
			set
			{
				normalcssclass = value;
			}
		}
//------------------------------------------------------------------------------------------
		[Category("Weavver, Inc.")]
		[PersistenceMode(PersistenceMode.Attribute)]
		[DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
		[Description("This class can be used to override the default class used for this item when it is being highlighted.")]
		public string HighlightedCssClass
		{
			get
			{
				return highlightedcssclass;
			}
			set
			{
				highlightedcssclass = value;
			}
		}
//-------------------------------------------------------------------------------------------
		[Category("Weavver, Inc.")]
		[PersistenceMode(PersistenceMode.Attribute)]
		[DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
		[Description("Instead of using the default behavior of onclick for this item you can override it with your own javascript. For example: document.location = 'http://www.titaniumsoft.net';")]
		public string CustomOnClick
		{
			get
			{
				return customonclick;
			}
			set
			{
				customonclick = value;
			}
		}
//-------------------------------------------------------------------------------------------
		[Category("Weavver, Inc.")]
		[PersistenceMode(PersistenceMode.Attribute)]
		[DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
		public string Text
		{
			get
			{
				return text;
			}
			set
			{
				text = value;
			}
		}
//-------------------------------------------------------------------------------------------
		[Category("Weavver, Inc.")]
		[PersistenceMode(PersistenceMode.Attribute)]
		[DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
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
//-------------------------------------------------------------------------------------------
		[Category("Weavver, Inc.")]
		[PersistenceMode(PersistenceMode.Attribute)]
		[DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
		public string IconImageUrl
		{
			get
			{
				return iconimageurl;
			}
			set
			{
				iconimageurl = value;
			}
		}
//-------------------------------------------------------------------------------------------
		[Category("Weavver, Inc.")]
		[PersistenceMode(PersistenceMode.Attribute)]
		[DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
		public string ToolTip
		{
			get
			{
				return tooltip;
			}
			set
			{
				tooltip = value;
			}
		}
//------------------------------------------------------------------------------------------
		public override string ToString()
		{
			return (Text == "") ? "[WeavverDropDownMenuItem]" : text;
		}
//-------------------------------------------------------------------------------------------	
	}
}
