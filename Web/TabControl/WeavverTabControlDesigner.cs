using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;

using System.Windows.Forms;
using System.Web.UI;
using System.Web.UI.Design;
using System.Web.UI.WebControls;

namespace Weavver.Web
{
	public class TabControlDesigner : TemplatedControlDesigner
	{
		DesignerVerb ShowNext;
//-------------------------------------------------------------------------------------------
		private TemplateEditingVerb[] _templateEditingVerbs;
		int	index = -1;
//-------------------------------------------------------------------------------------------
		public TabControlDesigner()
		{
			ShowNext			= new DesignerVerb("Next Tab", new EventHandler(asdf));
			base.Verbs.Add(ShowNext);
		}
//-------------------------------------------------------------------------------------------
		private void asdf(object ob, EventArgs e)
		{

		}
//-------------------------------------------------------------------------------------------
		#region Design-time HTML
		public override string GetDesignTimeHtml()
		{
			WeavverTabControl control = (WeavverTabControl) Component;

			if (control.Tabs[control.SelectedIndex] == null)
			{
				return GetEmptyDesignTimeHtml();
			}

			string designTimeHTML = String.Empty;
			try
			{
				control.DataBind();
				designTimeHTML = base.GetDesignTimeHtml();
			}
			catch (Exception e)
			{
				designTimeHTML = GetErrorDesignTimeHtml(e);
			}
			return designTimeHTML;
		}

		protected override string GetEmptyDesignTimeHtml()
		{
			return CreatePlaceHolderDesignTimeHtml("Right-click to edit the template. - Database Works, Inc.");
		}
		#endregion Design-time HTML
//-------------------------------------------------------------------------------------------
          [Obsolete]
		protected override TemplateEditingVerb[] GetCachedTemplateEditingVerbs()
		{
			if (_templateEditingVerbs == null)
			{
				WeavverTabControl tc = (WeavverTabControl) Component;
				_templateEditingVerbs = new TemplateEditingVerb[tc.Tabs.Count];
	
				for (int i = 0; i < tc.Tabs.Count; i++)
				{
					_templateEditingVerbs[i] = new TemplateEditingVerb("Tab " + i.ToString(), i, this);
				}					
			}
			return _templateEditingVerbs;
		}
//-------------------------------------------------------------------------------------------
          [Obsolete]
		protected override ITemplateEditingFrame CreateTemplateEditingFrame(TemplateEditingVerb verb)
		{
			ITemplateEditingFrame frame = null;

			for (int i = 0; i < _templateEditingVerbs.Length; i++)
			{
				if (_templateEditingVerbs[i] == verb)
					index = i;
			}
			
			System.Diagnostics.Debug.WriteLine(index.ToString() + ":CreateTemplateEditingFrame");

			if ((_templateEditingVerbs != null) && (index >= 0)) //(_templateEditingVerbs[0] == verb))
			{
				ITemplateEditingService teService = (ITemplateEditingService) GetService(typeof(ITemplateEditingService));

				if (teService != null)
				{
					Style style = ((WeavverTabControl) Component).ControlStyle;
					frame = teService.CreateFrame(this, verb.Text, new string[] {"New Tab Template" }, style, null);
				}

			}
			return frame;
		}

		private void DisposeTemplateEditingVerbs()
		{
			if (_templateEditingVerbs != null)
			{
				for (int i = 0; i < _templateEditingVerbs.Length; i++)
				{
					_templateEditingVerbs[i].Dispose();
				}
				_templateEditingVerbs = null;
			}
		}

          [Obsolete]
		public override string GetTemplateContent(ITemplateEditingFrame editingFrame, string templateName, out bool allowEditing)
		{
//			System.Diagnostics.Debug.WriteLine(index.ToString() + ":GetTemplateContent");

			string content		= String.Empty;
			allowEditing		= true;

			if (	(_templateEditingVerbs != null) 
				&&	(_templateEditingVerbs[index] == editingFrame.Verb))
			{
				//ITemplate currentTemplate	= ((WeavverTabControl)Component).Tabs[index];
				editingFrame.InitialWidth	= (int) ((WeavverTabControl)Component).Width.Value;
				editingFrame.InitialHeight	= (int) ((WeavverTabControl)Component).Height.Value;
//				if (currentTemplate != null)
//				{
//					content = GetTextFromTemplate(currentTemplate);
//				}
			}

			return content;
		}

		public override void SetTemplateContent(ITemplateEditingFrame editingFrame, string templateName, string templateContent)
		{
			if (	(_templateEditingVerbs != null)
				&&	(_templateEditingVerbs[index] == editingFrame.Verb))
			{
				WeavverTabControl control	= (WeavverTabControl) Component;
				ITemplate newTemplate	= null;

				if (	(templateContent != null)
					&&	(templateContent.Length != 0))
				{
					newTemplate				= GetTemplateFromText(templateContent);
					//control.Tabs[index]		= newTemplate;
				}
			}
			UpdateDesignTimeHtml();
		}
//-------------------------------------------------------------------------------------------
		public override bool AllowResize
		{
			get
			{
				bool templateExists = (((WeavverTabControl) Component).Tabs[index] != null);
				return templateExists || InTemplateMode;
			}
		}
//-------------------------------------------------------------------------------------------
		protected override void Dispose(bool disposing)
		{
			if (disposing)
				DisposeTemplateEditingVerbs();

			base.Dispose(disposing);
		}
//-------------------------------------------------------------------------------------------
		public override void Initialize(IComponent component)
		{
			if (!(component is WeavverTabControl))
			{
				throw new ArgumentException("Component must be a TabControl control.", "Component");
			}

			base.Initialize(component);
		}
//-------------------------------------------------------------------------------------------
		public override void OnComponentChanged(object sender, ComponentChangedEventArgs ce)
		{
			if (ce.Member != null)
			{
				string name = ce.Member.Name;
				
				if (	name.Equals("Font")
					||	name.Equals("ForeColor")
					||	name.Equals("BackColor"))
					DisposeTemplateEditingVerbs();
			}
		}
//-------------------------------------------------------------------------------------------
		protected override string GetErrorDesignTimeHtml(Exception e)
		{
			return CreatePlaceHolderDesignTimeHtml("There was an error rendering the control!");
		}
//-------------------------------------------------------------------------------------------
//		private void AddTab(object sender, EventArgs args) 
//		{
//			if (this.Component == null)
//				return;
//			
//            TabControl tb = (TabControl) this.Component;
//			tb.Tabs.Add(new Tab());
		//		}
//-------------------------------------------------------------------------------------------
	}
}
