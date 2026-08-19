namespace FMPasswordEncryptDecrypt
{
	using System;
	using System.Text;


	using FMPasswordEncryptDecrypt.Crypt;

	public class PasswordEncrytpDecrpt
	{
		#region Private static members
		private static readonly byte[] DummyData = (new Guid("4BE74006-F456-4399-86C5-03613D7FB234")).ToByteArray();
		private static readonly byte[] Seed = (new Guid("1488AE9C-6813-49AE-AF08-155A53D99CE6")).ToByteArray();
		private static readonly AESCrypt Encryptor = new AESCrypt();
		#endregion

		public static string Decrypt(string password, Guid siteGuid)
		{
			string hex		=  password;
			int prefixIndex = hex.IndexOf("0x");

			if (prefixIndex > -1)
			{
				hex = hex.Substring(2);
			}

			int numberChars = hex.Length;
			byte[] bytes	= new byte[numberChars / 2];

			for (int nextByte = 0; nextByte < numberChars; nextByte += 2)
			{
				bytes[nextByte / 2] = Convert.ToByte(hex.Substring(nextByte, 2), 16);
			}

			string decryptedPassword = Decode(bytes, siteGuid);

			return decryptedPassword;
		}
#region Private Static Methods
		/// <summary>
		/// This method will get the AES key using the site GUID.
		/// </summary>
		/// <param name="siteGuid">Site GUID</param>
		/// <returns>A new AES Key.</returns>
		private static AESKey GetKey(Guid siteGuid)
		{
			var newSeed = new byte[Seed.Length + DummyData.Length];
			Buffer.BlockCopy(Seed, 0, newSeed, 0, Seed.Length);
			Buffer.BlockCopy(DummyData, 0, newSeed, Seed.Length, DummyData.Length);
			return new AESKey(newSeed, siteGuid.ToByteArray());
		}

		/// <summary>
		/// This method will decode the password.
		/// </summary>
		/// <param name="encodedData">The encoded password</param>
		/// <param name="siteGuid">Site GUID</param>
		/// <returns>The naked password.</returns>
		public static string Decode(byte[] encodedData, Guid siteGuid)
		{
			using (AESKey key = GetKey(siteGuid))
			{
				return Encryptor.DecryptToText(encodedData, key);
			}
		}
		#endregion
	}
}
