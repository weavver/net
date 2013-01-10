using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;

using System.Web.UI;
using System.Web.UI.WebControls;

namespace Weavver.Web
{
	public class WeavverTabControlTab : Control, INamingContainer
	{
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public string Text
		{
			get
			{
				return (ViewState["text"] != null) ? ViewState["text"].ToString() : "";
			}
			set
			{
				ViewState["text"] = value;
			}
		}
	}
}
