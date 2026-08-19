using System;
using System.Security.Cryptography;
using Crypt.Interfaces;

namespace Crypt
{
	/// <summary>
	/// A wrapper class for AES
	/// </summary>
	public class AESKey : IKey
	{
		/// <summary>
		/// The internal AES key
		/// </summary>
		/// <remarks>
		/// This could change to just use the byte form representation for better inheritance in the future
		/// </remarks>
		protected SymmetricAlgorithm m_key = null;
        private bool m_disposed = false;
		
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                if (disposing)
                {
                    m_key.Clear();
                    m_key.Dispose();
                    m_key = null;
                }
                m_disposed = true;
            }
        }

		/// <summary>
		/// The default cipher code will be cipher block chaining (CBC)
		/// </summary>
		private const CipherMode DEFAULT_CIPHER_MODE = CipherMode.CBC; // This is .NET default
		private const PaddingMode DEFAULT_PADDING_MODE = System.Security.Cryptography.PaddingMode.PKCS7; // This is .NET default

		/// <summary>
		/// Constructor overload 1, constructs as a copy of the given key
		/// </summary>
		/// <param name="a_key">The key to copy from, this must be a type or sub-type of AESKey</param>
		public AESKey(AESKey a_key)
			: this(a_key.KeyValue, a_key.IV)
		{
		}

		/// <summary>
		/// Constructor overload2, constructs as a copy of the given raw key
		/// </summary>
		/// <param name="a_key">The byte form representation of the key to copy from</param>
		public AESKey(byte[] a_key, byte[] a_IV)
		{
			Clone(a_key, a_IV);
		}

		/// <summary>
		/// Constructor overload2, constructs as a copy of the given raw key
		/// </summary>
		/// <param name="a_key">The byte form representation of the key to copy from</param>
		public AESKey(byte[] a_key)
			: this(a_key, GenerateIVFromKey(a_key))
		{
		}

		/// <summary>
		/// Generate the IV/salt from the key
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		private static byte[] GenerateIVFromKey(byte[] key)
		{
			byte[] salt = new byte[key.Length];
			Array.Copy(key, salt, key.Length);
			Array.Reverse(salt);
			return salt;
		}

		public int KeySize
		{
			get { return m_key.KeySize; }
			set { m_key.KeySize = value; }
		}
		public byte[] IV
		{
			get { return m_key.IV; }
			set { m_key.IV = value; }
		}
		public byte[] KeyValue
		{
			get { return m_key.Key; }
			set { m_key.Key = value; }
		}
		
		/// <summary>
		/// Creates an encryptor which can be used to encrypt data
		/// </summary>
		/// <returns>The encryptor which AESEncryptor or its sub-classes can use</returns>
		public ICryptoTransform CreateEncryptor()
		{
			return m_key.CreateEncryptor(m_key.Key, m_key.IV);
		}

		/// <summary>
		/// Creates a decryptor which can be used to decrypt data
		/// </summary>
		/// <returns>The decryptor which AESEncryptor or its sub-classes can use</returns>
		public ICryptoTransform CreateDecryptor()
		{
			return m_key.CreateDecryptor(m_key.Key, m_key.IV);
		}

		/// <summary>
		/// Generates a random AES key
		/// </summary>
		public void GenerateKey()
		{
			m_key.GenerateKey();
			m_key.GenerateIV();
		}

		/// <summary>
		/// Uses the given byte form representation of an AES key 
		/// </summary>
		/// <param name="a_key">An array of bytes representing the byte version of the key</param>
		/// <remarks>
		/// Ideally this method should be polymorphic or extended so that it can clone based on different
		/// types of AES keys, at the moment of writing it only supports AES keys.
		/// </remarks>
		protected void Clone(byte[] a_key, byte[] a_IV)
		{
			m_key = NewProvider();
			m_key.Mode = DEFAULT_CIPHER_MODE;
			m_key.Padding = DEFAULT_PADDING_MODE;
			m_key.Key = a_key;
			m_key.IV = a_IV;
		}


		/// <summary>
		/// return the key and the Salt/IV
		/// </summary>
		/// <returns></returns>
		public byte[] ToBytes()
		{
			int keyLengthInBytes = KeySize / 8;
			byte[] result = new byte[keyLengthInBytes * 2];
			KeyValue.CopyTo(result, 0);
			IV.CopyTo(result, keyLengthInBytes);
			return result;
		}

		private SymmetricAlgorithm NewProvider()
		{
			SymmetricAlgorithm theProvider = new AesCryptoServiceProvider();
			return theProvider;
		}
	}
}
