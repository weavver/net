using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Weavver.Web
{
	[ToolboxItem(false)]
	[DefaultProperty("Text")]
	public class WeavverListBoxItem : Control
	{
		private string	text;
          private string	url;
		private bool	selected;
//-------------------------------------------------------------------------------------------
		public WeavverListBoxItem()
		{
		}
//-------------------------------------------------------------------------------------------
		[PersistenceMode(PersistenceMode.Attribute)]
		[DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
		public bool Selected
		{
			get
			{
				return selected;
			}
			set
			{
				selected = value;
			}
		}
//-------------------------------------------------------------------------------------------
		[PersistenceMode(PersistenceMode.Attribute)]
		[DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
		public string Text
		{
			get
			{
				if (text == null)
					text = "[WeavverListBoxItem]";
				return text;
			}
			set
			{
				text = value;
			}
		}
//-------------------------------------------------------------------------------------------
          [PersistenceMode(PersistenceMode.Attribute)]
          [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
          public string URL
          {
               get
               {
                    return url;
               }
               set
               {
                    url = value;
               }
          }
//-------------------------------------------------------------------------------------------
		public override string ToString()
		{
			return (text == "") ? "[WeavverListBoxItem]" : text;
		}
//-------------------------------------------------------------------------------------------
	}	
}