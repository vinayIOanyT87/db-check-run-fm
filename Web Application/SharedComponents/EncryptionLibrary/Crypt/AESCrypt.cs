using System;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using Crypt.Interfaces;

namespace Crypt
{
	public class AESCrypt : CryptBase
	{
		public static Encoding DEFAULT_CLEARTEXT_ENCODING = Encoding.UTF8;

		/// <summary>
		/// Default constructor
		/// </summary>
		public AESCrypt() { }


		/// <summary>
		/// Calling Decrypt with default text encoding 
		/// </summary>
		/// <param name="a_srcText"></param>
		/// <param name="a_key"></param>
		/// <returns></returns>
		public byte[] Encrypt(string a_srcText, IKey a_key)
		{
			return Encrypt(a_srcText, a_key, DEFAULT_CLEARTEXT_ENCODING);
		}

		/// <summary>
		/// Wrapper to Encrypt method using string
		/// </summary>
		/// <param name="a_srcText"></param>
		/// <param name="a_key"></param>
		/// <returns></returns>
		public byte[] Encrypt(string a_srcText, IKey a_key, Encoding textEncoding)
		{
			byte[] decryptedBytes = textEncoding.GetBytes(a_srcText);
			byte[] encryptedBytes = Encrypt(decryptedBytes, a_key);
			return encryptedBytes;
		}
		
		/// <summary>
		/// Encrypts a block of data using the AES algorithm
		/// </summary>
		/// <param name="a_pt">The block of data to encrypt</param>
		/// <param name="a_key">The key to use, this key should be a type or sub type of AESKey</param>
		/// <returns>A block of encrypted data</returns>
		public override byte[] Encrypt(byte[] a_pt, IKey a_key)
		{
			base.VerifyKey(a_key, typeof(AESKey));

			AESKey key = (AESKey)a_key;

			ICryptoTransform encryptor = key.CreateEncryptor();
			MemoryStream ms = new MemoryStream();
			CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);

			// Beging the encryption process, write the plain text into the memory stream
			cs.Write(a_pt, 0, a_pt.Length);
			// Complete the encryption process
			cs.FlushFinalBlock();

			// Get result from memory stream written to
			byte[] ct = ms.ToArray();

			// force dispose the streams to avoid memory dumping
			ms.Dispose();
			cs.Dispose();

			// Return it
			return ct;
		}

		/// <summary>
		/// Calling Decrypt with default text encoding 
		/// </summary>
		/// <param name="a_srcText"></param>
		/// <param name="a_key"></param>
		/// <returns></returns>
		public string DecryptToText(byte[] a_src, IKey a_key)
		{
			return DecryptToText(a_src, a_key, DEFAULT_CLEARTEXT_ENCODING);
		}
		/// <summary>
		/// Wrapper to Decrypt method using string
		/// </summary>
		/// <param name="a_srcText"></param>
		/// <param name="a_key"></param>
		/// <returns></returns>
		public string DecryptToText(byte[] a_src, IKey a_key, Encoding textEncoding)
		{
			byte[] decryptedBytes = Decrypt(a_src, a_key);
			string decryptedString = textEncoding.GetString(decryptedBytes);
			return decryptedString;
		}

		/// <summary>
		/// Decrypts a block of data using the AES algorithm
		/// </summary>
		/// <param name="a_ct">The block of data to decrypt</param>
		/// <param name="a_key">The key to use, this key should be a type or sub type of AESKey</param>
		/// <returns>A block of decrypted data</returns>
		public override byte[] Decrypt(byte[] a_ct, IKey a_key)
		{
			base.VerifyKey(a_key, typeof(AESKey));

			AESKey key = (AESKey)a_key;

			ICryptoTransform decryptor = key.CreateDecryptor();
			// initialise memory stream with cipher text
			MemoryStream ms = new MemoryStream(a_ct);
			CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);

			// create a buffer to hold the decrypted plain text
			byte[] temp_pt = new byte[a_ct.Length];
			int pt_size = cs.Read(temp_pt, 0, a_ct.Length);

			// now copy what was returned
			byte[] pt = new byte[pt_size];
			Array.Copy(temp_pt, pt, pt_size);

			// force dispose the streams to avoid memory dumping
			ms.Dispose();
			cs.Dispose();

			return pt;
		}
	}
}
