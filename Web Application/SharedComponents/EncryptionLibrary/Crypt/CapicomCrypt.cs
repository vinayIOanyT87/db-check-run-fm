using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Crypt.Interfaces;
using CAPICOM;

namespace Crypt
{
    public class CapicomCrypt : CryptBase
    {
        public override byte[] Encrypt(byte[] a_pt, IKey a_key)
        {
            if ((null == a_pt) || (a_pt.Length == 0))
            {
                throw new CryptException( 
                        "Must have data to encrypt: " +
                        MethodBase.GetCurrentMethod().ToString());
            }

            // Prepare the encryption
            EncryptedData encryptData = new EncryptedData();

            this.PrepareEncryptedData(ref encryptData, a_key);

            // add the data to be encrypted
            encryptData.Content = Encoding.UTF8.GetString(a_pt);

            // Perform the encryption
            encryptData.SetSecret(Encoding.UTF8.GetString(a_key.ToBytes()),
                    CAPICOM_SECRET_TYPE.CAPICOM_SECRET_PASSWORD);

            // return the encrypted string as the original prototype specifies
            string result = encryptData.Encrypt(CAPICOM_ENCODING_TYPE.CAPICOM_ENCODE_ANY);
            return Encoding.UTF8.GetBytes(result);
        }

        public override byte[] Decrypt(byte[] a_ct, IKey a_key)
        {
            if ((null == a_ct) || (a_ct.Length == 0))
            {
                throw new CryptException("Must have data to decrypt: " +
                        MethodBase.GetCurrentMethod().ToString());
            }

            // Prepare the encryption
            // An exception here may indicate that you need to register CAPICOM.DLL and 
            // GAC the Interop.CAPICOM.DLL file.
            EncryptedData decryptData = new EncryptedData();

            this.PrepareEncryptedData(ref decryptData, a_key);

            // Perform the decryption
            decryptData.SetSecret(Encoding.UTF8.GetString(a_key.ToBytes()),
                    CAPICOM_SECRET_TYPE.CAPICOM_SECRET_PASSWORD);
            string input = Encoding.UTF8.GetString(a_ct);
            decryptData.Decrypt(input);

            // return the encrypted string as the original prototype specifies
            System.Text.UTF8Encoding encoding = new UTF8Encoding();
            return encoding.GetBytes(decryptData.Content);
        }

        protected void PrepareEncryptedData(ref EncryptedData a_data, IKey a_key)
        {
            if (a_key.GetType() == typeof(AESKey))
            {
                // 1. Key Length
                switch (((AESKey)a_key).KeySize)
                {
                    case 128:
                        a_data.Algorithm.KeyLength = CAPICOM_ENCRYPTION_KEY_LENGTH.CAPICOM_ENCRYPTION_KEY_LENGTH_128_BITS;
                        break;
                    case 256:
                        a_data.Algorithm.KeyLength = CAPICOM_ENCRYPTION_KEY_LENGTH.CAPICOM_ENCRYPTION_KEY_LENGTH_256_BITS;
                        break;
                    default:
                        throw new CryptException("Key must be either 128 or 256 bit: " +
                                MethodBase.GetCurrentMethod().ToString());
                }
            }
            else
            {
                a_data.Algorithm.KeyLength = CAPICOM_ENCRYPTION_KEY_LENGTH.CAPICOM_ENCRYPTION_KEY_LENGTH_256_BITS;
            }

            // 2. Algorithm
            a_data.Algorithm.Name = CAPICOM_ENCRYPTION_ALGORITHM.CAPICOM_ENCRYPTION_ALGORITHM_AES;
        }
    }
}
