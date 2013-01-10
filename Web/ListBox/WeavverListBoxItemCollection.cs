using System.Collections;

namespace Weavver.Web
{
	public class WeavverListBoxItemCollection : CollectionBase
	{
//-------------------------------------------------------------------------------------------
		public WeavverListBoxItem Add()
		{
			WeavverListBoxItem tslistboxitem = new WeavverListBoxItem();
			return Add(tslistboxitem);
		}
//-------------------------------------------------------------------------------------------
		public WeavverListBoxItem Add(WeavverListBoxItem tslistboxitem)
		{			
			List.Add(tslistboxitem);
			return tslistboxitem;
		}
//-------------------------------------------------------------------------------------------
		public WeavverListBoxItem this[int index]
		{
			get { return (WeavverListBoxItem) List[index]; }
			set { base.List[index] = value; }
		}
//-------------------------------------------------------------------------------------------
		public int IndexOf(WeavverListBoxItem tslistboxitem)
		{
			return List.IndexOf(tslistboxitem);
		}
//-------------------------------------------------------------------------------------------
	}
}