using System.Collections;
using TitaniumSoft.Mail.Admin;

namespace TitaniumSoft.Mail.Admin
{
	public class MailBoxCollection : CollectionBase
	{
		public MailBox Add()
		{
			MailBox mailbox = new MailBox();
			return Add(mailbox);
		}

		public MailBox Add(MailBox mailbox)
		{			
			List.Add(mailbox);
			return mailbox;
		}

		public MailBox this[int index]
		{
			get { return (MailBox) List[index]; }
			set { base.List[index] = value; }
		}

		public int IndexOf(MailBox mailbox)
		{
			return List.IndexOf(mailbox);
		}
		
		public MailBox findByID(string mailboxid)
		{
			foreach (MailBox mailbox in List)
			{
				if (mailbox.ID == mailboxid)
					return mailbox;
			}
			return null;
		}
		
		public MailBox findByName(string mailboxname)
		{
			foreach (MailBox mailbox in List)
			{
				if (mailbox.Name == mailboxname)
					return mailbox;
			}
			return null;
		}
	}
}