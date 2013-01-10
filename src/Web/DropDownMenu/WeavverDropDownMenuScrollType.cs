using System;

namespace Weavver.Web
{
//-------------------------------------------------------------------------------------------
	public enum WeavverDropDownMenuOverflowType
	{
		/// <summary>
		/// Auto will cause the browser to automatically determine whether or not scroll bars are needed.
		/// </summary>
		Auto,
		/// <summary>
		/// Always will cause the browser to render both horizontal and vertical scroll bars whether or not they are needed.
		/// </summary>
		Always,
		/// <summary>
		/// Hidden will cause the browser to automatically hide any overflow on the drop down menu layer.
		/// </summary>
		Hidden,
		/// <summary>
		/// Visible will cause the browser to force the drop down menu layer to the correct size so that all overflow is visible.
		/// </summary>
		Visible,
		/// <summary>
		/// None will act the same was as "hidden" and will hide any overflow on the drop down menu layer.
		/// </summary>
		None
	}
//-------------------------------------------------------------------------------------------
	public class WeavverDropDownMenuOverflow
	{
		public static string ConvertTypeToString(WeavverDropDownMenuOverflowType type)
		{
			switch (type)
			{
                    case WeavverDropDownMenuOverflowType.Auto:
					return "auto";

                    case WeavverDropDownMenuOverflowType.Always:
					return "scroll";

                    case WeavverDropDownMenuOverflowType.Hidden:
					return "hidden";

                    case WeavverDropDownMenuOverflowType.None:
					goto case WeavverDropDownMenuOverflowType.Hidden;

                    case WeavverDropDownMenuOverflowType.Visible:
					return "visible";

				default:
                         goto case WeavverDropDownMenuOverflowType.None;
			}
		}
	}
//-------------------------------------------------------------------------------------------
}
