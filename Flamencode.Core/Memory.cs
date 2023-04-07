namespace Flamencode;

public class Memory
{
    private int _startSize;
    private byte[] _positiveMemory; // from 0 to int.MaxValue
    private byte[] _negativeMemory; // from -1 to int.MinValue

    public byte this[int index]
    {
        get => Get(index);
        set => Set(index, value);
    }

    public Memory(int startSize)
    {
        _startSize = startSize;
        _positiveMemory = new byte[startSize];
    }

    private byte Get(int index)
    {
        byte result = 0;
        GetMemoryAndIndex(index, false, out byte[] memory, out index);

        if (index < memory.Length)
            result = memory[index];

        return result;
    }

    private void Set(int index, byte value)
    {
        GetMemoryAndIndex(index, true, out byte[] memory, out index);

        memory[index] = (byte)(value + 0);
    }

    private void GetMemoryAndIndex(int originalIndex, bool ensureCpacity, out byte[] memory, out int index)
    {
        if (originalIndex >= 0)
        {
            index = originalIndex;
            if (ensureCpacity) EnsureCapacity(index, ref _positiveMemory);
            memory = _positiveMemory;
        }
        else
        {
            index = Math.Abs(originalIndex) - 1;
            _negativeMemory ??= new byte[_startSize];
            if (ensureCpacity) EnsureCapacity(index, ref _negativeMemory);
            memory = _negativeMemory;        
        }
    }

    private void EnsureCapacity(int index, ref byte[] memory)
    {
        if (index >= memory.Length)
        {
            int newSize = index * 2;
            byte[] newArray = new byte[newSize];

            Array.Copy(memory, newArray, memory.Length);
            memory = newArray;
        }
    }

    public void Clear() 
    {
        for (int i = 0; i < _positiveMemory.Length; i++)
            _positiveMemory[i] = 0;

        _negativeMemory = null;
    }
}
