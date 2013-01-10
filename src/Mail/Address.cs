using System;

namespace Weavver.Net.Mail
{
	public class Address
	{
		public string address;

		public string	prefix;
		public string	domain;

		private AddressEndPointStatus	endpoint;

		public decimal	quota;

		private string	server;
		private string	path;
		public int		port;
		private string	error;
//--------------------------------------------------------------------------------------------
		public AddressEndPointStatus EndPoint
		{
			get
			{
				return endpoint;
			}
			set
			{
				endpoint = value;
			}
		}
//--------------------------------------------------------------------------------------------
		public string Path
		{
			get
			{
				return path;
			}
			set
			{
				path = value;
			}
		}
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
		public override string ToString()
		{
			string recipient = "Recipient: " + Path;
			if (endpoint == AddressEndPointStatus.SMS)
				recipient = recipient + "|" + endpoint.ToString();

			recipient = recipient + "|0";
			return recipient;
		}
//--------------------------------------------------------------------------------------------
	}
}