
namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.IO;
	using System.Web.UI.WebControls;
	using System.Xml.Serialization;

	using FMBusinessObjects.UtilityObjects;

	[Serializable]
	public class AsyncMethodCallClass
	{
		public Guid SiteGuid;

		public Guid PointGuid;

		public Guid ModuleCalculationGuid;

		public ParameterCollection Parameters;

		public AsyncMethodCallClass()
		{
		}

		public AsyncMethodCallClass(Guid siteGuid, Guid pointGuid, Guid moduleCalculationGuid, ParameterCollection parameters)
		{
			SiteGuid = siteGuid;
			PointGuid = pointGuid;
         ModuleCalculationGuid = moduleCalculationGuid;
			Parameters = parameters;
		}

		public string ToXML()
		{
			var serializer = CachingXmlSerializerFactory.Create(typeof(AsyncMethodCallClass));
			var tempWriter = new StringWriter();
			serializer.Serialize(tempWriter, this);
			return tempWriter.ToString();
		}

		public static AsyncMethodCallClass FromXML(string aXmlString)
		{
			try
			{
				var serializer = CachingXmlSerializerFactory.Create(typeof(AsyncMethodCallClass));
				var tempReader = new StringReader(aXmlString);
				var ret = serializer.Deserialize(tempReader) as AsyncMethodCallClass;
				return ret;
			}
			catch (Exception)
			{
				return null;
			}
		}

	}
}
