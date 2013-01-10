using System.Collections;
using TitaniumSoft.Mail.Admin;

namespace TitaniumSoft.Mail.Admin
{
	public class AddressCollection : CollectionBase
	{
		public Address Add()
		{
			Address address = new Address();
			return Add(address);
		}

		public Address Add(Address address)
		{			
			List.Add(address);
			return address;
		}

		public Address this[int index]
		{
			get { return (Address) List[index]; }
			set { base.List[index] = value; }
		}

		public int IndexOf(Address address)
		{
			return List.IndexOf(address);
		}
		
		public Address findByID(string addressid)
		{
			foreach (Address address in List)
			{
				if (address.ID == addressid)
					return address;
			}
			return null;
		}
	}
}