using System;

namespace Weavver.Net.Mail
{
	public class AddressEndPoint
	{
		public string address;

		public string	prefix;
		public string	domain;

		 public AddressEndPointStatus endpoint;

		public string	path;
		public decimal	quota;

		private string	server;
		public int		port;
		private string	error;
//--------------------------------------------------------------------------------------------
		public void SetAddress(string _address)
		{
			address = _address;
			try
			{
				prefix = address.Substring(0, address.IndexOf("@"));
				domain = address.Substring(address.IndexOf("@") + 1);
			}
			catch {}
		}
//--------------------------------------------------------------------------------------------
		public string	Server
		{ 
			get
			{
				return server;
			}
			set
			{
				if (value.IndexOf(":") > 0)
				{
					server = value; 
					try
					{
						port = Int32.Parse(server.Substring(server.IndexOf(":") + 1).Trim());
						server = server.Substring(0, server.IndexOf(":")).Trim();
					}
					catch {}
				}
				else
				{
					server = value;
				}
			}
		}
//--------------------------------------------------------------------------------------------
		public string	Error
		{
			get
			{
				return error;
			}
			set
			{
				error	= value;
			}
		}
//--------------------------------------------------------------------------------------------
	}
}
