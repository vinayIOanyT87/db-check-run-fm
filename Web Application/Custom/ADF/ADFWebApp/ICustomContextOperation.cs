using System;
using System.Collections.Generic;
using System.Text;

namespace ADFWebApp
{
    public interface ICustomContextOperation<T> //where T : ICustomPopupContext
    {
        T GetContext();
        void StoreContext(Object a_context);
        T LoadToContext(ref Object a_context);
        void LoadFromContext(T a_context);
    }
}
