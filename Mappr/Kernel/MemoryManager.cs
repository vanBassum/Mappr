using System.Configuration;
using System.Diagnostics;
using System.Net;
using System.Text;
using Mappr.Kernel.DataConverters;

namespace Mappr.Kernel
{
    public class MemoryManager : IDisposable
    {
        private readonly Dictionary<Type, IMemoryReader> _converters;
        private Process? _process;
        private nint _processHandle;

        private IMemoryReader[] basicConverters = {
            new IntMemReader(),
            new FloatMemReader(),
            new Vector3MemReader(),
            new Vector4MemReader(),
            new QuaternionMemReader(),
            new StringMemReader(),
        };


        public MemoryManager(IEnumerable<IMemoryReader> converters)
        {
            _converters = converters.ToDictionary(converter => converter.DataType, converter => converter);

            foreach(var converter in basicConverters)
            {
                if (!_converters.ContainsKey(converter.DataType))
                    _converters.Add(converter.DataType, converter);
            }
        }

        #region ProcessAttach
        public bool AttachToProcess(string processName)
        {
            try
            {
                _process = Process.GetProcessesByName(processName).FirstOrDefault();
                if (_process == null)
                {
                    return false;
                }

                _processHandle = Kernel32.OpenProcess(Kernel32.PROCESS_WM_READ | Kernel32.PROCESS_WM_WRITE | Kernel32.PROCESS_WM_OPERATION, false, _process.Id);
                return true;
            }
            catch (Exception ex)
            {
                // Log or handle exception appropriately
                Console.WriteLine($"Error attaching to process: {ex.Message}");
                return false;
            }
        }

        public bool IsAttached => _process != null;

        public bool DetachFromProcess()
        {
            try
            {
                _process?.Dispose();
                _process = null;
                return true;
            }
            catch (Exception ex)
            {
                // Log or handle exception appropriately
                Console.WriteLine($"Error detaching from process: {ex.Message}");
                return false;
            }
        }
#endregion



        public nint GetProcessBase()
        {
            return _process?.MainModule?.BaseAddress ?? nint.Zero;
        }

        public nint GetProcessModuleBase(string moduleName)
        {
            var module = _process?.Modules.Cast<ProcessModule>().FirstOrDefault(p => p.ModuleName == moduleName);
            return module?.BaseAddress ?? nint.Zero;
        }

        public T? Read<T>(nint address)
        {
            var type = typeof(T);
            if (!_converters.TryGetValue(type, out var converterObj))
                throw new NotSupportedException($"Type {type} is not supported.");

            IMemoryReader<T> converter = (IMemoryReader<T>)converterObj;
            return converter.Convert(this, address);
        }

        public nint ReadAddress(nint address)
        {
            byte[] buffer = ReadBytes(address, nint.Size);
            return nint.Size == 4 ? (nint)BitConverter.ToUInt32(buffer, 0) : (nint)BitConverter.ToUInt64(buffer, 0);
        }

        public nint ReadChain(params nint[] addressChain)
        {
            nint address = 0;

            foreach(var v in addressChain)
            {
                address = ReadAddress(address + v);
                if (address == 0)
                    return 0;
            }
                
            return address;
        }

        public byte[] ReadBytes(nint address, int length)
        {
            var buffer = new byte[length];

            if (!Kernel32.ReadProcessMemory(_processHandle, address, buffer, buffer.Length, out nint bytesRead))
                throw new InvalidOperationException($"Failed to read memory at address {address}");

            return buffer;
        }

        public void Dispose()
        {
            DetachFromProcess();
        }
    }
}
