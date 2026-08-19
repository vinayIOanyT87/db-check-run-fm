using System;
using System.Runtime.InteropServices;

namespace FMBusinessObjects
{
	public class RasApi
	{
		public const int RAS_MaxEntryName = 256;
		public const int RAS_MaxPhoneNumber = 128;
		public const int RAS_MaxCallbackNumber = RAS_MaxPhoneNumber;
		public const int UNLEN = 256;
		public const int PWLEN = 256;
		public const int CNLEN = 15;
		public const int DNLEN = CNLEN;

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct RASENTRYNAME
		{
			public uint Size;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 257)]
			public string EntryName;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct RASDIALPARAMS
		{
			public uint Size;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = RAS_MaxEntryName + 1)]
			public string EntryName;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = RAS_MaxPhoneNumber + 1)]
			public string PhoneNumber;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = RAS_MaxCallbackNumber + 1)]
			public string CallbackNumber;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = UNLEN + 1)]
			public string UserName;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = PWLEN + 1)]
			public string Password;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = DNLEN + 1)]
			public string Domain;
			public uint SubEntry;
			public uint CallbackID;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct RASDIALEXTENSIONS
		{
			public uint Size;
			public uint Options;
			public uint Parent;
			public uint reserved1;
		}

		[DllImport("rasapi32.dll", CharSet = CharSet.Unicode)]
		public static extern uint RasEnumEntries(
			[MarshalAs(UnmanagedType.LPWStr)]
			string reserved,
			[MarshalAs(UnmanagedType.LPWStr)]
			string PhoneBook,
			[In, Out] RASENTRYNAME[] RasEntryName,
			ref uint Size,
			out uint Number);

		[DllImport("rasapi32.dll", CharSet = CharSet.Unicode)]
		public static extern uint RasGetEntryDialParams(
			[MarshalAs(UnmanagedType.LPWStr)]
			string PhoneBook,
			ref RASDIALPARAMS RasDialParams,
			out bool PasswordFlag);

		[DllImport("rasapi32.dll", CharSet = CharSet.Unicode)]
		public static extern uint RasDial(
			ref RASDIALEXTENSIONS RasDialExtensions,
			[MarshalAs(UnmanagedType.LPWStr)]
			string PhoneBook,
			ref RASDIALPARAMS RasDialParams,
			uint NotifierType,
			[MarshalAs(UnmanagedType.FunctionPtr)]
			Delegate Notifier,
			ref uint RasConn);

		[DllImport("rasapi32.dll", CharSet = CharSet.Unicode)]
		public static extern uint RasHangUp(
			uint RasConn);

	}
}
