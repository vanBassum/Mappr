using System.Diagnostics;
using System.Text;

namespace Mappr.Kernel
{

    public class MemoryManager
    {
        Process? process;
        IntPtr processHandle;

        #region ProcessAttach

        public bool attach(string processname)
        {
            try
            {
                process = Process.GetProcessesByName(processname).FirstOrDefault();
                if (process == null)
                    return false;
                processHandle = Kernel32.OpenProcess(Kernel32.PROCESS_WM_READ | Kernel32.PROCESS_WM_WRITE | Kernel32.PROCESS_WM_OPERATION, false, process.Id);
                return true;
            }
            catch
            {
                if (process != null)
                {
                    process.Close();
                    process.Dispose();
                }
            }

            return false;
        }

        public bool IsAttached()
        {
            if (process != null)
            {
                return true;
            }
            return false;
        }

        public bool detach()
        {
            try
            {
                if (process != null)
                {
                    process.Close();
                    process.Dispose();
                }
                return true;
            }
            catch
            {
            }

            return false;
        }



        #endregion

        public nint GetProcessBase()
        {
            return process?.MainModule?.BaseAddress ?? nint.Zero;
        }

        public nint GetProcessModuleBase(string moduleName)
        {
            var mod = process?.Modules.Cast<ProcessModule>().Where(p => p.ModuleName == moduleName).FirstOrDefault();
            if (mod != null)
                return mod.BaseAddress;
            return nint.Zero;
        }


        #region Read
        public byte[] Read_Bytes(IntPtr address, int length)
        {
            byte[] buffer = new byte[length];
            IntPtr bytesRead = IntPtr.Zero;
            Kernel32.ReadProcessMemory(processHandle, address, buffer, buffer.Length, out bytesRead);
            return buffer;
        }

        public IntPtr Read_Address(IntPtr address)
        {
            byte[] buffer = Read_Bytes(address, IntPtr.Size);
            if (IntPtr.Size == 4)
                return new IntPtr(Read_Int32(address));
            else
                return new IntPtr(Read_Int64(address));
        }

        public int Read_Int32(IntPtr address)
        {
            byte[] buffer = Read_Bytes(address, 4);
            return BitConverter.ToInt32(buffer, 0);
        }

        public long Read_Int64(IntPtr address)
        {
            byte[] buffer = Read_Bytes(address, 8);
            return BitConverter.ToInt64(buffer, 0);
        }

        public float Read_Float(IntPtr address)
        {
            byte[] buffer = Read_Bytes(address, 4);
            return BitConverter.ToSingle(buffer, 0);
        }

        public double Read_Double(IntPtr address)
        {
            byte[] buffer = Read_Bytes(address, 4);
            return BitConverter.ToDouble(buffer, 0);
        }

        public string Read_String(IntPtr address, int strLength)
        {
            byte[] buffer = Read_Bytes(address, strLength);
            // Convert the byte array to ASCII string
            string str = Encoding.ASCII.GetString(buffer);
            // Find the index of the null character ('\0')
            int nullCharIndex = str.IndexOf('\0');
            if (nullCharIndex >= 0)
            {
                // If null character found, return substring up to that point
                return str.Substring(0, nullCharIndex);
            }
            // If null character not found, return the full string
            return str;
        }

        public byte Read_Byte(IntPtr address)
        {
            byte[] buffer = Read_Bytes(address, 1);
            return buffer[0];
        }
        #endregion



    }

}
