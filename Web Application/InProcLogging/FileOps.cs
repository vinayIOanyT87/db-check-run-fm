using HANDLE = System.IntPtr;


namespace InProcLogging
{
	using System;
	using System.Runtime.InteropServices;

	public class FileOps
    {
        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Auto)]
        public static extern HANDLE CreateFile(String lpFileName, UInt32 dwDesiredAccess, UInt32 dwShareMode, IntPtr lpSecurityAttributes, UInt32 dwCreationDisposition, UInt32 dwFlagsAndAttributes, IntPtr hTemplateFile);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool WriteFile(HANDLE hFile, byte[] lpBuffer, uint nNumberOfBytesToWrite, out uint lpNumberOfBytesWritten, IntPtr lpOverlapped);

        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(HANDLE hObject);

        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Auto)]
        public static extern uint GetFileSize(HANDLE hFile, out uint lpFileSizeHigh);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern uint SetFilePointer(HANDLE hFile, int lDistanceToMove, IntPtr lpDistanceToMoveHigh, uint dwMoveMethod);

        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool FlushFileBuffers(HANDLE hFile);

        public const uint FILE_ATTRIBUTE_NORMAL = 0x80;
        public const int INVALID_HANDLE_VALUE = -1;
        public const uint GENERIC_READ = 0x80000000;
        public const uint GENERIC_WRITE = 0x40000000;
        public const uint FILE_APPEND_DATA = 0x0004;
        public const uint CREATE_NEW = 1;
        public const uint CREATE_ALWAYS = 2;
        public const uint OPEN_EXISTING = 3;
        public const uint OPEN_ALWAYS = 4;
        public const uint FILE_SHARE_READ = 1;
        public const uint FILE_SHARE_WRITE = 2;
        public const uint FILE_END = 2;

        public static bool AppendFile(string aFileName, out HANDLE fd)
        {
            fd = CreateFile(aFileName, GENERIC_WRITE, FILE_SHARE_READ, IntPtr.Zero, OPEN_ALWAYS, FILE_ATTRIBUTE_NORMAL, IntPtr.Zero);
            if (INVALID_HANDLE_VALUE == fd.ToInt32())
            {
                return false;
            }
            if (SetFilePointer(fd, 0, IntPtr.Zero, FILE_END) == 0xFFFFFFFF)
            {
                CloseHandle(fd);
                return false;
            }
            return true;
        }

        public static bool WriteLogStatement(ref HANDLE fd, string aLogStatement, out uint aFileSize)
        {
            aFileSize = 0;
            try
            {
                char[] cBuff = aLogStatement.ToCharArray();
                byte[] buff = new byte[cBuff.Length];
                for (int i = 0; i < buff.Length; i++)
                {
                    buff[i] = (byte)cBuff[i];
                }
                int bytesToWrite = buff.Length;
                while (bytesToWrite > 0)
                {
                    uint bytesWritten = 0;
                    bool ret = WriteFile(fd, buff, (uint)bytesToWrite, out bytesWritten, IntPtr.Zero);
                    if (ret == false)
                    {
                        CloseFile(ref fd);
                        return false;
                    }
                    uint fileSizeHigh = 0;
                    aFileSize = GetFileSize(fd, out fileSizeHigh);
                    if (bytesToWrite > bytesWritten)
                    {
                        byte[] tempBuff = new byte[bytesToWrite - bytesWritten];
                        for (int j = (int)bytesWritten; j < bytesToWrite; j++)
                        {
                            tempBuff[j] = buff[j];
                        }
                        buff = tempBuff;
                    }
                    bytesToWrite -= (int)bytesWritten;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                CloseFile(ref fd);
                return false;
            }
            return true;
        }

        public static bool CloseFile(ref HANDLE fd)
        {
            bool retVal = false;
            if (INVALID_HANDLE_VALUE != fd.ToInt32())
            {
                retVal &= CloseHandle(fd);
                fd = (HANDLE)INVALID_HANDLE_VALUE;
            }
            return retVal;
        }

    }
}
