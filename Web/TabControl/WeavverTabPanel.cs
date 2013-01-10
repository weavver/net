using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Weavver.Web
{
	[ToolboxItem(false)]
	public class WeavverTabPanel : Panel, INamingContainer
	{
		private string _caption;
		private string _info;

		public WeavverTabPanel()
		{
			_caption = "none";
			_info = "none";
		}

		internal WeavverTabPanel(string caption, string info)
		{
			_caption = caption;
			_info = info;
		}

		public string Caption
		{
			get	{ return _caption;}
		}
		
		public string Info
		{
			get	{ return _info;}
		}
	}
}
