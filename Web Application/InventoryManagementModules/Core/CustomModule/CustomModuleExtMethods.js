var ModuleExtMethods = {

	StatusCodesBad: 2147483648,
	StatusCodesGood: 0,
	StatusCodesGoodLocalOverride: 9830400,
	StatusCodesUncertain: 1073741824,

	PointTagInputOutputTypeCalculated: 2,

	LimitBitsNone: 0,
	LimitBitsLow: 256,
	LimitBitsHigh: 512,


	IsPointTagValueGood: function (pointTag) {
		if (pointTag.Value == null || ((pointTag.Status & 0x80000000) !== 0)) {
			return false;
		}
		else {
			return true;
		}
	},

	IsPointTagValueOverridden: function (pointTag) {
		if ((pointTag.Status & 0xFFFF0000) === this.StatusCodesGoodLocalOverride) {
			return true;
		}
		else {
			return false;
		}
	},


	IsPointTagCalculated: function (pointTag) {
		if (pointTag == null)
			return false;
		return (pointTag.InputOutputType == this.PointTagInputOutputTypeCalculated);
	},

	IsPointTagStatusUncertain: function (pointTag) {
		if (pointTag == null)
			return false;

		if ((pointTag.Status & this.LimitBitsHigh) === this.LimitBitsHigh
		|| (pointTag.Status & this.LimitBitsLow) === this.LimitBitsLow
		|| (pointTag.Status & 0x40000000) === 0x40000000
		|| (pointTag.Status & 0xFFFF0000) === this.StatusCodesGoodLocalOverride) {
			return true;
		}
		else {
			return false;
		}
	},

	IsPointTagStatusChange: function (pointTag, newStatus) {
		if (pointTag == null)
			return false;
		return (Number(pointTag.Status) == newStatus);
	},

	IsGuidEqual: function (guid1, guid2) {
		return (String(guid1) == String(guid2));
	},

	CheckForAndSetOverUnderRange: function (pointTag) {
		var tagstatusCode = pointTag.Status;

		/*
		if (pointTag.Value > pointTag.Maximum)
		{
			tagstatusCode.LimitBits = LimitBitsHigh;
		}
		else if (pointTag.Value < pointTag.Minimum)
		{
			tagstatusCode.LimitBits = LimitBitsLow;
		}
		else
		{
			tagstatusCode.LimitBits = LimitBitsNone;
		}
		*/
		pointTag.Status = tagstatusCode;
	},

	GetDateTimeNow: function () {
		return new Date();
	},

	IsAlarmTestEnabled: function (targetTag, referenceTag) {
		var alarmMatch = false;

		if (!targetTag.AlarmsEnabled)
			return false;

		if (!referenceTag.AlarmsEnabled)
			return false;

		for (i = 0; i < referenceTag.Alarms.length; i++) {
			if (alarmMatch)
				break;
			var alarm = referenceTag.Alarms[i];
			var alarmTests = alarm.AlarmTests;
			for (j = 0; j < alarmTests.length; j++) {
				var alarmTest = alarmTests[j];
				if (ModuleExtMethods.IsGuidEqual(alarmTest.LimitTagGuid, targetTag.PointTagGuid)) {
					alarmMatch = true;
					if (!alarmTest.Enabled)
						return false;
					if (!alarm.Enabled)
						return false;
					break;
				}
			}
		}
		return true;
	}

}