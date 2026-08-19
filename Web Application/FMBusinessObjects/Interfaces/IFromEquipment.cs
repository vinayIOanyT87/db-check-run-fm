using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FMBusinessObjects.Interfaces
{
	public interface IFromEquipment
	{
		string RegistrationID { get; set; }
		string SerialNumber { get; set; }
	}
}
