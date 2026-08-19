namespace FMBusinessObjects.UtilityObjects
{
    using System;
    using System.Data;
    using System.IO;
    using System.IO.Compression;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Text;

    public class CompressionProcessor
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CompressionProcessor"/> class.
        /// </summary>
        public CompressionProcessor()
        {
        }

        #endregion Constructors and Destructors

        #region Public Methods and Operators

        /// <summary>
        /// Compresses the passed in <see cref="DataSet"/> using <see cref="GZipStream"/> compression.
        /// </summary>
        /// <param name="dataSet">The data set to compress.</param>
        /// <returns>Returns an array of bytes that contains the compressed data set.</returns>
        public static byte[] CompressDataSet(DataSet dataSet)
        {
            // Make sure this is done so we get binary serialization rather than XML.
            dataSet.RemotingFormat = SerializationFormat.Binary;

            long originalSize = 0;
            return CompressionProcessor.CompressObjectInternal(dataSet, out originalSize);
        }

        /// <summary>
        /// Compresses the passed in <see cref="DataSet"/> using <see cref="GZipStream"/> compression.
        /// </summary>
        /// <param name="dataSet">The data set to compress.</param>
        /// <param name="originalSize">Out Parameter that contains the size of the compressed data.</param>
        /// <returns>Returns an array of bytes that contains the compressed data set.</returns>
        public static byte[] CompressDataSet(DataSet dataSet, out long originalSize)
        {
            // Make sure this is done so we get binary serialization rather than XML.
            dataSet.RemotingFormat = SerializationFormat.Binary;

            return CompressionProcessor.CompressObjectInternal(dataSet, out originalSize);
        }

        /// <summary>
        /// Compresses the passed in <see cref="object"/> using binary serialization and <see cref="GZipStream"/> compression.
        /// </summary>
        /// <param name="uncompressedObject">The object to compress.</param>
        /// <returns>Returns an array of bytes that contains the compressed object.</returns>
        /// <remarks>The passed in object must be serializable.</remarks>
        public static byte[] CompressObject(object uncompressedObject)
        {
            long originalSize = 0;
            return CompressObjectInternal(uncompressedObject, out originalSize);
        }

        /// <summary>
        /// Compresses the passed in <see cref="object"/> using binary serialization and <see cref="GZipStream"/> compression.
        /// </summary>
        /// <param name="uncompressedObject">The object to compress.</param>
        /// <param name="originalSize">Out Parameter that contains the size of the compressed data.</param>
        /// <returns>Returns an array of bytes that contains the compressed object.</returns>
        /// <remarks>The passed in object must be serializable.</remarks>
        public static byte[] CompressObject(object uncompressedObject, out long originalSize)
        {
            return CompressObjectInternal(uncompressedObject, out originalSize);
        }

        /// <summary>
        /// Decompresses the passed in byte array that represents a compressed data set.
        /// </summary>
        /// <param name="compressedDataSetBytes">The compressed data set bytes.</param>
        /// <returns>DataSet.</returns>
        public static DataSet DecompressDataSet(byte[] compressedDataSetBytes)
        {
            long uncompressedSize = 0;
            return (DataSet)CompressionProcessor.DecompressObjectInternal(compressedDataSetBytes, out uncompressedSize);
        }

        /// <summary>
        /// Decompresses the passed in byte array that represents a compressed data set.
        /// </summary>
        /// <param name="compressedDataSetBytes">The compressed data set bytes.</param>
        /// <param name="uncompressedSize">Out Parameter that contains the approximate uncompressed size of the object.</param>
        /// <returns>DataSet.</returns>
        public static DataSet DecompressDataSet(byte[] compressedDataSetBytes, out long uncompressedSize)
        {
            return (DataSet)CompressionProcessor.DecompressObjectInternal(compressedDataSetBytes, out uncompressedSize);
        }

        /// <summary>
        /// Decompresses the passed in byte array and deserializes the results to produce the object.
        /// </summary>
        /// <param name="compressedObjectBytes">The compressed object represented as a byte array.</param>
        /// <returns>A deserialized copy of the compressed data as a <see cref="System.Object"/>.</returns>
        /// <remarks>The compressed object is uncompressed and the results are deserialized using a binary formatter.</remarks>
        public static object DecompressObject(byte[] compressedObjectBytes)
        {
            long uncompressedSize = 0;
            return DecompressObjectInternal(compressedObjectBytes, out uncompressedSize);
        }

        /// <summary>
        /// Decompresses the passed in byte array and deserializes the results to produce the object.
        /// </summary>
        /// <param name="compressedObjectBytes">The compressed object represented as a byte array.</param>
        /// <param name="uncompressedSize">Out Parameter that contains the approximate uncompressed size of the object.</param>
        /// <returns>A deserialized copy of the compressed data as a <see cref="System.Object"/>.</returns>
        /// <remarks>The compressed object is uncompressed and the results are deserialized using a binary formatter.</remarks>
        public static object DecompressObject(byte[] compressedObjectBytes, out long uncompressedSize)
        {
            return DecompressObjectInternal(compressedObjectBytes, out uncompressedSize);
        }

        /// <summary>
        /// Gets the approximate size of the passed in <see cref="DataSet"/> instance.
        /// </summary>
        /// <param name="dataSet">The data set to calculate the size of.</param>
        /// <returns>Returns the approximate size of the object within an <see cref="System.Int64"/> type.</returns>
        public static long GetApproximateDataSetSize(DataSet dataSet)
        {
            long size = 0;

            using (var memStream = new MemoryStream())
            {
                dataSet.RemotingFormat = SerializationFormat.Binary;
                dataSet.WriteXml(memStream, XmlWriteMode.WriteSchema);

                size = memStream.Length;
            }

            return size;
        }

        /// <summary>
        /// Gets the approximate size of the passed in <see cref="object"/>.
        /// </summary>
        /// <param name="inObject">The object to compress.</param>
        /// <returns>Returns the approximate size of the object within an <see cref="System.Int64"/> type.</returns>
        /// <remarks>The passed in object must be serializable.</remarks>
        public static long GetApproximateObjectSize(object inObject)
        {
            long size = 0;

            using (var memStream = new MemoryStream())
            {
                var bf = new BinaryFormatter();
                bf.Serialize(memStream, inObject);

                size = memStream.Length;
            }

            return size;
        }

        /// <summary>
        /// Compresses the specified content.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns>System.Byte[][].</returns>
        public byte[] Compress(string content)
        {
            var encoder = new ASCIIEncoding();

            char[] contextCharArray = content.ToCharArray();
            int ByteCount = encoder.GetByteCount(contextCharArray);

            byte[] buffer = CompressInternal(encoder.GetBytes(contextCharArray));
            encoder = null;

            return buffer;
        }

        /// <summary>
        /// Compresses the specified content.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns>System.Byte[][].</returns>
        public byte[] Compress(byte[] content)
        {
            return CompressInternal(content);
        }

        #endregion Public Methods and Operators

        #region Methods

        /// <summary>
        /// Compresses the internal.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns>System.Byte[][].</returns>
        private static byte[] CompressInternal(byte[] content)
        {
            var ms = new MemoryStream();
            var oCompress = new GZipStream(ms, CompressionMode.Compress, true);

            oCompress.Write(content, 0, content.Length);
            oCompress.Flush();
            oCompress.Close();
            byte[] buffer = null;
            long size = ms.Length;
            if (null != ms && ms.CanRead)
            {
                buffer = new byte[size];
                buffer = ms.ToArray();
                ms.Flush();
                ms.Close();
            }
            oCompress = null;
            ms = null;
            GC.Collect();
            return buffer;
        }

        /// <summary>
        /// Compresses the passed in <see cref="object"/> using binary serialization and <see cref="GZipStream"/> compression.
        /// </summary>
        /// <param name="uncompressedObject">The object to compress.</param>
        /// <param name="originalSize">Out Parameter that contains the size of the compressed data.</param>
        /// <returns>Returns an array of bytes that contains the compressed object.</returns>
        /// <remarks>The passed in object must be serializable.</remarks>
        private static byte[] CompressObjectInternal(object uncompressedObject, out long originalSize)
        {
            byte[] compressedBytes = null;

            originalSize = 0;

            using (var compressedMemoryStream = new MemoryStream())
            {
                using (var compressionStream = new GZipStream(compressedMemoryStream, CompressionMode.Compress, true))
                {
                    using (var memStream = new MemoryStream())
                    {
                        var bf = new BinaryFormatter();
                        bf.Serialize(memStream, uncompressedObject);

                        originalSize = memStream.Length;

                        memStream.Seek(0, SeekOrigin.Begin);
                        memStream.CopyTo(compressionStream);
                    }
                }

                compressedBytes = compressedMemoryStream.ToArray();
                compressedMemoryStream.Close();
            }

            return compressedBytes;
        }

        /// <summary>
        /// Decompresses the passed in byte array and deserializes the results to produce the object.
        /// </summary>
        /// <param name="compressedObjectBytes">The compressed object represented as a byte array.</param>
        /// <param name="uncompressedSize">Out Parameter that contains the size of the uncompressed data</param>
        /// <returns>A deserialized copy of the compressed data as a <see cref="System.Object"/>.</returns>
        /// <remarks>The compressed object is uncompressed and the results are deserialized using a binary formatter.</remarks>
        private static object DecompressObjectInternal(byte[] compressedObjectBytes, out long uncompressedSize)
        {
            object decompressedObject = null;

            uncompressedSize = 0;

            using (var decompressedMemStream = new MemoryStream())
            {
                using (
                    var decompressionStream = new GZipStream(
                        new MemoryStream(compressedObjectBytes),
                        CompressionMode.Decompress))
                {
                    decompressionStream.CopyTo(decompressedMemStream);
                }

                uncompressedSize = decompressedMemStream.Length;

                var bf = new BinaryFormatter();

                decompressedMemStream.Position = 0;
                decompressedObject = bf.Deserialize(decompressedMemStream);
            }

            return decompressedObject;
        }

        #endregion Methods
    }
}