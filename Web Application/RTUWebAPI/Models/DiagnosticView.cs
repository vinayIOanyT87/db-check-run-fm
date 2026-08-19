using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RTUWebAPI.Models
{
    public class DiagnosticView
    {
        public string id { get; set; }
        public Parameter[] parameters { get; set; }
        public IFilterCollection filterCollection { get; set; }


    }

    public class IFilterCollection
    {
        public enum dataType
        {
            STATIC = 0, NUMERIC = 1, TIMESTAMP = 2, STRING = 3
        }

        public IFilter[] filters; 
    }

    public class IFilter
    {
        public enum operatorType
        { 
            IF,AND,OR,NOT
        }
        public enum comparator
        {
            GREATERTHAN, GREATERTHANOREQUALTO, EQUALTO, LESSTHANOREQUALTO, LESSTHAN, CONTAINS
        }
    }
}
