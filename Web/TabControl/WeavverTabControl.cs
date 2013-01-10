using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Collections;

using Weavver.Design;

// DOES NOT WORK, can't figure it out, if you can let me know!
namespace Weavver.Web
{
	[ToolboxItem(true)]
	[DefaultProperty("Tabs")]
	[ParseChildren(true, "Tabs")]
	[PersistChildren(false)]
	//[Designer(typeof(TabControlDesigner))]
//	[ToolboxData("<{0}:WeavverTabControl runat=server><{0}:WeavverTabControlTab></{0}:WeavverTabControlTab}></{0}:WeavverTabControl>")]
//	[ControlBuilder(typeof(WeavverTabControlBuilder))]
	public class WeavverTabControl : WebControl, INamingContainer
	{
		private WeavverTabCollection	tabs			= new WeavverTabCollection();
		private int				selectedindex	= 0;
//-------------------------------------------------------------------------------------------
		public WeavverTabControl()
		{
		}
//-------------------------------------------------------------------------------------------
		[Category("Weavver, Inc.")]
		[PersistenceMode(PersistenceMode.InnerDefaultProperty)]
		[DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content)]
		[Description("This is the item collection that will be used to hold each Tab Control Tab's content.")]
		public WeavverTabCollection Tabs
		{
			get	
			{
				return tabs;
			}
		}
//-------------------------------------------------------------------------------------------
//		protected override void CreateChildControls()
//		{
//			this.Controls.Clear();
//
//			for (int i = 0; i < Tabs.Count; i++)
//			{
//				if (Tabs[i] == null)
//					Tabs[i] = new WeavverTabControlTab(); //"asdfs", "asdf@@");
//
//				this.Controls.Add(Tabs[i]);
//			}
//		}

		[
		Bindable(true),
		Category("Design"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerDefaultProperty)
		]
		public int SelectedIndex
		{
			get{return			selectedindex;}
			set{selectedindex	= (value < tabs.Count) ? value : selectedindex;}
		}

		public override ControlCollection Controls
		{
			get
			{
				EnsureChildControls();
				return base.Controls;
			}
		}


		#region Hide!
		protected override void Render(HtmlTextWriter output)
		{
			if (Tabs.Count == 0)
				return;

			base.Render(output);

			string height = "";
			if (!(Height.IsEmpty || Height.Value == 0))
			{
				height = "height='" + Height.Value.ToString() + GetUnit(Height.Type.ToString()) + "'";
			}
			
			string width = "";
			if (!(Width.IsEmpty || Width.Value == 0))
			{
				width = "width='" + Width.Value.ToString() + GetUnit(Width.Type.ToString()) + "'";
			}

			string htmltabs = "";;
			for (int i = 0; i < tabs.Count; i++)
			{
				bool b = (i == selectedindex);
				htmltabs += FormatTab(0, i, tabs.Count, b, tabs[i].Text);
			}

			output.AddStyleAttribute("width", Width.ToString());
			output.RenderBeginTag("div");
				#region htmltabtable
				string htmltabtable =
					@"<table %width% cellpadding=""0"" cellspacing=""0"" width=""100%"" border=0>
						<tr id=""{0}:Row{1}"">
						{2}
						</tr>
						</table>";
				#endregion
				htmltabtable = htmltabtable.Replace("%width%", width);
				htmltabtable = String.Format(htmltabtable, this.UniqueID, 1, htmltabs);

				string htmltablemain = 
					@"	<TABLE {0} style=""BORDER-RIGHT: #CCCCCC thin solid; Z-INDEX: 101; LEFT: 232px; BORDER-LEFT: #CCCCCC thin solid; BORDER-BOTTOM: #CCCCCC thin solid;"" cellSpacing=1 cellPadding=1 border=0>
						<TR>
							<TD valign=""top"">";
						
				htmltablemain = String.Format(htmltablemain, width + " " + height);

				string htmltablemainb =
					@"		</TD>
						</TR>
						</TABLE>";

				if (!Page.ClientScript.IsClientScriptBlockRegistered("Dbworks.Web.Controls.TabControl"))
					Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Dbworks.Web.Controls.TabControl", JavaScript());

				output.Write(JavaScript());

				output.Write(TabStyle());
				output.Write(htmltabtable);

				output.Write(htmltablemain);
				for (int i = 0; i < tabs.Count; i++)
				{
					output.Write(FormatContentTableA(i, (i == selectedindex)));
					try
					{
						//	Tabs[i].TabContent.InstantiateIn(Tabs[i].TabPanel);
						//	Tabs[i].Visible = true; //TabPanelRenderControl(output);
					}
					catch (Exception e)
					{
						output.Write(e.Message);
					}
					Tabs[i].RenderControl(output);
					output.Write(FormatContentTableB());
				}
				output.Write(htmltablemainb);
			output.RenderEndTag();
		}

