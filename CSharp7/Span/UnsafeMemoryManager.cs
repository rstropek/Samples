using System;
using System.Buffers;

namespace Span
{
    /// <summary>
    /// A MemoryManager over a raw pointer
    /// </summary>
    public unsafe class UnmanagedMemoryManager<T> : MemoryManager<T> where T : unmanaged
    {
        private readonly T* pointer;
        private readonly int length;

        public UnmanagedMemoryManager(T* pointer, int length)
        {
            this.pointer = pointer;
            this.length = length;
        }

        public UnmanagedMemoryManager(IntPtr pointer, int length)
            : this((T*)pointer.ToPointer(), length) 
        { }

        public override Span<T> GetSpan() => new Span<T>(pointer, length);

        public override MemoryHandle Pin(int elementIndex = 0) =>
            new MemoryHandle(pointer + elementIndex);
        
        public override void Unpin() { }

        protected override void Dispose(bool disposing) { }
    }
}
