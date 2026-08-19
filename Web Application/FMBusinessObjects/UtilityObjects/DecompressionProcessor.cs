// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DecompressionProcessor.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the DecompressionProcessor type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.UtilityObjects
{
	using System;
	using System.IO;
	using System.IO.Compression;
	using System.Text;

	/// <summary>
	/// The decompression processor.
	/// </summary>
	public class DecompressionProcessor
	{
		/// <summary>
		/// The decompress.
		/// </summary>
		/// <param name="content">
		/// The content.
		/// </param>
		/// <returns>
		/// The <see cref="byte"/>.
		/// </returns>
		public byte[] Decompress(string content)
		{
			byte[] compressedbuffer = Convert.FromBase64String(content);
			return this.Decompress(compressedbuffer);
		}

		/// <summary>
		/// The decompress to string.
		/// </summary>
		/// <param name="compressedBuffer">
		/// The compressed buffer.
		/// </param>
		/// <returns>
		/// The <see cref="string"/>.
		/// </returns>
		public string DecompressToString(byte[] compressedBuffer)
		{
			return this.DecompressInternal(compressedBuffer);
		}

		/// <summary>
		/// The decompress.
		/// </summary>
		/// <param name="compressedBuffer">
		/// The compressed buffer.
		/// </param>
		/// <returns>
		/// The <see cref="byte"/>.
		/// </returns>
		public byte[] Decompress(byte[] compressedBuffer)
		{
			var encoder			= new ASCIIEncoding( );
			string strRawData	= this.DecompressInternal(compressedBuffer);
			int length			= encoder.GetByteCount(strRawData.ToCharArray( ));

			return encoder.GetBytes(strRawData.ToCharArray( ), 0, length);
		}

		/// <summary>
		/// The decompress internal.
		/// </summary>
		/// <param name="compressedBuffer">
		/// The compressed buffer.
		/// </param>
		/// <returns>
		/// The <see cref="string"/>.
		/// </returns>
		protected string DecompressInternal(byte[] compressedBuffer)
		{
			var encoder = new ASCIIEncoding( );
			var memoryStream = new MemoryStream( );

			memoryStream.SetLength(compressedBuffer.Length);
			memoryStream.Write(compressedBuffer, 0, compressedBuffer.Length);

			GC.Collect( );

			var decompressObj		= new GZipStream(memoryStream, CompressionMode.Decompress, true);
			memoryStream.Position	= 0;
			string strRawData		= string.Empty;
			var buffer				= new byte[5000000];

			while ( true )
			{
				int bytesRead = decompressObj.Read(buffer, 0, buffer.Length);

				if ( memoryStream.Position == memoryStream.Length )
				{
					strRawData += encoder.GetString(buffer, 0, bytesRead);
					decompressObj.Flush( );
					decompressObj.Close( );
					break;
				}

				strRawData += encoder.GetString(buffer, 0, bytesRead);
			}

			memoryStream.Flush( );
			memoryStream.Close( );
			GC.Collect( );

			return strRawData;
		}
	}
}
