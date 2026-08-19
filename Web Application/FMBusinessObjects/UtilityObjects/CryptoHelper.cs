// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CryptoHelper.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the CryptoHelper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.UtilityObjects
{
    using System;

    using Crypt;

    /// <summary>
    /// The crypto helper.
    /// </summary>
    public class CryptoHelper : EncryptionBase
    {
        #region Private Static Attributes
        /// <summary>
        /// Preset common seed value used to construct the AESKey.  This cannot be altered at runtime.
        /// </summary>
        private static readonly byte[] PresetSeed = (new Guid("1488AE9C-6813-49AE-AF08-155A53D99CE6")).ToByteArray();

        /// <summary>
        /// Preset common seed data used to construct the AESKey.  This cannot be altered at runtime.
        /// </summary>
        private static readonly byte[] PresetSeedData = (new Guid("4BE74006-F456-4399-86C5-03613D7FB234")).ToByteArray();

        /// <summary>
        /// The encryptor.
        /// </summary>
        private static readonly AESCrypt Encryptor = new AESCrypt();
        #endregion Private Static Attributes

        #region Private Attributes

        /// <summary>
        /// The key guid.
        /// </summary>
        private readonly Guid keyGuid = Guid.Empty;

        /// <summary>
        /// Seed to use when constructing AESKey.
        /// </summary>
        private readonly byte[] seed = null;

        /// <summary>
        /// Additional seed data to use when constructing AESKey.
        /// </summary>
        private readonly byte[] seedData = null;

        #endregion Private Attributes

        #region Properties

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CryptoHelper"/> class using the passed in key and preset seed values.
        /// </summary>
        /// <param name="keyGuid">
        /// The guid key to encrypt and decrypt the data with.
        /// </param>
        /// <remarks>
        /// The created instance will use preset seed values in addition to the passed in key.
        /// </remarks>
        public CryptoHelper(Guid keyGuid)
        {
            this.keyGuid = keyGuid;
            this.seed = CryptoHelper.PresetSeed;
            this.seedData = CryptoHelper.PresetSeedData;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CryptoHelper"/> class.
        /// </summary>
        /// <param name="keyGuid">
        /// The guid key to encrypt and decrypt the data with.
        /// </param>
        /// <param name="customSeed">
        /// Custom seed that can be provided by the consumer in place of the preset seed value.
        /// </param>
        /// <param name="customSeedData">
        /// Custom seed data that can be provided by the consumer in place of the preset seed value.
        /// </param>
        private CryptoHelper(Guid keyGuid, byte[] customSeed, byte[] customSeedData)
        {
            this.keyGuid = keyGuid;
            this.seed = customSeed;
            this.seedData = customSeedData;
        }

        #endregion Constructors

        #region Static Methods
        /// <summary>
        /// Decrypts the passed in encoded data using the configured key and seed values.  These must be the same values used to encrypt the data.
        /// </summary>
        /// <param name="encryptedData">
        /// The encoded data.
        /// </param>
        /// <param name="keyGuid">
        /// The guid key to decode the encoded data with.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string DecryptAesSymmetric(byte[] encryptedData, Guid keyGuid)
        {
            using (AESKey key = GetKey(keyGuid, CryptoHelper.PresetSeed, CryptoHelper.PresetSeedData))
            {
                return CryptoHelper.Encryptor.DecryptToText(encryptedData, key);
            }
        }

        /// <summary>
        /// Encrypts the passed in data using the configured key and seed values.
        /// </summary>
        /// <param name="decryptedData">
        /// The data that should be encrypted.
        /// </param>
        /// <param name="keyGuid">
        /// The guid key to decrypt the encrypted data with.
        /// </param>
        /// <returns>
        /// The <see cref="byte[]"/>.
        /// </returns>
        public static byte[] EncryptAesSymmetric(string decryptedData, Guid keyGuid)
        {
            using (AESKey key = GetKey(keyGuid, CryptoHelper.PresetSeed, CryptoHelper.PresetSeedData))
            {
                return CryptoHelper.Encryptor.Encrypt(decryptedData, key);
            }
        }

        /// <summary>
        /// Creates and initializes a new AESKey object using the passed in initialization Vector Key, Seed and SeedData
        /// </summary>
        /// <param name="initializationVectorKey">
        /// When constructing the AESKey instance, the first encryption block uses the initialization vector key.
        /// </param>
        /// <param name="seed">
        /// The seed.
        /// </param>
        /// <param name="seedData">
        /// The seed Data.
        /// </param>
        /// <returns>
        /// The <see cref="AESKey"/>.
        /// </returns>
        private static AESKey GetKey(Guid initializationVectorKey, byte[] seed, byte[] seedData)
        {
            byte[] newSeed = new byte[seed.Length + seedData.Length];
            Buffer.BlockCopy(seed, 0, newSeed, 0, seed.Length);
            Buffer.BlockCopy(seedData, 0, newSeed, seed.Length, seedData.Length);
            return new AESKey(newSeed, initializationVectorKey.ToByteArray());
        }

        #endregion Static Methods

        #region Public Methods
        /// <summary>
        /// Decrypts the passed in encoded data using the configured key and seed values.  These must be the same values used to encrypt the data.
        /// </summary>
        /// <param name="encryptedData">
        /// The data that should be decrypted.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string DecryptAesSymmetric(byte[] encryptedData)
        {
            using (AESKey key = GetKey(this.keyGuid, this.seed, this.seedData))
            {
                return CryptoHelper.Encryptor.DecryptToText(encryptedData, key);
            }
        }

        /// <summary>
        /// Encrypts the passed in data using the configured key and seed values.
        /// </summary>
        /// <param name="data">
        /// The data that should be encrypted.
        /// </param>
        /// <returns>
        /// The <see cref="byte[]"/>.
        /// </returns>
        public byte[] EncryptAesSymmetric(string data)
        {
            using (AESKey key = GetKey(this.keyGuid, this.seed, this.seedData))
            {
                return CryptoHelper.Encryptor.Encrypt(data, key);
            }
        }
        #endregion Public Methods
    }
}
