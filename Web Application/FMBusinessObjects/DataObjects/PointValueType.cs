namespace FMBusinessObjects.DataObjects
{
	using System;

	public enum PointValueType
	{
		Tag = 0,
		Setting = 1,
        Point = 2,
        All = 3
	}

	public enum PointValueFieldType
	{
		VALUE = 0,
		ID = 1,
		TIMESTAMP = 2,
		UNITS = 3,
		ALARMSTATUS = 4
	}

	public enum EAnimationTestComparisonOperators
	{
		GreaterThan = 0,
		GreaterThanOrEqual = 1,
		LessThan = 2,
		LessThanOrEqual = 3,
		Equals = 4,
		NotEqual = 5,
		Else = 6,
		Contains = 7,
		BeginsWith = 8
	}

	public enum EAnimationTestBitmaskOperators
	{
		And = 0,
		Or = 1,
		Nand = 2,
		Nor = 3,
		Xand = 4,
		Xor = 5,
		None = 6
	}
}
