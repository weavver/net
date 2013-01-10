using System;

namespace TitaniumSoft.Mail.Admin
{
	public class MailBox
	{
		private AddressCollection	addresses;
		private string				id;
		private string				name;
		private int					status;
		private int					quota;
		private string				misc;
		private string				description;
		
		public string ID
		{
			get {return id; }
			set {id = value; }
		}
		
		public string Name
		{
			get {return name; }
			set {name = value; }
		}

		public int Status
		{
			get {return status; }
			set {status = value; }
		}

		public int Quota
		{
			get {return quota; }
			set {quota = value; }
		}

		public string Misc
		{
			get {return misc; }
			set {misc = value; }
		}

		public string Description
		{
			get {return description; }
			set {description = value; }
		}

		public AddressCollection Addresses
		{
			get
			{
				if (addresses == null)
					addresses = new AddressCollection();
				return addresses;
			}
		}
	}
}