		//	protected void AddParsedSubObject(object obj)
		//	{
		//		if (obj.GetType() == _Tabs.GetType())
		//			_Tabs.Add((Tab) obj);
		//	}

			#region TabStyle
			private string TabStyle()
			{
				string style = 
				@"	<style>
						A {text-decoration:none;color:black;}
						A:Hover {color:Blue}
					</style>";
				return style;
			}
			#endregion

			#region FormatTab
			private string FormatTab(int Row, int Tab, int TotalTabs, bool selected, string content)
			{
				string htmltab	=
					@"<td align=""middle"" id=""{0}:Tab{1}"" width=""20%"" bgcolor=""{3}"" style=""BORDER-BOTTOM: none; BORDER-LEFT: {4} thin solid; BORDER-RIGHT: #CCCCCC thin solid; BORDER-TOP: #CCCCCC thin solid; FONT-FAMILY: Verdana; FONT-SIZE: smaller; FONT-WEIGHT: bold; cursor: hand;"" onclick=""javascript:ShowTab('{0}:', {1}, {2})"">{5}</a>
					</td>";

				string bgcolor		= (selected) ? "#FFFFFF" : "#D4D0C8";
			
				string borderleft	= "";
				if (Tab == (SelectedIndex + 1) || Tab == SelectedIndex || Tab == 0)
					borderleft		= "#CCCCCC thin solid";
				else
					borderleft		= "#CCCCCC thin solid";
		
				return String.Format(htmltab, UniqueID, Tab, TotalTabs,  bgcolor, borderleft, content);
			}
			#endregion

			#region FormatContentTableA
			private string FormatContentTableA(int TabIndex, bool selected)
			{
				string htmlcontentdiv =
					@"<div id=""{0}:Pane{1}"" style='display: {2};'>";

				return String.Format(htmlcontentdiv, this.UniqueID, TabIndex, (selected) ? "block" : "none");
			}
			#endregion
			#region FormatContentTableB
			private string FormatContentTableB()
			{
				string htmlcontenttable = 
					@"</div>";

				return htmlcontenttable;
			}
			#endregion

			#region GetUnit
			private string GetUnit(string unit)
			{
				switch (unit)
				{
					case "Percentage":
						return "%";

					case "Pixel":
						return "px";

					case "Point":
						return "pt";

					default:
						return "";
				}
			}
			#endregion GetUnit

			#region JavaScript
			private string JavaScript()
			{
				string script =
					@"<script language=javascript>
						function ShowTab(objid, index, count)
						{
								var tab		= """";
								var pane	= """";
								for(var i= 0; i < count; i++)
								{
									tab  = objid + ""Tab""	+ i;
									pane = objid + ""Pane""	+ i;

									var otab	= document.getElementById(tab).style;
									var opane	= document.getElementById(pane).style;
									if (i != index)
									{
										otab.backgroundColor	= ""#D4D0C8"";
										if (i - 1 == index || i == 0)
											otab.borderLeft		= ""#CCCCCC thin solid"";
										else
											otab.borderLeft		= ""#FFFFFF thin solid"";
										//otab.borderBottom		= '#CCCCCC thin groove';

										opane.display = ""none"";
									}
									else
									{
										otab.backgroundColor	= ""#FFFFFF"";
										if (i == 0)
											otab.borderleft		= ""#CCCCCC thin solid"";
										else
											otab.borderLeft		= """";
										otab.borderBottom		= ""none"";

										opane.display			= ""block"";
									}
								}
						}
					</SCRIPT>";

			
				return script;
			}
			#endregion
		//}

		#endregion
	}
}
