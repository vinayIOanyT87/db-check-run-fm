using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
//using ErrorHandler;

namespace Crypt
{
    /// <summary>
    /// Manages RSA certificates, this class interacts with the internal RSA key store
    /// </summary>
    class CertificateManager
    {
        /// <summary>
        /// Mechanism for safe operation in a multi-threaded environment
        /// </summary>
        private static Object m_singleton = new Object();

        /// <summary>
        /// Keeps track of the singleton instance in the scope of the running thread
        /// </summary>
        protected static CertificateManager m_instance = null;

        /// <summary>
        /// Default constructor, cannot be constructed by any callers other than itself
        /// </summary>
        private CertificateManager() { }

        /// <summary>
        /// Gets the singleton instance, if it doesn't exist then it will be created in a mutually exclusive way
        /// </summary>
        public static CertificateManager Instance
        {
            get
            {
                if (null == m_instance)
                {
                    lock (m_singleton)
                    {
                        m_instance = new CertificateManager();
                    }
                }

                return m_instance;
            }
        }

		  ///// <summary>
		  ///// Attempts to find a RSA certificate of X509 type from the internal certificate store, the search method
		  ///// used is FindSubjectDistinguishedName
		  ///// </summary>
		  ///// <param name="a_name">The subject distinguished name</param>
		  ///// <returns>The RSA certificate found </returns>
		  //public RSACertificate GetCertificateBy_SubjectDistinguishedName(string a_name)
		  //{
		  //    X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
		  //    RSACertificate result = null;

		  //    // attempt to open the store to get the certificate
		  //    store.Open(OpenFlags.ReadOnly);
		  //    X509Certificate2Collection certs = store.Certificates.Find(X509FindType.FindBySubjectDistinguishedName, a_name, false);
		  //    store.Close();

		  //    // there should be only one, so grab the first one
		  //    if (certs.Count > 0)
		  //    {
		  //        result = new RSACertificate(certs[0]);
		  //    }            

		  //    return result;
		  //}

		  /// <summary>
		  /// This method will retrieve a certificate from the Ceritifcate Store. It will
		  /// call the set key method to extract the public key for asymmetric encryption.
		  /// </summary>
		  public X509Certificate2 GetCertificateBySubjectName(string certificateName)
		  {
			  X509Certificate2 theCertificate = null;
			  X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine);

			  try
			  {
				  store.Open(OpenFlags.ReadOnly);
				  X509Certificate2Collection collection =
					  store.Certificates.Find(X509FindType.FindBySubjectName, certificateName, false);
				  if (collection.Count > 0)
				  {
					  theCertificate = collection[0];
				  }
				  else
				  {
					  for (int indx = 0; indx < store.Certificates.Count; indx++)
					  {
						  if (store.Certificates[indx].FriendlyName == certificateName)
						  {
							  theCertificate = store.Certificates[indx];
							  return theCertificate;
						  }
					  }
					  throw new ArgumentException("Certificate not found.");
				  }

			  }
			  catch (Exception ex)
			  {
				  string msg = ex.Message;
				  throw new Exception("Could not create X509Certificate object. " + msg);
			  }
			  finally
			  {
				  store.Close();
			  }
			  return theCertificate;
		  }
    }
}
