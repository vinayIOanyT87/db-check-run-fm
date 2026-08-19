// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Decryption.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the Decryption type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.UtilityObjects
{
	using System;
	using System.IO;
	using System.Text;

	using CAPICOM;

	using Crypt;

	public class Decryption : EncryptionBase
	{
		#region Private Attributes
		/// <summary>
		/// The encoding.
		/// </summary>
		private Encoding encoding;

		/// <summary>
		/// The symmetric key.
		/// </summary>
		private string symmetricKey;
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="Decryption"/> class. 
		/// This is the default constructor for the Encryption Base class.
		/// </summary>
		/// <param name="encoding">
		/// The encoding.
		/// </param>
		public Decryption (Encoding encoding )
		{
			this.encoding = encoding;
			this.Initialize ( );
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the decryption content. This property will set the contents to be decrypted.
		/// </summary>
		public string DecryptionContent { private get; set; }

		/// <summary>
		/// Gets or sets the encrypted key.  This property will set the symmetric key value as a byte array.
		/// </summary>
		public byte[] EncryptedKey { private get; set; }

		#endregion

		#region Public Methods
		/// <summary>
		/// This method is exposed to the client to decrypt a string using AES. The
		/// client must supply the content in the call.
		/// </summary>
		/// <param name="content">Content to decrypt</param>
		/// <returns>Decrypted content.</returns>
		public string DecryptSymmetric ( string content )
		{
			if (string.IsNullOrEmpty(content))
			{
				this.logger.Error ( "Decryption.DecryptSymmetric: No data to decrypt." );
				throw new Exception ( "Must have data to encrypt!" );
			}

			if (( this.EncryptedKey == null ) || ( this.EncryptedKey.Length < 1 ))
			{
				this.logger.Error ( "Decryption.DecryptSymmetric: No data to decrypt." );
				throw new Exception ( "Must supply an encrypted key" );
			}

			this.DecryptionContent = content;
			return this.StartDecryptionSymmetric ( );
		}

		/// <summary>
		/// This method is exposed to the client to decrypt a string using AES. The
		/// client must have previously set the content prior to making this call.
		/// </summary>
		/// <returns>Decrypted data.</returns>
		public string DecryptSymmetric ( )
		{
			if (string.IsNullOrEmpty(this.DecryptionContent))
			{
				this.logger.Error ( "Decryption.DecryptSymmetric: No data to decrypt." );
				throw new Exception ( "Must have data to decrypt!" );
			}

			if (( this.EncryptedKey == null ) || ( this.EncryptedKey.Length < 1 ))
			{
				this.logger.Error ( "Decryption.DecryptSymmetric: No data to decrypt." );
				throw new Exception ( "Must supply an encrypted key" );
			}

			return this.StartDecryptionSymmetric ( );
		}

		/// <summary>
		/// The un-package.
		/// </summary>
		/// <param name="stream">
		/// The stream.
		/// </param>
		/// <returns>
		/// The <see cref="byte[]"/>.
		/// </returns>
		/// <exception cref="Exception">
		/// Unable to read package exception.
		/// </exception>
		public byte [] Unpackage ( Stream stream )
		{
			int initialPosition = Convert.ToInt32 ( stream.Position );
			int headerlen = 200;
			int dataPos = 0;
			var buffer = new byte[30];

			var headerbuffer = new byte[200];
			stream.Read ( headerbuffer, 0, 200 );
			int numberof20Hex = 0;

			for (int i = 0; i < headerbuffer.Length; i++)
			{
				if (0x20 == headerbuffer[i])
				{
					numberof20Hex++;
				}
			}

			if (numberof20Hex >= 30)
			{
				throw new Exception ( "Unable to read package.  The file may be corrupt by a user opening and resaving the file in Microsoft Notepad." );
			}


			stream.Seek ( initialPosition, SeekOrigin.Begin );

			// reader position of first byte of secureSessionKey
			var integerArray = new byte[4];
			stream.Read ( integerArray, 0, 4 );
			int sessionKeyPos = BitConverter.ToInt32 ( integerArray, 0 );
			if (sessionKeyPos != 0)
			{
				throw new Exception ( "Unable to read package.  The file may be corrupt" );
			}

			headerlen -= sizeof(int);

			// read length of secureSessionKey
			stream.Read ( integerArray, 0, 4 );
			int sessionKeyLen = BitConverter.ToInt32(integerArray, 0);
			headerlen -= sizeof(int);

			// read postion of first byte in secure data
			stream.Read ( integerArray, 0, 4 );
			dataPos = BitConverter.ToInt32(integerArray, 0);
			headerlen -= sizeof(int);

			// read length of secure data
			stream.Read ( integerArray, 0, 4 );
			int dataLen = BitConverter.ToInt32(integerArray, 0);
			headerlen -= sizeof(int);

			stream.Seek ( headerlen, SeekOrigin.Current );

			// read secureSessionKey
			var secureSessionKey = new byte[sessionKeyLen];
			stream.Read ( secureSessionKey, 0, sessionKeyLen );

			// read Buffer
			stream.Read ( buffer, 0, buffer.Length );

			// read Secure Data
			var secureData = new byte[dataLen];
			stream.Read ( secureData, 0, dataLen );

			// read Buffer
			stream.Read ( buffer, 0, buffer.Length );

			if (this.encoding == System.Text.Encoding.ASCII)
			{
				this.DecryptionContent = Encoding.ASCII.GetString(secureData);
			}
			else
			{
				this.DecryptionContent = Convert.ToBase64String(secureData);
			}

			this.EncryptedKey = secureSessionKey;

			return Convert.FromBase64String(this.DecryptSymmetric());
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// This method will initialize the base object.
		/// </summary>
		private void Initialize ( )
		{
			this.DecryptionContent = null;
			this.symmetricKey = string.Empty;
		}

		/// <summary>
		/// This method will start the decryption process by encrypting the user's content
		/// using AES.
		/// </summary>
		/// <returns>Decrypted data.</returns>
		private string StartDecryptionSymmetric()
		{
			this.DecryptionAsymmetric();

			var decryptData					= new EncryptedData();
			decryptData.Algorithm.KeyLength = CAPICOM_ENCRYPTION_KEY_LENGTH.CAPICOM_ENCRYPTION_KEY_LENGTH_128_BITS;
			decryptData.Algorithm.Name		= CAPICOM_ENCRYPTION_ALGORITHM.CAPICOM_ENCRYPTION_ALGORITHM_AES;

			decryptData.SetSecret(this.symmetricKey);
			decryptData.Decrypt(this.DecryptionContent);

			return decryptData.Content;
		}

		/// <summary>
		/// This method will decrypt asymmetric encryption key aysmmetrically. It will
		/// get the private asymmetric key from a certificate in order to perform the
		/// decryption.
		/// </summary>
		private void DecryptionAsymmetric ( )
		{
			try
			{
				var cryptor = new RSACrypt();

				using (RSACertificate theCert = new RSACertificate(certificateName))
				{
					byte[] decryptValue = cryptor.Decrypt(this.EncryptedKey, theCert);
					this.symmetricKey = this.encoding.GetString(decryptValue, 0, decryptValue.Length);
				}
			}
			catch (Exception ex)
			{
				string msg = ex.Message;
				this.logger.Error ( "Decryption.DecryptionAsymmetric: " + msg );
				throw new Exception ( "Error in setting up RSACryptoServiceProvider. " + msg );
			}
		}
		#endregion
	}
}
