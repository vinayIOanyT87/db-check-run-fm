using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FMLegacyTestApp
{
    public static class Marshaller
    {
        public static IntPtr IntPtrFromStuctArray<T>(T[] InputArray) where T : new()
        {
            int count = InputArray.Length;

            T[] resArray = new T[count];

            IntPtr rRoot = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(T)) * count);

            for (int i = 0; i < count; i++)
            {
                Marshal.StructureToPtr(InputArray[i], (IntPtr)(rRoot.ToInt32() + i * Marshal.SizeOf(InputArray[i])), false);
            }

            return rRoot;
        }

        public static T[] StuctArrayFromIntPtr<T>(IntPtr rRoot, int count) where T : new()
        {
            T[] resArray = new T[count];
            IntPtr current = rRoot;
            //get the output array of pointers
            //IntPtr[] OutPointers = new IntPtr[count];
            //Marshal.Copy(rRoot, OutPointers, 0, count);

            for (int i = 0; i < count; i++)
            {
                resArray[i] = new T();
                //current = OutPointers[i];
                Marshal.PtrToStructure(current, resArray[i]);
                Marshal.DestroyStructure(current, typeof(T));
                int structsize = Marshal.SizeOf(resArray[i]);
                current = (IntPtr)((long)current + structsize);
            }
            Marshal.FreeCoTaskMem(rRoot);
            return resArray;
        }

        public static MOVEMENTDATA BuildMovementData(MOVEINSTANCEDATA mvRecord)
        {
            MOVEMENTDATA moveData = new MOVEMENTDATA(mvRecord);
            string[] szUserDefines = IntPtrToStringArray<byte>(10, mvRecord.szUserDef);//, 62);
            moveData.szUserDef = szUserDefines;
            //moveData.szUserDef = new string[10];
            NODEINSTANCEDATA[] nodeData = StuctArrayFromIntPtr<NODEINSTANCEDATA>(mvRecord.pNodeInstanceData, mvRecord.wNumberOfNodes);
            moveData.NodeInstanceData = nodeData;
            return moveData;
        }

        public static MOVEINSTANCEDATA[] BuildMovementData(MOVEMENTDATA mvRecord)
        {
            MOVEINSTANCEDATA moveData = new MOVEINSTANCEDATA(mvRecord);
            IntPtr szUserDefPtr = StringArrayToIntPtr<byte>(mvRecord.szUserDef);//, 62);
            moveData.szUserDef = szUserDefPtr;
            IntPtr nodeDataPtr = IntPtrFromStuctArray<NODEINSTANCEDATA>(mvRecord.NodeInstanceData);
            moveData.pNodeInstanceData = nodeDataPtr;
            return new MOVEINSTANCEDATA[] { moveData };
        }

        public static IntPtr StringArrayToIntPtr<GenChar>(string[] InputStrArray) where GenChar : struct
        {
            int count = InputStrArray.Length;

            if (count == 0) return IntPtr.Zero;

            //build array of pointers to string
            IntPtr[] InPointers = new IntPtr[count];
            int dim = IntPtr.Size * count;
            IntPtr rRoot = Marshal.AllocCoTaskMem(dim);

            for (int i = 0; i < count; i++)
            {
                if (typeof(GenChar) == typeof(char))
                    InPointers[i] = Marshal.StringToCoTaskMemAnsi(InputStrArray[i]);
                else if (typeof(GenChar) == typeof(byte))
                    InPointers[i] = Marshal.StringToCoTaskMemUni(InputStrArray[i]);
                else if (typeof(GenChar) == typeof(IntPtr))//assume BSTR for IntPtr param
                    InPointers[i] = Marshal.StringToBSTR(InputStrArray[i]);
            }

            //copy the array of pointers
            Marshal.Copy(InPointers, 0, rRoot, count);
            return rRoot;
        }

        public static string[] IntPtrToStringArray<GenChar>(int count, IntPtr rRoot) where GenChar : struct
        {
            if (rRoot == IntPtr.Zero) return new string[] { };

            //get the output array of pointers
            IntPtr[] OutPointers = new IntPtr[count];
            Marshal.Copy(rRoot, OutPointers, 0, count);
            string[] OutputStrArray = new string[count];

            for (int i = 0; i < count; i++)
            {
                if (typeof(GenChar) == typeof(char))
                    OutputStrArray[i] = Marshal.PtrToStringAnsi(OutPointers[i]);
                else if (typeof(GenChar) == typeof(byte))
                    OutputStrArray[i] = Marshal.PtrToStringUni(OutPointers[i]);
                else if (typeof(GenChar) == typeof(IntPtr))//assume BSTR for IntPtr param
                    OutputStrArray[i] = Marshal.PtrToStringBSTR(OutPointers[i]);
                //dispose of unneeded memory
                Marshal.FreeCoTaskMem(OutPointers[i]);
            }

            //dispose of the pointers array
            Marshal.FreeCoTaskMem(rRoot);
            return OutputStrArray;
        }

        public static string[] IntPtrToStringArray<GenChar>(int count, IntPtr rRoot, int size) where GenChar : struct
        {
            if (rRoot == IntPtr.Zero) return new string[] { };

            //get the output array of pointers
            string[] OutputStrArray = new string[count];
            byte[] managedBuffer = new byte[size];

            GCHandle pinnedArray = GCHandle.Alloc(managedBuffer, GCHandleType.Pinned);
            IntPtr managedBufferPtr = pinnedArray.AddrOfPinnedObject();

            for (int i = 0; i < count; i++)
            {
                Marshal.Copy(rRoot + (i * size), managedBuffer, 0, size);
                if (typeof(GenChar) == typeof(char))
                    OutputStrArray[i] = Marshal.PtrToStringAnsi(managedBufferPtr);
                else if (typeof(GenChar) == typeof(byte))
                    OutputStrArray[i] = Marshal.PtrToStringUni(managedBufferPtr);
                else if (typeof(GenChar) == typeof(IntPtr))//assume BSTR for IntPtr param
                    OutputStrArray[i] = Marshal.PtrToStringBSTR(managedBufferPtr);
            }
            pinnedArray.Free();
            managedBufferPtr = IntPtr.Zero;

            //dispose of the pointers array
            Marshal.FreeCoTaskMem(rRoot);
            return OutputStrArray;
        }

        public static IntPtr StringArrayToIntPtr<GenChar>(string[] InputStrArray, int size) where GenChar : struct
        {
            int count = InputStrArray.Length;

            if (count == 0) return IntPtr.Zero;

            //build array of pointers to string
            int dim = size * count;

            IntPtr rRoot = Marshal.AllocCoTaskMem(dim);

            byte[] EmptyArray = new byte[dim];
            Array.Clear(EmptyArray, 0, EmptyArray.Length);
            
            // clear out the memory
            Marshal.Copy(EmptyArray, 0, rRoot, dim);

            for (int i = 0; i < count; i++)
            {
                byte[] managedBuffer = Encoding.Unicode.GetBytes(InputStrArray[i]);
                Marshal.Copy(managedBuffer, 0, rRoot + (i * size), managedBuffer.Length < size ? managedBuffer.Length : size);
            }

            //copy the array of pointers
            return rRoot;
        }
    }
}
