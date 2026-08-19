using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FMBusinessObjects.DataObjects
{
	[DataContract]
   [Serializable]
	public class UserDataClass
	{
		protected const int MAX_INDEX = 24;

		[DataMember]
		public string[] UserData = new string[MAX_INDEX];

		public UserDataClass()
		{
			for (int index = 0; index < MAX_INDEX; index++)
			{
				UserData[index] = "";
			}
		}

		public string this[int index]
		{
			get
			{
				if (index > -1 && index < MAX_INDEX)
				{
					return UserData[index];
				}
				else
				{
					throw new InvalidOperationException ( "UserDataClass.set_Item Index Out of Range" );
				}
			}
			set
			{
				if (index > -1 && index < MAX_INDEX)
				{
					if (value.Length > 60)
					{
						throw new Exception ( "[User Data " + ( index + 1 ).ToString ( ) + "], [maximum length of] 60 [exceeded]" );
					}

					UserData[index] = value;
				}
				else
				{
					throw new InvalidOperationException ( "UserDataClass.set_Item Index Out of Range" );
				}
			}
		}
	}
}
