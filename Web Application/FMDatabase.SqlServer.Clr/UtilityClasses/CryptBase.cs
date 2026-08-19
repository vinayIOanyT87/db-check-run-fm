
namespace FMPasswordEncryptDecrypt.Crypt
{
	using System;
	using System.Reflection;

	using FMPasswordEncryptDecrypt.Crypt.Interfaces;

	/// <summary>
	/// Base class with shared functionalities for all encryptors
	/// </summary>
	public class CryptBase : IEncryptor
	{
		/// <summary>
		/// Encrypts a block of bytes
		/// </summary>
		/// <param name="a_pt">The plain text / block of bytes to be encrypted</param>
		/// <param name="a_key">The key to use for encryption</param>
		/// <remarks>This method is polymorphic, the base functionality is not implemented</remarks>
		/// <returns>The computed block of cipher-text</returns>
		public virtual byte[] Encrypt(byte[] a_pt, IKey a_key)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Decrypts a block of bytes
		/// </summary>
		/// <param name="a_ct">The cipher text / block of bytes to be decrypted</param>
		/// <param name="a_key">The key to use for the decryption</param>
		/// <remarks>This method is polymorphic, the base functionality is not implemented</remarks>
		/// <returns>The decrypted block of cipher text using the given key</returns>
		public virtual byte[] Decrypt(byte[] a_ct, IKey a_key)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Verifies if the key used is of a compatible / expected type
		/// </summary>
		/// <param name="a_key">The key used</param>
		/// <param name="a_expected">The compatible / expected type</param>
		protected void VerifyKey(IKey a_key, Type a_expected)
		{
			if (!a_expected.IsAssignableFrom(a_key.GetType()))
			{
				throw new CryptException(MethodBase.GetCurrentMethod() + ": Expected " + a_expected.ToString() + " but got " + a_key.GetType().ToString());
			}
		}
	}

	/// <summary>
	/// Specialization of ApplicationException for error encountered by the ConsolidatedDAL
	/// </summary>
	/// <remarks>
	/// Currently does nothing beyond the base ApplicationException class
	/// </remarks>
	[Serializable()]
	public class CryptException : ApplicationException
	{
		/// <summary>
		/// Initializes a new instance of the Exception class.
		/// </summary>
		public CryptException()
			: base()
		{
		}

		/// <summary>
		/// Initializes a new instance of the Exception class with a specified error message. 
		/// </summary>
		/// <param name="msg">Error message</param>
		public CryptException(string msg)
			: base(msg)
		{
		}

		/// <summary>
		/// Initializes a new instance of the Exception class with a specified error message and 
		/// a reference to the inner exception that is the cause of this exception.
		/// </summary>
		/// <param name="msg">Error message</param>
		/// <param name="innerException">inner exception that is the cause of this exception</param>
		public CryptException(string msg, Exception innerException)
			: base(msg, innerException)
		{
		}

		/// <summary>
		/// Initializes a new instance of the Exception class with serialized data
		/// </summary>
		/// <param name="info">The SerializationInfo that holds the serialized object data about the exception being thrown</param>
		/// <param name="context">The StreamingContext that contains contextual information about the source or destination</param>
		protected CryptException(System.Runtime.Serialization.SerializationInfo info,
													 System.Runtime.Serialization.StreamingContext context)
			: base(info, context)
		{
		}
	}
}
