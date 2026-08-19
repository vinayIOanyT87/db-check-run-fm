using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Reflection;
using Crypt.Interfaces;
using ErrorHandler;

namespace Crypt
{
    /// <summary>
    /// Used to identify which key to use for RSA encryption and decryption
    /// </summary>
    enum PKI
    {
        /// <summary>
        /// Use this value for using the public key
        /// </summary>
        PUBLIC_KEY,

        /// <summary>
        /// Use this value for using the private key
        /// </summary>
        PRIVATE_KEY
    }

    class RSAEncryptor : EncryptorBase
    {
        #region Properties
        public PKI EncryptMode { get; set; }
        public PKI DecryptMode { get; set; }
        #endregion // Properties

        #region Constructors
        /// <summary>
        /// Default constructor, by default sets encryption to use any given certificate's public key, and 
        /// decryption to use any given certificate's private key
        /// </summary>
        public RSAEncryptor()
        {
            EncryptMode = PKI.PUBLIC_KEY;
            DecryptMode = PKI.PRIVATE_KEY;
        }

        /// <summary>
        /// Overloaded constructor 1, lets the caller set the encryption and decryption mode
        /// </summary>
        /// <param name="a_encryptMode">the encryption mode, pass in PKI.PUBLIC_KEY to use the public key for encryption, otherwise PKI.PRIVATE_KEY</param>
        /// <param name="a_decryptMode">the decryption mode, pass in PKI.PUBLIC_KEY to use the public key for decryption, otherwise PKI.PRIVATE_KEY</param>
        public RSAEncryptor(PKI a_encryptMode, PKI a_decryptMode)
        {
            EncryptMode = a_encryptMode;
            DecryptMode = a_decryptMode;
        }
        #endregion // Constructors

        /// <summary>
        /// Decrypts a block of data using a specified RSA certificate, this method will use the PRIVATE_KEY
        /// on the certificate by default for decryption. To change this set the DecryptMode or use the
        /// overloaded constructor.
        /// </summary>
        /// <param name="a_ct">The block of data to decrypt</param>
        /// <param name="a_key">The RSA certificate</param>
        /// <returns>Block of decrypted data</returns>
        public override byte[] Decrypt(byte[] a_ct, IKey a_key)
        {
            base.VerifyKey(a_key, typeof(RSACertificate));

            RSACryptoServiceProvider provider = CreateCryptProvider((RSACertificate) a_key, DecryptMode);

            byte[] pt = provider.Decrypt(a_ct, false);

            return pt;
        }

        /// <summary>
        /// Encrypts a block of data using a specified RSA certificate, this method will use the PUBLIC_KEY
        /// on the certificate by default for encryption. To change this set the EncryptMode or use the
        /// overloaded constructor.
        /// </summary>
        /// <param name="a_pt">The block of data the encrypt</param>
        /// <param name="a_key">The RSA certificate</param>
        /// <returns>Block of encrypted data</returns>
        public override byte[] Encrypt(byte[] a_pt, IKey a_key)
        {
            base.VerifyKey(a_key, typeof(RSACertificate));

            RSACryptoServiceProvider provider = CreateCryptProvider((RSACertificate) a_key, EncryptMode);

            byte[] ct = provider.Encrypt(a_pt, false);
            
            return ct;
        }

        #region Protected methods

        /// <summary>
        /// Create a RSACryptoServiceProvider based on a certifiate
        /// </summary>
        /// <param name="a_cert">The RSA certificate</param>
        /// <param name="a_cryptMode">Whether to generate the provider based on the PKI.PUBLIC_KEY or PKI.PRIVATE_KEY keys</param>
        /// <returns>The created RSACryptoServiceProvider object</returns>
        protected RSACryptoServiceProvider CreateCryptProvider(RSACertificate a_cert, PKI a_cryptMode)
        {
            X509Certificate2 cert = a_cert.Certificate;
            RSACryptoServiceProvider provider = new RSACryptoServiceProvider();

            if (PKI.PUBLIC_KEY == a_cryptMode)
            {
                provider.FromXmlString(cert.PublicKey.Key.ToXmlString(false));
            }
            else
            {
                provider.FromXmlString(cert.PrivateKey.ToXmlString(false));
            }

            return provider;
        }

        #endregion // Protected methods
    }
}
