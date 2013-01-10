using System.Collections;

namespace Weavver.Web
{
	public class WeavverDropDownMenuItemCollection : CollectionBase
	{
//-------------------------------------------------------------------------------------------
		public WeavverDropDownMenuItem Add()
		{
			WeavverDropDownMenuItem item = new WeavverDropDownMenuItem();
			return Add(item);
		}
//-------------------------------------------------------------------------------------------
          public WeavverDropDownMenuItem Add(WeavverDropDownMenuItem item)
		{
               List.Add(item);
               return item;
		}
//-------------------------------------------------------------------------------------------
          public WeavverDropDownMenuItem this[int index]
		{
			get { return (WeavverDropDownMenuItem) List[index]; }
			set { base.List[index] = value; }
		}
//-------------------------------------------------------------------------------------------
          public int IndexOf(WeavverDropDownMenuItem tsdropdownmenuitem)
		{
			return List.IndexOf(tsdropdownmenuitem);
		}
//-------------------------------------------------------------------------------------------
	}
}