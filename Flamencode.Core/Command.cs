namespace Flamencode;

internal enum Command
{
    None = 0,

    IncrementMemoryPointer,
    DecrementMemoryPointer,
    IncrementMemoryCell,
    DecrementMemoryCell,
    Read,
    Write,
    JumpForward,
    JumpBackward
}
