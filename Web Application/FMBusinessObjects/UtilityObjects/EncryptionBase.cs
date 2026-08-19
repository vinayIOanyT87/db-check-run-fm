using System;
using System.Collections.Generic;
using System.Text;
using Crypt;
using FMBusinessObjects.LogClient;

namespace FMBusinessObjects.UtilityObjects
{
	public abstract class EncryptionBase
	{
		#region Protected Attributes
		protected string certificateName;
		protected Logger logger;
		#endregion

		#region Private Attributes
		private int asymmetricKeySize;
		private string asymmetricKeyContainerName;
		#endregion

		#region Properties
		protected int AsymmetricKeySize
		{
			get { return this.asymmetricKeySize; }
		}

		protected string AsymmetricKeyContainerName
		{
			get { return this.asymmetricKeyContainerName; }
		}
		/// <summary>
		/// This property will get or set the actual certificate name.
		/// 06-19-2007 E. Simmons
		/// Added to handled multiple Certificates 
		///</summary>
		public string CertificateName
		{
			get { return this.certificateName; }
			set { this.certificateName = value; }
		}
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the Encryption Library class.
		/// </summary>
		public EncryptionBase ( )
		{
			this.Initialize ( );
		}
		#endregion

		#region Protected

		/// <summary>
		/// This method is used to convert a string to a byte array.
		/// </summary>
		/// <param name="encryptedString"></param>
		/// <returns></returns>
		protected byte[] ConvertToBytes ( string convertString )
		{
			byte[] byteStr = null;

			if (( convertString != null ) && ( convertString.Length > 0 ))
			{
				byteStr = System.Text.ASCIIEncoding.ASCII.GetBytes ( convertString );
			}

			return byteStr;
		}
		#endregion
		

		#region Private methods
		/// <summary>
		/// This method initialize the EcryptionBase class to its initial state.
		/// </summary>
		private void Initialize ( )
		{
			this.asymmetricKeySize = 1024;
			this.asymmetricKeyContainerName = "Encryption.AsymmetricEncryption.DefaultContainerName";
			this.logger = new Logger ( "EncryptionBase" );
		}
		#endregion
	}
}
