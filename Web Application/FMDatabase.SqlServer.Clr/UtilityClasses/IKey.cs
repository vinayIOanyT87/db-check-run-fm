namespace FMPasswordEncryptDecrypt.Crypt.Interfaces
{
	using System;

	/// <summary>
	/// Interface for all encryptor keys
	/// </summary>
	public interface IKey : IDisposable
	{
		/// <summary>
		/// Generates an random key for the implementing class
		/// </summary>
		void GenerateKey();

		/// <summary>
		/// Converts the implement key into its raw form which is more portable
		/// </summary>
		/// <returns>An array of bytes representing the key</returns>
		byte[] ToBytes();
	}
}
