using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Serialization;

namespace FMBusinessObjects.UtilityObjects
{

	// Implemented a factory to create XMLSerializers that include its own caching instead of depending on the default
	// There is a known memory leak in the default implementation of XMLSerializer and this is a workaround
	public static class CachingXmlSerializerFactory
	{
		private static readonly Dictionary<string, XmlSerializer> Cache = new Dictionary<string, XmlSerializer>();

		private static readonly object SyncRoot = new object();

		public static XmlSerializer Create(Type type, XmlRootAttribute root)
		{
			if (type == null) throw new ArgumentNullException("type");
			if (root == null) throw new ArgumentNullException("root");

			var key = String.Format(CultureInfo.InvariantCulture, "{0}:{1}", type, root.ElementName);

			lock (SyncRoot)
			{
				if (!Cache.ContainsKey(key))
				{
					Cache.Add(key, new XmlSerializer(type, root));
				}
			}

			return Cache[key];
		}

		public static XmlSerializer Create<T>(XmlRootAttribute root)
		{
			return Create(typeof(T), root);
		}

		public static XmlSerializer Create<T>()
		{
			return Create(typeof(T));
		}

		public static XmlSerializer Create<T>(string defaultNamespace)
		{
			return Create(typeof(T), defaultNamespace);
		}

		public static XmlSerializer Create(Type type)
		{
			if (type == null) throw new ArgumentNullException("type");

			var key = String.Format(CultureInfo.InvariantCulture, "{0}", type.Name);

			lock (SyncRoot)
			{
				if (!Cache.ContainsKey(key))
				{
					Cache.Add(key, new XmlSerializer(type));
				}
			}

			return Cache[key];
		}

		public static XmlSerializer Create(Type type, string defaultNamespace)
		{
			if (type == null) throw new ArgumentNullException("type");
			if (defaultNamespace == null) throw new ArgumentNullException("defaultNamespace");

			var key = String.Format(CultureInfo.InvariantCulture, "{0}:{1}", type, defaultNamespace);

			lock (SyncRoot)
			{
				if (!Cache.ContainsKey(key))
				{
					Cache.Add(key, new XmlSerializer(type, defaultNamespace));
				}
			}

			return Cache[key];
		}
	}
}
