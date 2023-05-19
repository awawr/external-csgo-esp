using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static kuchima.Native;

namespace kuchima
{
    public class Memory
    {
        public Process proc { get; set; }
        private IntPtr handle { get; set; }

        public Memory(Process p)
        {
            proc = p;
            handle = proc.Handle;
        }

        public void Dispose() => CloseHandle(handle);

        public IntPtr GetModuleBaseAddress(string modulename)
        {
            foreach (ProcessModule module in proc.Modules)
            {
                if (module.ModuleName == modulename)
                {
                    return module.BaseAddress;
                }
            }
            return IntPtr.Zero;
        }

        public T Read<T>(IntPtr address)
        {
            int size = Marshal.SizeOf(typeof(T));
            byte[] buffer = new byte[size];
            ReadProcessMemory(handle, address, buffer, buffer.Length, out _);

            GCHandle ptr = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            T data = (T)Marshal.PtrToStructure(ptr.AddrOfPinnedObject(), typeof(T));
            ptr.Free();

            return data;
        }

        public bool Write<T>(IntPtr address, T value)
        {
            int size = Marshal.SizeOf(value);
            byte[] buffer = new byte[size];

            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(value, ptr, true);
            Marshal.Copy(ptr, buffer, 0, size);
            Marshal.FreeHGlobal(ptr);

            return WriteProcessMemory(handle, address, buffer, buffer.Length, out _);
        }
    }
}
