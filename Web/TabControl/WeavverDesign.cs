using System;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.Design;
using System.Web.UI.Design.WebControls;
using System.Threading;

using System.ComponentModel;

namespace Weavver.Web
{
	public class WeavverDropDownMenuDesaigner : ControlDesigner // : /*TemplatedControlDesigner */ReadWriteControlDesigner
	{
//
//		public SimpleDesigner() 
//		{
//		}

//		protected override string GetEmptyDesignTimeHtml() 
//		{
//			
//			// when blank, render design time warning
//			string text="Not visible at runtime with current settings";
//			return CreatePlaceHolderDesignTimeHtml(text);
//		}

		public override bool AllowResize
		{
			get
			{
				return true;
			}
		}
        
//		protected override TemplateEditingVerb[] GetCachedTemplateEditingVerbs()
//		{
//			return null;
//		}
//
//		protected override ITemplateEditingFrame CreateTemplateEditingFrame(TemplateEditingVerb verb)
//		{
//			return null;
//		}
//
//		public override string GetTemplateContent(ITemplateEditingFrame editingFrame, string templateName, out bool allowEditing)
//		{
//			allowEditing = false;
//			return null;
//		}
//        
//		public override void SetTemplateContent(ITemplateEditingFrame editingFrame, string templateName, string templateContent)
//		{
//
//		}

		public override string GetDesignTimeHtml()
		{
			return "asdf: " + base.GetDesignTimeHtml();
		}

		protected override string GetEmptyDesignTimeHtml()
		{
			return base.GetEmptyDesignTimeHtml ();
		}



	}
}
