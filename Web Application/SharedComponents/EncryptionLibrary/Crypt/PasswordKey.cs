using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using Crypt.Interfaces;

namespace Crypt
{
    public class PasswordKey : IKey
    {
        protected byte[] m_key;

        public void Dispose()
        {
            m_key = null;
            System.GC.Collect();
        }

        public PasswordKey()
        {
            GenerateKey();
        }

        public PasswordKey(PasswordKey a_key)
        {
            Clone(a_key.ToBytes());
        }

        public PasswordKey(byte[] a_key)
        {
            Clone(a_key);
        }

        protected void Clone(byte[] a_key)
        {
            m_key = a_key;
        }

        public byte[] ToBytes()
        {
            return m_key;
        }

        public void GenerateKey()
        {
            RandomNumberGenerator gen = RandomNumberGenerator.Create();
            
            m_key = new byte[32];
            gen.GetBytes(m_key);
            
        }
    }
}
