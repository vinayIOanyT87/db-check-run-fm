using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FMBusinessObjects.DataObjects.CodedVariables
{
	public class SelectList
	{
		protected static bool IsNumber(char c)
		{
			int asciiCode = (int)c;
			return (asciiCode >= 48 && asciiCode <= 57);
		}

		protected static bool IsCapitalLetter(char c)
		{
			int asciiCode = (int)c;
			return (asciiCode >= 65 && asciiCode <= 90);
		}

		protected static bool IsLowerCaseLetter(char c)
		{
			int asciiCode = (int)c;
			return (asciiCode >= 97 && asciiCode <= 122);
		}

		protected static char CapitalizeLetter(char c)
		{
			int cInt = (int)c;
			int capitalCInt = cInt - 32;
			char retChar = (char)capitalCInt;
			return retChar;
		}

		protected static int GetIndexOfNextWord(string workStr)
		{
			bool wordStartsWithNumber = false;
			bool previousCharWasCapitalLetter = true;
			wordStartsWithNumber = IsNumber(workStr[0]);
			for (int i = 1; i < workStr.Length; i++)
			{
				if (!wordStartsWithNumber)
				{
					if (IsNumber(workStr[i]))
					{
						return i;
					}
					if (IsCapitalLetter(workStr[i]))
					{
						if (!previousCharWasCapitalLetter)
						{
							return i;
						}

					}
					else
					{
						previousCharWasCapitalLetter = false;
					}
				}
				else
				{
					if (!IsNumber(workStr[i]))
					{
						return i;
					}
				}
			}
			return -1;
		}

		protected static string CapitalizeFirstLetter(string workStr)
		{
			if (IsLowerCaseLetter(workStr[0]))
			{
				string ret = "";
				ret += CapitalizeLetter(workStr[0]);
				if (workStr.Length > 1)
				{
					ret += workStr.Substring(1);
				}
				return ret;
			}
			return workStr;
		}

		protected static string GetHead(ref string workStr)
		{
			string ret = "";
			if (workStr.Length <= 1)
			{
				ret = workStr;
				ret = CapitalizeFirstLetter(ret);
				workStr = string.Empty;
				return ret;
			}
			int index = GetIndexOfNextWord(workStr);
			if (index < 0)
			{
				ret = workStr;
				ret = CapitalizeFirstLetter(ret);
				workStr = string.Empty;
				return ret;
			}
			ret = workStr.Substring(0, index);
			ret = CapitalizeFirstLetter(ret);
			workStr = workStr.Substring(index);
			return ret;

		}
		public static string CreateUIString(Enum val)
		{
			bool firstTimeThroughStringArr = true;
			string ret = "";
			if (val != null)
			{
				var stringArr = val.ToString().Split('_');
				foreach (var stringElement in stringArr)
				{
					if (!firstTimeThroughStringArr)
					{
						ret += " ";
					}
					else
					{
						firstTimeThroughStringArr = false;
					}
					string workStr = stringElement;
					bool firstTimeThroughGetHead = true;
					while (!string.IsNullOrEmpty(workStr))
					{
						if (!firstTimeThroughGetHead)
						{
							ret += " ";
						}
						else
						{
							firstTimeThroughGetHead = false;
						}
						ret += GetHead(ref workStr);
					}
				}
			}
			return ret;
		}

		public static List<Tuple<Enum, string>> Of<T>() where T : struct, IConvertible
		{
			Type t = typeof(T);
			if (t.IsEnum)
			{
				var values = Enum.GetValues(t).Cast<Enum>();
				var EnumUIStringList = new List<Tuple<Enum, string>>();
				foreach (var val in values)
				{
					EnumUIStringList.Add(new Tuple<Enum, string>(val, CreateUIString(val)));
				}
				return EnumUIStringList;

			}
			throw new ArgumentException("<T> must be an enumerated type.");
		}

		public static List<string> GetDllEnumUIStrings(string path, string nmSpace)
		{
			var dynEnt = Assembly.LoadFrom(path);
			var tArr = dynEnt.GetTypes();
			return GetDllEnumUIStrings(tArr, nmSpace);
		}

		public static List<string> GetDllEnumUIStrings(Type[] tArr, string nmSpace)
		{
			List<string> ret = new List<string>();
			int exceptionCount = 0;
			foreach (var t in tArr)
			{
				if (t.DeclaringType == null && t.IsEnum && t.Namespace == nmSpace)
				{
					try
					{
						var values = Enum.GetValues(t).Cast<Enum>();
						foreach (var val in values)
						{
							ret.Add(CreateUIString(val));
						}
					}
					catch (Exception)
					{
						exceptionCount++;
						//System.Console.WriteLine("Exception Number " + exceptionCount + " : Type Name is " + t.Name);
					}
				}
			}

			return ret;
		}
	}
}
