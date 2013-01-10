using System.Collections;
using TitaniumSoft.Mail.Admin;

namespace TitaniumSoft.Mail.Admin
{
	public class PostOfficeCollection : CollectionBase
	{
		public AdminConnection adminconnection;

//		public PostOffice Add()
//		{
//			PostOffice postoffice		= new PostOffice();
//			adminconnection
//			postoffice.adminconnection	= adminconnection;
//			return Add(postoffice);
//		}

		public PostOffice Add(PostOffice postoffice)
		{	
			postoffice.adminconnection	= adminconnection;
			List.Add(postoffice);
			return postoffice;
		}

		public PostOffice this[int index]
		{
			get { return (PostOffice) List[index]; }
			set { base.List[index] = value; }
		}

		public int IndexOf(PostOffice postoffice)
		{

			return List.IndexOf(postoffice);
		}

		public PostOffice findByName(string name)
		{
			foreach (PostOffice postoffice in List)
			{
				if (postoffice.Name == name)
					return postoffice;
			}

			return null;
		}

		public PostOffice findByID(string postofficeid)
		{
			foreach (PostOffice postoffice in List)
			{
				if (postoffice.ID == postofficeid)
					return postoffice;
			}
			return null;
		}
	}
}