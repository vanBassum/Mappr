namespace Mappr.Kernel
{
    public class MemoryManagerBuilder
    {
        private readonly List<IMemoryReader> readers = new List<IMemoryReader> ();

        public MemoryManagerBuilder WithTypeReader(IMemoryReader reader)
        {
            readers.Add(reader);    
            return this;
        }

        public MemoryManager Build()
        {
            return new MemoryManager(readers);
        }
    }
}
