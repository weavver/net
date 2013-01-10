using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;

using System.Web.UI;
using System.Web.UI.WebControls;


namespace Weavver.Web
{
	public class WeavverDefaultTabControlTabTemplate : ITemplate
	{
		public void InstantiateIn(Control container)
		{
			Label label		= new Label();
			label.Text		= "Empty Template";
			container.Controls.Add(label);
		}
	}
}
