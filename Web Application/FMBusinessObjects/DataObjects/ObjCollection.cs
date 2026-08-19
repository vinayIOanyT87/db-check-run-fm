
namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections.Generic;

	[Serializable]
	public sealed class ObjCollection : List<ParamNameValue>
	{

		public void Add(string parameterName, object item)
		{
			if (item == null)
			{
				Add(new ParamNameValue { ParameterName = parameterName, Value = item });
				return;
			}
			Type complexType = item.GetType();
			if (complexType.IsPrimitive)
			{
				Add(new ParamNameValue { ParameterName = parameterName, Value = item });
				return;
			}
			if (complexType == typeof(string))
			{
				var strComplexObj = new ComplexObj { TypeName = complexType.ToString(), Xml = (string)item };
				Add(new ParamNameValue { ParameterName = parameterName, Value = strComplexObj.ToXML() });
				return;
			}
			if (complexType.IsSerializable)
			{
				var complexObj = new ComplexObj(item);
				Add(new ParamNameValue { ParameterName = parameterName, Value = complexObj.ToXML() });
				return;
			}
			throw new Exception("Object is not primitive or serializable");
		}

		public object GetParameterValue(string parameterName)
		{
			foreach (var obj in this)
			{
				if (obj.ParameterName == parameterName)
				{
					if (obj.Value == null)
					{
						return null;
					}
					if (obj.Value.GetType().IsPrimitive)
					{
						return obj.Value;
					}
					if (obj.Value.GetType() == typeof(string))
					{
						var complexVal = ComplexObj.FromXML((string)obj.Value);
						if (complexVal.TypeName == typeof(string).ToString())
						{
							return complexVal.Xml;
						}
						return complexVal.GetObject();
					}
				}
			}
			throw new Exception("Parameter " + parameterName + " not found!");
		}

		public bool HasParameter(string parameterName)
		{
			foreach (var obj in this)
			{
				if (obj.ParameterName == parameterName)
				{
					return true;
				}
			}
			return false;
		}

		public List<string> GetParameterNames()
		{
			List<string> ret = new List<string>();
			foreach (var obj in this)
			{
				ret.Add(obj.ParameterName);
			}
			return ret;
		}
	}
}
