using System;

namespace TitaniumSoft.Mail.Admin
{
	public class Domain
	{
		private string	id;
		private string	name;
		private int		status;
		private string	misc;
		private string	description;

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
	}
}
