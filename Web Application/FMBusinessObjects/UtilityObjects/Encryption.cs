using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;
using CAPICOM;
using Crypt;

namespace FMBusinessObjects.UtilityObjects
{

	public class Encryption : EncryptionBase
	{
		#region Private Attributes
		private Encoding encoding;
		private Guid symmetryicKey;
		private byte[] encryptedSymmetricKey;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the Encryption Base class.
		/// </summary>
		public Encryption (Encoding encoding )
		{
			this.encoding = encoding;
			this.Initialize ( );
		}
		#endregion

		#region Properties
		/// <summary>
		/// This property will set the contents to be encrypted.
		/// </summary>
		public string EncryptionContent { private get; set;}

		#endregion

		#region Public Methods
		/// <summary>
		/// This method is exposed to the client to encrypt a string using AES. The
		/// client must supply the content in the call.
		/// </summary>
		/// <param name="content"></param>
		/// <returns></returns>
		public string EncryptSymmetric ( string content )
		{

			if (string.IsNullOrEmpty(content))
			{
				logger.Error ( "Encryption.EncryptSymmetric: No data to encrpt." );
				throw new EncryptionException ( "Must have data to encrypt!" );
			}

			this.EncryptionContent = content;

			return this.StartEncryptionSymmetric ( );
		}

		/// <summary>
		/// This method is exposed to the client to encrypt a string using AES. The
		/// client must have previously set the content prior to making this call.
		/// </summary>
		/// <returns></returns>
		public string EncryptSymmetric ( )
		{
			if (string.IsNullOrEmpty(this.EncryptionContent))
			{
				logger.Error ( "Encryption.EncryptSymmetric: No data to encrpt." );
				throw new EncryptionException ( "Must have data to encrypt!" );
			}

			return this.StartEncryptionSymmetric();
		}


		public MemoryStream Package ( byte[] data )
		{
			this.EncryptionContent = Convert.ToBase64String(data);

			byte[] encodedData;
			if (this.encoding == Encoding.ASCII)
				encodedData = this.encoding.GetBytes(EncryptSymmetric());
			else
				encodedData = Convert.FromBase64String(EncryptSymmetric());


			byte[] secureSessionKey = encryptedSymmetricKey;

			var newStream = new MemoryStream ( );

			var writer = new BinaryWriter ( newStream );

			byte[] buffer = {0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
								  0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
								  0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00};

			int headerlen = 200;

			//Write position of first byte of secureSessionKey
			writer.Write ( (int) 0 );
			headerlen -= sizeof ( int );

			//Write length of secureSessionKey
			writer.Write ( secureSessionKey.Length );
			headerlen -= sizeof ( int );

			//Write postion of first byte in secure data
			writer.Write ( secureSessionKey.Length + buffer.Length );
			headerlen -= sizeof ( int );

			//Write length of secure data
			writer.Write ( encodedData.Length );
			headerlen -= sizeof ( int );

			//Write remaining bytes of header record.
			for (int i = 0; i < headerlen; i++)
			{
				writer.Write ( (byte) 0 );
			}

			//Write secureSessionKey
			writer.Write ( secureSessionKey, 0, secureSessionKey.Length );

			//Write buffer
			writer.Write ( buffer, 0, buffer.Length );

			//Write SecureData
			writer.Write ( encodedData, 0, encodedData.Length );

			//Write buffer
			writer.Write ( buffer, 0, buffer.Length );

			return writer.BaseStream as MemoryStream;

		}

		#endregion

		#region Private Methods
		/// <summary>
		/// This method will initialize the base object.
		/// </summary>
		private void Initialize ( )
		{
			this.EncryptionContent = null;
			this.symmetryicKey = Guid.Empty;
		}

		/// <summary>
		/// This method will start the encryption process by encrypting the user's content
		/// using AES.
		/// </summary>
		/// <returns></returns>
		private string StartEncryptionSymmetric ( )
		{

			this.symmetryicKey = Guid.NewGuid();
			this.EncryptKeyAysmmetic();

			var encryptData = new EncryptedData();
			encryptData.Algorithm.KeyLength = CAPICOM_ENCRYPTION_KEY_LENGTH.CAPICOM_ENCRYPTION_KEY_LENGTH_128_BITS;
			encryptData.Algorithm.Name = CAPICOM_ENCRYPTION_ALGORITHM.CAPICOM_ENCRYPTION_ALGORITHM_AES;
			encryptData.Content = this.EncryptionContent;
			encryptData.SetSecret(this.symmetryicKey.ToString(), CAPICOM_SECRET_TYPE.CAPICOM_SECRET_PASSWORD);
			return encryptData.Encrypt(CAPICOM_ENCODING_TYPE.CAPICOM_ENCODE_BASE64);
		}

		/// <summary>
		/// Encrypts the key aysmmetic.
		/// </summary>
		/// <exception cref="FMBusinessObjects.UtilityObjects.EncryptionException">Error in setting up RSACryptoServiceProvider.  + msg</exception>
		private void EncryptKeyAysmmetic ( )
		{
			try
			{
				this.encryptedSymmetricKey = null;
				byte[] valueToBeEncrypted = encoding.GetBytes(this.symmetryicKey.ToString());

				RSACrypt cryptor = new RSACrypt();
				using (RSACertificate theCert = new RSACertificate(certificateName))
				{
					this.encryptedSymmetricKey = cryptor.Encrypt(valueToBeEncrypted, theCert);
				}				
			}
			catch (Exception ex)
			{
				string msg = ex.Message;
				logger.Error ( "Encryption.EncryptKeyAsymmetric: " + msg );
				throw new EncryptionException ( "Error in setting up RSACryptoServiceProvider. " + msg );
			}
		}
		#endregion
	}

	/// <summary>
	/// Specialization of Exception class for error encountered by Encryption.
	/// </summary>
	/// <remarks>
	/// Currenly does nothing beyond the base Exception class 
	///</remarks>
	[Serializable ( )]
	public class EncryptionException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the Exception class.
		/// </summary>
		public EncryptionException ( )
			: base ( )
		{
		}

		/// <summary>
		/// Initializes a new instance of the Exception class with a specified error message. 
		/// </summary>
		/// <param name="msg">Error message</param>
		public EncryptionException ( string msg )
			: base ( msg )
		{
		}

		/// <summary>
		/// Initializes a new instance of the Exception class with a specified error message and 
		/// a reference to the inner exception that is the cause of this exception.
		/// </summary>
		/// <param name="msg">Error message</param>
		/// <param name="innerException">inner exception that is the cause of this exception</param>
		public EncryptionException ( string msg, Exception innerException )
			: base ( msg, innerException )
		{
		}

		/// <summary>
		/// Initializes a new instance of the Exception class with serialized data
		/// </summary>
		/// <param name="info">The SerializationInfo that holds the serialized object data about the exception being thrown</param>
		/// <param name="context">The StreamingContext that contains contextual information about the source or destination</param>
		protected EncryptionException ( System.Runtime.Serialization.SerializationInfo info,
												System.Runtime.Serialization.StreamingContext context )
			: base ( info, context )
		{
		}
	}
}
