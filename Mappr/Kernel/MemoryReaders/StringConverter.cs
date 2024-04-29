using System.Text;

namespace Mappr.Kernel.DataConverters
{
    public class StringConverter : IMemoryReader<string>
    {
        public string Convert(MemoryManager manager, nint address)
        {
            byte[] buffer = manager.ReadBytes(address, 128);
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
    }
}
