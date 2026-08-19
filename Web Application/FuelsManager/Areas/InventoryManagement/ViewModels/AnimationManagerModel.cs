

namespace FuelsManager.Areas.InventoryManagement.ViewModels
{
	using System;
	using System.Collections.Generic;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.DataObjects;

	[Serializable]
	public class AnimationPointValue
	{
		public Guid AnimationTestGroupGuid;

		public string DataType;

		public Guid PointValueGuid;

		public Guid PointGuid;

		public PointValueFieldType Field;

		public bool PointValueIsFromTemplate;

		public PointValueType ValueType;

		public string PointID;

		public string PointValueID;
	}

	[Serializable]
	public class AnimationPointValueList
	{
		public Guid AnimationGuid;

		public string AnimationID;

		public List<AnimationPointValue> TestGroupPointValueInfoList;
	}

	[Serializable]
	public class AnimationManagerPropertyVisualState
	{

		public Guid AnimationPropertyVisualStateGuid;

		public string Value;
	}

	[Serializable]
	public class AnimationManagerProperty
	{

		public Guid AnimationPropertyGuid;

		public string Name;

		public string LookupName;

		public string gojsPropertyName;

		public List<AnimationManagerPropertyVisualState> VisualStates;

		public string Value
		{
			get
			{
				string valueStr = string.Empty;
				if (this.VisualStates.Count > 1)
				{
					valueStr += "{";
				}
				for (var i = 0; i < this.VisualStates.Count; i++)
				{
					if (i != 0)
					{
						valueStr += ", ";
					}
					valueStr += this.VisualStates[i].Value;
				}
				if (this.VisualStates.Count > 1)
				{
					valueStr += "}";
				}
				return valueStr;
			}
		}
	}

	[Serializable]
	public class AnimationManagerModelTest
	{
		public Guid AnimationTestGuid;

		public EAnimationTestComparisonOperators TestComparisonOperator;

		public string BitmaskStr;

		public long Bitmask;

		public EAnimationTestBitmaskOperators BitmaskOperator;

		public string ComparisonValue;

		//Properties

		public List<AnimationManagerProperty> PropertyList;

		protected string GetPropertiesString()
		{
			string propertiesString = "";
			var propertiesList = this.PropertyList;
			foreach (var property in propertiesList)
			{
				if (propertiesString.Length > 0)
				{
					propertiesString += ", ";
				}
				propertiesString += property.Name + ": " + property.Value;
			}
			return propertiesString;
		}

		protected string GetComparisonOperatorString()
		{
			string comparisonOperatorString = "";
			switch (this.TestComparisonOperator)
			{
				case EAnimationTestComparisonOperators.GreaterThan:
					comparisonOperatorString = ">";
					break;
				case EAnimationTestComparisonOperators.GreaterThanOrEqual:
					comparisonOperatorString = ">=";
					break;
				case EAnimationTestComparisonOperators.LessThan:
					comparisonOperatorString = "<";
					break;
				case EAnimationTestComparisonOperators.LessThanOrEqual:
					comparisonOperatorString = "<=";
					break;
				case EAnimationTestComparisonOperators.Equals:
					comparisonOperatorString = "=";
					break;
				case EAnimationTestComparisonOperators.NotEqual:
					comparisonOperatorString = "<>";
					break;
				case EAnimationTestComparisonOperators.Else:
					comparisonOperatorString = "ELSE";
					break;
				case EAnimationTestComparisonOperators.Contains:
					comparisonOperatorString = "Contains";
					break;
				case EAnimationTestComparisonOperators.BeginsWith:
					comparisonOperatorString = "Begins With";
					break;
				default:
					comparisonOperatorString = "UNKNOWN";
					break;
			}
			return comparisonOperatorString;
		}

		protected string GetBitwiseOperatorString()
		{
			string bitwiseOperatorString = "";
			switch (this.BitmaskOperator)
			{
				case EAnimationTestBitmaskOperators.And:
					bitwiseOperatorString = "AND";
					break;
				case EAnimationTestBitmaskOperators.Or:
					bitwiseOperatorString = "OR";
					break;
				case EAnimationTestBitmaskOperators.Nand:
					bitwiseOperatorString = "NAND";
					break;
				case EAnimationTestBitmaskOperators.Nor:
					bitwiseOperatorString = "NOR";
					break;
				case EAnimationTestBitmaskOperators.Xand:
					bitwiseOperatorString = "XAND";
					break;
				case EAnimationTestBitmaskOperators.Xor:
					bitwiseOperatorString = "XOR";
					break;
				case EAnimationTestBitmaskOperators.None:
					bitwiseOperatorString = "NONE";
					break;
				default:
					bitwiseOperatorString = "UNKNOWN";
					break;
			}
			return bitwiseOperatorString;
		}

		protected string GetBitwiseOperationString()
		{
			if (this.BitmaskOperator == EAnimationTestBitmaskOperators.None)
			{
				return string.Empty;
			}
			return this.GetBitwiseOperatorString() + " " + this.Bitmask + " ";
		}


		public string GetUiString()
		{
			string uiString = "";
			if (this.TestComparisonOperator == EAnimationTestComparisonOperators.Else)
			{
				uiString = this.GetComparisonOperatorString();
			}
			else
			{
				uiString = "Test " + this.GetBitwiseOperationString() + this.GetComparisonOperatorString() + " " + this.ComparisonValue;
			}
			return uiString;
		}
	}

	[Serializable]
	public class AnimationManagerModelTestGroup
	{
		public Guid AnimationTestGroupGuid;

		public string ID;

		public string DataType;

		public Guid PointValueGuid;

		public Guid PointGuid;

		public PointValueFieldType Field;

		public bool PointValueIsFromTemplate;

		public string PointValueAndFieldID;

		public PointValueType ValueType;

		public string PointID;

		public string PointValueID;

		public List<AnimationManagerModelTest> TestList;
	}

	[Serializable]
	public class AnimationManagerModelAnimation
	{

		public Guid AnimationGuid;

		public string ID;

		public int UseCount;

		public List<AnimationManagerModelTestGroup> AnimationTestGroups;

		public AnimationManagerModelTestGroup GetAnimationTestGroup(Guid testGroupGuid)
		{
			foreach (var testGroup in this.AnimationTestGroups)
			{
				if (testGroup.AnimationTestGroupGuid == testGroupGuid)
				{
					return testGroup;
				}
			}
			return null;
		}
	}

	[Serializable]
	public class AnimationManagerModel
	{

		public List<AnimationManagerModelAnimation> AnimationList;

		public List<KeyValuePair<string, string>> ValidTestGroupDataTypeList; 

		public Guid SelectedAnimationGuid;

		public string TranslatedTextForValueField;

		public string TranslatedTextForIDField;

		public string TranslatedTextForTimestampField;

		public string TranslatedTextForUnitsField;

		public string TranslatedTextForAlarmStatusField;

		public AnimationPointValueList PointValueList;

		public void SortAnimationsAlphabetically()
		{
			this.AnimationList.Sort((x, y) => x.ID.CompareTo(y.ID));
		}

		public AnimationManagerModelAnimation GetAnimation(Guid animationGuid)
		{
			foreach (var anime in this.AnimationList)
			{
				if (anime.AnimationGuid == animationGuid)
				{
					return anime;
				}
			}
			return null;
		}

	}
}
