namespace FMBusinessObjects.UtilityObjects
{
	using System.Net;
	using System.Net.Sockets;

	public static class ClientIpAddressUtility
	{
		private static readonly string UnknownIPv4Address = IPAddress.Any.ToString();

		public static string NormalizeToIPv4(string clientIpAddress)
		{
			if (string.IsNullOrWhiteSpace(clientIpAddress))
			{
				return UnknownIPv4Address;
			}

			IPAddress ipAddress;
			if (!IPAddress.TryParse(clientIpAddress.Trim(), out ipAddress))
			{
				return UnknownIPv4Address;
			}

			if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
			{
				return ipAddress.ToString();
			}

			if (ipAddress.IsIPv4MappedToIPv6)
			{
				return ipAddress.MapToIPv4().ToString();
			}

			if (IPAddress.IPv6Loopback.Equals(ipAddress))
			{
				return IPAddress.Loopback.ToString();
			}

			return UnknownIPv4Address;
		}
	}
}
