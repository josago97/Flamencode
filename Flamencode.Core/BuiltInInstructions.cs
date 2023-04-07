namespace Flamencode;

internal static class BuiltInInstructions
{
    public static void IncrementMemoryPointer(Interpreter machine)
    {
        machine.MemoryPointer++;
    }

    public static void DecrementMemoryPointer(Interpreter machine)
    {
        machine.MemoryPointer--;
    }

    public static void IncrementMemoryCell(Interpreter machine)
    {
        machine.Memory[machine.MemoryPointer]++;
    }

    public static void DecrementMemoryCell(Interpreter machine)
    {
        machine.Memory[machine.MemoryPointer]--;
    }

    public static void Read(Interpreter machine)
    {
        byte key = (byte)machine.Input.ReadByte();
        machine.Memory[machine.MemoryPointer] = key;
    }

    public static void Write(Interpreter machine)
    {
        byte key = machine.Memory[machine.MemoryPointer];
        machine.Output.WriteByte(key);
    }

    public static void JumpForward(Interpreter machine)
    {
        if (machine.Memory[machine.MemoryPointer] == 0)
        {
            Function function = (Function)machine.CurrentInstruction;
            machine.CodePointer = function.End;
        }
    }

    public static void JumpBackward(Interpreter machine)
    {
        if (machine.Memory[machine.MemoryPointer] != 0)
        {
            Function function = (Function)machine.CurrentInstruction;
            machine.CodePointer = function.Start;
        }
    }
}
