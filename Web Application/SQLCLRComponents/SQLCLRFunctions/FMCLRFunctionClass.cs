/******************************************************************************
	FILE NAME:		FMCLRFunctionClass.cs
	PURPOSE:			FMCLRFunctionClass

	COMMENTS:
		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2000
		This file shall not be copied or reproduced in any form without
		the express written consent of Endress+Hauser.

	AUTHOR(S):	W. Gray
	VERSION:		1.0.0  Current version

	MODIFICATION HISTORY:
		Date:		By:					Reason:
		----------	-----------------	-------------------------------------------

		2009-03-24	W.Gray				7.4.6.2 - Correction to properly handle situation where Standard month follows
												Day Light Savings month such as occurs south of the equater 

**********************************************************************/
using System;
using System.Collections.Generic;
using Microsoft.SqlServer.Server;
using Microsoft.Win32;
using System.Diagnostics;


public class FMCLRFunctionClass
{
	//This function needs to be impletmented
	[SqlFunction(DataAccess = DataAccessKind.Read)] 
	public static float ConvertUnits(float FromValue,int FromUnitIndex, int ToUnitIndex)
	{
		return 0;
	}

	//This function needs to be impletmented
	[SqlFunction(DataAccess = DataAccessKind.Read)]
	public static float ConvertToSIUnits(float FromValue, int FromUnitIndex, int RoundFactor)
	{
		return 0;
	}

	//This function needs to be impletmented
	[SqlFunction(DataAccess = DataAccessKind.Read)]
	public static float ConvertFromSIUnits(float FromValue, int ToUnitIndex, int RoundFactor)
	{
		return 0;
	}

