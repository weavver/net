using System.Collections;
using TitaniumSoft.Mail.Admin;

namespace TitaniumSoft.Mail.Admin
{
	public class DomainCollection : CollectionBase
	{
		public Domain Add()
		{
			Domain domain = new Domain();
			return Add(domain);
		}

		public Domain Add(Domain domain)
		{			
			List.Add(domain);
			return domain;
		}

		public Domain this[int index]
		{
			get { return (Domain) List[index]; }
			set { base.List[index] = value; }
		}

		public int IndexOf(Domain domain)
		{
			return List.IndexOf(domain);
		}

		public Domain findByID(string domainid)
		{
			foreach (Domain domain in List)
			{
				if (domain.ID == domainid)
					return domain;
			}
			return null;
		}
	}
}