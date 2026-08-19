using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMWebAPIBusinessLogic.Interfaces.Core
{
    public interface ICacheService
    {
        /// <summary>
        /// Saves the current object for the timeout specified.  If timeout is left null, it will cache for the system default cache time (2 minutes at this time)
        /// </summary>
        void Add<T>(string key, T toSave, TimeSpan? timeout = null) where T : class;

        /// <summary>
        /// Will return the item if possible, otherwise it will be null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        T Get<T>(string key) where T : class;

        /// <summary>
        /// will clear out the memory cache
        /// </summary>
        void Clear();

    }
}