	[SqlFunction(DataAccess = DataAccessKind.Read)]
	public static int GetLocalOffset(DateTime dateTime, Boolean UtcDateTime, String StandardName, Boolean AdjustForDaylightSavings)
	{
		int[] DOM ={ 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
		int Displacement = 0;

		RegistryKey Key = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows NT\\CurrentVersion\\Time Zones\\" + StandardName);
		if (Key == null)
			return Displacement;

		RegistryKey DynamicDSTKey = Key.OpenSubKey("Dynamic DST");

		byte[] TZI = null;

		if (DynamicDSTKey != null)
		{
			int FirstEntry = (int)DynamicDSTKey.GetValue("FirstEntry");
			int LastEntry = (int)DynamicDSTKey.GetValue("LastEntry");

			if (dateTime.Year >= FirstEntry
			&& dateTime.Year <= LastEntry)
				TZI = (byte[])DynamicDSTKey.GetValue(dateTime.Year.ToString("D4"));

			else if (dateTime.Year < FirstEntry)
				TZI = (byte[])DynamicDSTKey.GetValue(FirstEntry.ToString("D4"));

			DynamicDSTKey.Close();
		}

		if (TZI == null)
			TZI = (byte[])Key.GetValue("TZI");

		Key.Close();

		if (TZI != null
		&& TZI.Length == 44)
		{
			int Bias = TZI[0] + (((int)TZI[1]) << 8) + (((int)TZI[2]) << 16) + (((int)TZI[3]) << 24);
			int StandardBias = TZI[4] + (((int)TZI[5]) << 8) + (((int)TZI[6]) << 16) + (((int)TZI[7]) << 24);
			int DaylightBias = TZI[8] + (((int)TZI[9]) << 8) + (((int)TZI[10]) << 16) + (((int)TZI[11]) << 24);

			int StandardYear = TZI[12] + (((int)TZI[13]) << 8);
			int StandardMonth = TZI[14];
			int StandardDayOfWeek = TZI[16];
			int StandardDay = TZI[18];
			int StandardHour = TZI[20];
			int StandardMin = TZI[22];
			int StandardSec = TZI[24];
			int StandardMil = TZI[26] + (((int)TZI[27]) << 8);

			int DaylightYear = TZI[28] + (((int)TZI[29]) << 8);
			int DaylightMonth = TZI[30];
			int DaylightDayOfWeek = TZI[32];
			int DaylightDay = TZI[34];
			int DaylightHour = TZI[36];
			int DaylightMin = TZI[38];
			int DaylightSec = TZI[40];
			int DaylightMil = TZI[42] + (((int)TZI[43]) << 8);

			Displacement = Bias;

			if (AdjustForDaylightSavings
			&& StandardMonth != 0
			&& DaylightMonth != 0
			&& (DaylightYear == 0
			|| DaylightYear == dateTime.Year))
			{
				DateTime localDateTime;
				if (UtcDateTime)
					localDateTime = dateTime.AddMinutes(-Displacement);
				else
					localDateTime = dateTime;


				bool LeapYear = false;

				if (localDateTime.Year % 4 == 0
				&& localDateTime.Year % 400 != 0)
					LeapYear = true;

				int DaysInMonth = DOM[localDateTime.Month - 1];
				if (LeapYear && localDateTime.Month == 2)
					DaysInMonth++;

				int FirstDayOfWeek = (int)localDateTime.DayOfWeek - ((localDateTime.Day % 7) - 1);

				if (FirstDayOfWeek < 0)
					FirstDayOfWeek += 7;

				if (FirstDayOfWeek > 6)
					FirstDayOfWeek -= 7;

				if (localDateTime.Month == StandardMonth)
				{
					// Determine the day of the month when DST ends
					int STDDay = 1 + 7 * (StandardDay - 1);

					if (FirstDayOfWeek > StandardDayOfWeek)
						STDDay += 7 - (FirstDayOfWeek - StandardDayOfWeek);
					else if (FirstDayOfWeek < StandardDayOfWeek)
						STDDay += StandardDayOfWeek - FirstDayOfWeek;

					if (STDDay > DaysInMonth)
						STDDay -= 7;

					if (localDateTime.Day < STDDay)
						Displacement += DaylightBias;

					else if (localDateTime.Day == STDDay)
					{
						if ((localDateTime.AddMinutes(-DaylightBias).Hour < StandardHour) &&
							(localDateTime.Day == localDateTime.AddMinutes(-DaylightBias).Day))
						{
							// Tricksy tricksy - we have to check that after applying the daylight bias that we are still on the same day;
							// otherwise we get caught where we are comparing hours of different days, which happens in the last bias minutes of
							// the day
							Displacement += DaylightBias;
						}

						else if (localDateTime.AddMinutes(-DaylightBias).Hour == StandardHour)
						{
							if (localDateTime.AddMinutes(-DaylightBias).Minute < StandardMin)
								Displacement += DaylightBias;
							else
								Displacement += StandardBias;

						}
						else
							Displacement += StandardBias;
					}

					else
						Displacement += StandardBias;
				}

				else if (localDateTime.Month == DaylightMonth)
				{
					// determine the day of the month when DST begins
					int DSTDay = 1 + 7 * (DaylightDay - 1);

					if (FirstDayOfWeek > DaylightDayOfWeek)
						DSTDay += 7 - (FirstDayOfWeek - DaylightDayOfWeek);
					else if (FirstDayOfWeek < DaylightDayOfWeek)
						DSTDay += DaylightDayOfWeek - FirstDayOfWeek;


					if (DSTDay > DaysInMonth)
						DSTDay -= 7;

					if (localDateTime.Day > DSTDay)
						Displacement += DaylightBias;

					else if (localDateTime.Day == DSTDay)
					{
						if (localDateTime.Hour > DaylightHour)
							Displacement += DaylightBias;

						else if (localDateTime.Hour == DaylightHour)
						{
							if (localDateTime.Minute >= DaylightMin)
								Displacement += DaylightBias;
						}
					}
				}

				else if (DaylightMonth < StandardMonth)
				{
					if (localDateTime.Month > DaylightMonth
					&& localDateTime.Month < StandardMonth)
						Displacement += DaylightBias;
				}

				else
				{
					if (localDateTime.Month < StandardMonth
					|| localDateTime.Month > DaylightMonth)
						Displacement += DaylightBias;
				}							
			}
		}

		return Displacement;
	}
}

