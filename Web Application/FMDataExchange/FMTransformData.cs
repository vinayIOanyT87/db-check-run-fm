using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

using Crypt;

namespace FMDataExchange
{
	internal class FMTransform : IDisposable
	{
		public FMTransform(byte[] key, byte[] iv)
		{
			_Key = key;
			_IV = iv;
			_AESCryptor = new AESCrypt();
			_AESKey = new AESKey(_Key, _IV);
			UTF8Encoder = new UTF8Encoding();
		}

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _AESKey.Dispose();
                    _AESKey = null;
                }

                _disposed = true;
            }
        }
        private bool _disposed = false;
		private byte[] _Key = null;
		private byte[] _IV = null;
		private UTF8Encoding UTF8Encoder = null;
		private AESCrypt _AESCryptor = null;
		private AESKey _AESKey = null;

		public string EncryptObjectToString(object obj)
		{
			XmlSerializer xs = new XmlSerializer(obj.GetType(), "http://www.varec.com/");
			StringBuilder sb = new StringBuilder();
			StringWriter sw = new StringWriter(sb);
			xs.Serialize(sw, obj);

			return EncryptToString(sb.ToString());
		}

		public string EncryptToString(string TextValue)
		{
			return Convert.ToBase64String(Encrypt(TextValue));
		}

		/// Encrypt some text and return an encrypted byte array.          
		public byte[] Encrypt(string TextValue)
		{
			if (_AESCryptor == null)
			{
				throw new Exception("Encryption algorithm not set");
			}

			//Translates our text value into a byte array.              
			Byte[] bytes = UTF8Encoder.GetBytes(TextValue);
			Byte[] encrypted = null;
			encrypted = _AESCryptor.Encrypt(bytes, _AESKey);
			return encrypted;
		}

		/// The other side: Decryption methods          
		public object DecryptObjectFromString(string EncryptedString, Type type)
		{
			XmlSerializer xs = new XmlSerializer(type, "http://www.varec.com/");
			string plainString = DecryptString(EncryptedString);
			StringReader sr = new StringReader(plainString);
			object retVal = xs.Deserialize(sr);
			return retVal;
		}

		public string DecryptString(string EncryptedString)
		{
			return Decrypt(Convert.FromBase64String(EncryptedString));
		}

		/// Decryption when working with byte arrays.              
		public string Decrypt(byte[] EncryptedValue)
		{
			if (_AESCryptor == null)
			{
				throw new Exception("Encryption algorithm not set");
			}

			Byte[] decryptedBytes = null;
			decryptedBytes = _AESCryptor.Decrypt(EncryptedValue, _AESKey);
			return UTF8Encoder.GetString(decryptedBytes, 0, decryptedBytes.Length);
		}
	}
}
