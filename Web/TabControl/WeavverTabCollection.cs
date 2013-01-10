using System.Collections;

namespace Weavver.Web
{
	public class WeavverTabCollection : CollectionBase
	{
//-------------------------------------------------------------------------------------------
		public WeavverTabControlTab Add()
		{
			WeavverTabControlTab tstabcontroltab = new WeavverTabControlTab();
			return Add(tstabcontroltab);
		}
//-------------------------------------------------------------------------------------------
		public WeavverTabControlTab Add(WeavverTabControlTab tstabcontroltab)
		{			
			List.Add(tstabcontroltab);
			return tstabcontroltab;
		}
//-------------------------------------------------------------------------------------------
		public WeavverTabControlTab this[int index]
		{
			get { return (WeavverTabControlTab) List[index]; }
			set { base.List[index] = value; }
		}
//-------------------------------------------------------------------------------------------
		public int IndexOf(WeavverTabControlTab tstabcontroltab)
		{
			return List.IndexOf(tstabcontroltab);
		}
//-------------------------------------------------------------------------------------------
	}
}
