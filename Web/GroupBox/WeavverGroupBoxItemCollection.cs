using System.Collections;

namespace Weavver.Web
{
     public class WeavverGroupBoxItemCollection : CollectionBase
	{
//-------------------------------------------------------------------------------------------
		public WeavverGroupBoxItem Add()
		{
			WeavverGroupBoxItem tsgroupboxitem = new WeavverGroupBoxItem();
			return Add(tsgroupboxitem);
		}
//-------------------------------------------------------------------------------------------
          public WeavverGroupBoxItem Add(WeavverGroupBoxItem tsgroupboxitem)
		{			
			List.Add(tsgroupboxitem);
			return tsgroupboxitem;
		}
//-------------------------------------------------------------------------------------------
          public WeavverGroupBoxItem this[int index]
		{
			get
			{
                    return (WeavverGroupBoxItem)List[index];
			}
			set
			{
				base.List[index] = value;
			}
		}
//-------------------------------------------------------------------------------------------
          public int IndexOf(WeavverGroupBoxItem tsgroupboxitem)
		{
			return List.IndexOf(tsgroupboxitem);
		}
//-------------------------------------------------------------------------------------------
	}
}
