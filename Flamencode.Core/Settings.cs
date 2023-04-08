namespace Flamencode;

internal class Settings
{
    public static readonly Dictionary<string, Command> COMMANDS = new Dictionary<string, Command>()
    {
        { "anda", Command.IncrementMemoryPointer },
        { "asi", Command.DecrementMemoryPointer },

        { "ole", Command.IncrementMemoryCell },
        { "arsa", Command.DecrementMemoryCell },

        { "mira", Command.Read },
        { "toma", Command.Write },

        { "dale", Command.JumpForward },
        { "arre", Command.JumpBackward }
    };
}
