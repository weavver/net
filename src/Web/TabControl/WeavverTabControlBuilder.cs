using System;
using System.IO;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;


namespace Weavver.Web
{
	public class WeavverTabControlBuilder : ControlBuilder
	{
		#region Fields
		private static TagPrefixAttribute tagprefix = null;
		#endregion
		

		#region Overrides
		public override Type GetChildControlType(String strTagName, IDictionary attributes) 
		{
			// Get the predefined TagPrefix (if exists) from this Assembly
			if (tagprefix == null)
			{
				System.Reflection.Assembly assembly = System.Reflection.Assembly.GetAssembly(typeof(WeavverTabControl));
				object[] tagPrefixes = assembly.GetCustomAttributes(typeof(TagPrefixAttribute), false);
				if (tagPrefixes.Length > 0)
				{
					tagprefix = (TagPrefixAttribute) tagPrefixes[0];
				}
			}

			Type type = null;

			// Check to see if the current tag is a Tab.
			if (tagprefix != null)
			{
				if ( String.Compare(strTagName, tagprefix.NamespaceName + ":tab", true) == 0 ||
					 String.Compare(strTagName, tagprefix.TagPrefix + ":tab", true) == 0 )
				{
					type = typeof(WeavverTabControlTab);
				}
			}

			return type;
		}
		
		public override bool AllowWhitespaceLiterals()
		{
			return false;
		}
		#endregion
	}
}
