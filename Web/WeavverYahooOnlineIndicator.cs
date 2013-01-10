using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;

using TitaniumSoft;

namespace Weavver.Web
{
	/// /// THIS CONTROL IS UNFINISHED AND NOT WORKING
	/// /// THIS CONTROL IS UNFINISHED AND NOT WORKING
	/// /// THIS CONTROL IS UNFINISHED AND NOT WORKING
	/// /// THIS CONTROL IS UNFINISHED AND NOT WORKING
	/// /// THIS CONTROL IS UNFINISHED AND NOT WORKING
	/// /// THIS CONTROL IS UNFINISHED AND NOT WORKING
	/// /// THIS CONTROL IS UNFINISHED AND NOT WORKING
	/// /// THIS CONTROL IS UNFINISHED AND NOT WORKING
	/// 
	/// <summary>
	/// This control will generate the html required to show whether or not a yahoo user is online or not. You can not
	/// use this control server side to determine if a user is online.
	/// </summary>
	[PersistChildren(false)]
	[ToolboxItem(true)]
     public class WeavverYahooOnlineIndicator : WeavverWebControl
	{
		private string	username;
//-------------------------------------------------------------------------------------------
		/// <summary>
		/// This is the yahoo user name of the person you want to see the status of.
		/// </summary>
		[Category("Weavver, Inc.")]
		[PersistenceMode(PersistenceMode.Attribute)]
		[DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
		public string UserName
		{
			get
			{
				return username;
			}
			set
			{
				username = value;
			}
		}
//-------------------------------------------------------------------------------------------
		protected override HtmlTextWriterTag TagKey
		{
			get
			{
				return HtmlTextWriterTag.Img;
			}
		}
//-------------------------------------------------------------------------------------------
		protected override void AddAttributesToRender(HtmlTextWriter writer)
		{
			base.AddAttributesToRender(writer);
			writer.AddAttribute		(HtmlTextWriterAttribute.Src, "http://mail.opi.yahoo.com/online?u=" + UserName + "&m=g&t=0");
		}
//-------------------------------------------------------------------------------------------
		public override void RenderBeginTag(HtmlTextWriter writer)
		{
			base.RenderBeginTag(writer);
		}

	}
}

