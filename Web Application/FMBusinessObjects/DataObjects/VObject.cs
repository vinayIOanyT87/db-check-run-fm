using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FMBusinessObjects.DataObjects
{
    public class VObject
    {
        public static object GetValue( object o )
        {
            return (o == null) ? System.DBNull.Value : o;
        }
    }
}
