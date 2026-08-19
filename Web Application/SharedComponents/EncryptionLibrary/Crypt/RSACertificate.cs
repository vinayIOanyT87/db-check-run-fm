using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Crypt.Interfaces;

namespace Crypt
{
    // a wrapper for the X509Certificate2 certificate, this is done so that we can use
    // EncryptorBase for this.

    /// <summary>
    /// A wrapper for the X509Certificate2 certificate, this is done so that we can use
    /// EncryptorBase for this, as well as hiding archaic knowledge from future programmers
    /// </summary>
    public class RSACertificate : IKey
    {
        protected X509Certificate2 m_cert;

        public void Dispose()
        {
            m_cert = null;
            System.GC.Collect();
        }

		  /// <summary>
		  /// Create a RSACertificate using the given cert name
		  /// </summary>
		  /// <param name="a_cert"></param>
		  public RSACertificate(string certificateName)
		  {
			  RSACryptoServiceProvider.UseMachineKeyStore = true;
			  Certificate = CertificateManager.Instance.GetCertificateBySubjectName(certificateName);
		  }
		  

        /// <summary>
        /// Converts the RSACertificate object into byte represented form
        /// </summary>
        /// <returns>An array of bytes representing the RSACertificate / the internal X509Certificate2 object</returns>
        public byte[] ToBytes()
        {
            byte[] rawData = Certificate.RawData;

            string rawData_base64 = Convert.ToBase64String(rawData);

            System.Text.UTF8Encoding encoding = new UTF8Encoding();
            return encoding.GetBytes(rawData_base64);
        }

        /// <summary>
        /// Gets or sets the internal X509Certificate2 object
        /// </summary>
        public X509Certificate2 Certificate 
        {
            get { return m_cert; }
            set { m_cert = value; } 
        }

        #region unused methods

        /// <summary>
        /// Generates a NotImplementedException, this was not implemented because it's against accepted security practices to
        /// generate random certificates.
        /// </summary>
        public void GenerateKey()
        {
            throw new NotImplementedException();
        }

        #endregion // unused methods
    }
}
