namespace Flamencode;

internal class Settings
{
    public static readonly Dictionary<string, Command> COMMANDS = new Dictionary<string, Command>()
    {
        { "asi", Command.IncrementMemoryPointer },
        { "anda", Command.DecrementMemoryPointer },

        { "ole", Command.IncrementMemoryCell },
        { "arsa", Command.DecrementMemoryCell },

        { "mira", Command.Read },
        { "toma", Command.Write },

        { "dale", Command.JumpForward },
        { "arre", Command.JumpBackward },

        { ">", Command.IncrementMemoryPointer },
        { "<", Command.DecrementMemoryPointer },

        { "+", Command.IncrementMemoryCell },
        { "-", Command.DecrementMemoryCell },

        { ",", Command.Read },
        { ".", Command.Write },

        { "[", Command.JumpForward },
        { "]", Command.JumpBackward }
    };
}
