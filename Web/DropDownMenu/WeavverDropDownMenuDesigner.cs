using System.Web.UI.Design;
using System.ComponentModel;

using Weavver.Web;

namespace Weavver.Design
{
	public class WeavverDropDownMenuDesigner : ControlDesigner
	{
//-------------------------------------------------------------------------------------------
		protected override string GetEmptyDesignTimeHtml() 
		{
			return CreatePlaceHolderDesignTimeHtml("The menu text is empty therefore this control will not render.");
		}
//-------------------------------------------------------------------------------------------
		public override string GetDesignTimeHtml()
		{
			((WeavverDropDownMenu) Component).DesignMode = true;
			string html = base.GetDesignTimeHtml();
			((WeavverDropDownMenu) Component).DesignMode = false;
			return html;
		}
//-------------------------------------------------------------------------------------------
	}
}
