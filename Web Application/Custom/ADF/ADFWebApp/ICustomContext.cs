using System;
using System.Collections.Generic;
using System.Text;

namespace ADFWebApp
{
    public interface ICustomContext
    {
        string GetKey();
        void ResetContextProperties();
    }
}
