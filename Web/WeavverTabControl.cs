using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;

namespace TitaniumSoft.WebControls
{
	/// /// THIS CONTROL IS UNFINISHED AND NOT WORKING
	/// /// THIS CONTROL IS UNFINISHED AND NOT WORKING
	/// /// THIS CONTROL IS UNFINISHED AND NOT WORKING
	/// /// THIS CONTROL IS UNFINISHED AND NOT WORKING
	/// /// THIS CONTROL IS UNFINISHED AND NOT WORKING
	/// /// THIS CONTROL IS UNFINISHED AND NOT WORKING
	/// /// THIS CONTROL IS UNFINISHED AND NOT WORKING
	/// /// THIS CONTROL IS UNFINISHED AND NOT WORKING
	public class TSTabControl : WebControl
	{
		private string	text;
		private int		selectedtab;
//-------------------------------------------------------------------------------------------
		[Bindable(true)]
		[Category("Appearance")]
		[DefaultValue("")]
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
		[Bindable(true)]
		[Category("Appearance")]
		public int SelectedIndex
		{
			get
			{
				return selectedtab;
			}
			set
			{
				selectedtab = value;
			}
		}
//-------------------------------------------------------------------------------------------
		protected override void Render(HtmlTextWriter output)
		{
			output.Write(Text);
		}
//-------------------------------------------------------------------------------------------
		public override bool Visible
		{
			get
			{
				return base.Visible;
			}
			set
			{
				base.Visible = value;
			}
		}
//-------------------------------------------------------------------------------------------
	}
}
