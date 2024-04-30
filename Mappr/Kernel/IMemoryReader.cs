namespace Mappr.Kernel
{
    public interface IMemoryReader<T> : IMemoryReader
    {
        Type IMemoryReader.DataType => typeof(T);
        T? Convert(MemoryManager manager, nint address);
    }

    public interface IMemoryReader
    {
        Type DataType { get; }
    }
}
