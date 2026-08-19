using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crypt.Interfaces
{
    public interface IEncryptor
    {
        byte[] Encrypt(byte[] a_pt, IKey a_key);
        byte[] Decrypt(byte[] a_ct, IKey a_key);
    }
}
