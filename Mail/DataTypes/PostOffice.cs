using System;

namespace TitaniumSoft.Mail.Admin
{
	public class PostOffice
	{
		public	AdminConnection		adminconnection;
		private string				postofficeid;
		private int					status;
		private bool				isdefault;
		private string				postoffice;
		private string				description;
		private DomainCollection	domains;
		private MailBoxCollection	mailboxes;

		public string ID
		{
			get {return postofficeid;}
			set {postofficeid = value;}
		}
		
		public string Name
		{
			get
			{
				return postoffice;
			}
			set
			{
				if (value != postoffice && adminconnection != null)
				{
					//urladminconnection.
				}
				postoffice = value;
			}
		}

		public int Status
		{
			get {return status;}
			set {status = value;}
		}
		
		public bool IsDefault
		{
			get {return isdefault;}
			set {isdefault = value;}
		}
		

		public string Description
		{
			get {return description;}
			set {description = value;}
		}

		public DomainCollection Domains
		{
			get
			{
				if (domains == null)
					domains = new DomainCollection();
				return domains;
			}
		}

		public MailBoxCollection MailBoxes
		{
			get
			{
				if (mailboxes == null)
					mailboxes = new MailBoxCollection();
				return mailboxes;
			}
		}

		/// <summary>
		/// Do not manually call this method, it will be called when you use AdminConnection.PostOffices.Add(postoffice) automatically.
		/// </summary>
		public void SetAdminConnection(AdminConnection adminconnection)
		{
			this.adminconnection = adminconnection;
		}
	}
}
