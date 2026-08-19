using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMCore.Interfaces
{
    public interface IFMConfigurationManager
    {
        string Get(string key);
    }
}
