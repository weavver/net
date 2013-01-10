using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Caching;
using System.ComponentModel;
using System.Reflection;
using System.Text.RegularExpressions;
using System.IO;

namespace Weavver.Web
{
	[ToolboxItem(false)]
	public class WeavverWebControl : WebControl
	{
		private string text	= string.Empty;
//------------------------------------------------------------------------------------------
		[Category("Weavver")]
		public override string ID
		{
			get
			{
				return base.ID;
			}
			set
			{
				base.ID = value;
			}
		}
//------------------------------------------------------------------------------------------
          [Category("Weavver")]
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
//------------------------------------------------------------------------------------------
		[Browsable(false)]
		public override string AccessKey
		{
			get
			{
				return base.AccessKey;
			}
			set
			{
				base.AccessKey = value;
			}
		}
//------------------------------------------------------------------------------------------
		[Browsable(false)]
		public override System.Drawing.Color BackColor
		{
			get
			{
				return base.BackColor;
			}
			set
			{
				base.BackColor = value;
			}
		}
//------------------------------------------------------------------------------------------
		[Browsable(false)]
		public override System.Drawing.Color BorderColor
		{
			get
			{
				return base.BorderColor;
			}
			set
			{
				base.BorderColor = value;
			}
		}
//------------------------------------------------------------------------------------------
		public override BorderStyle BorderStyle
		{
			get
			{
				return base.BorderStyle;
			}
			set
			{
				base.BorderStyle = value;
			}
		}
//------------------------------------------------------------------------------------------
		public override Unit BorderWidth
		{
			get
			{
				return base.BorderWidth;
			}
			set
			{
				base.BorderWidth = value;
			}
		}
//------------------------------------------------------------------------------------------
//		[Browsable(false)]
//		public override FontInfo Font
//		{
//			get
//			{
//				return base.Font;
//			}
//		}
//------------------------------------------------------------------------------------------
		public override System.Drawing.Color ForeColor
		{
			get
			{
				return base.ForeColor;
			}
			set
			{
				base.ForeColor = value;
			}
		}
//------------------------------------------------------------------------------------------
		[Browsable(false)]
		public override short TabIndex
		{
			get
			{
				return base.TabIndex;
			}
			set
			{
				base.TabIndex = value;
			}
		}
//------------------------------------------------------------------------------------------
	}
}
