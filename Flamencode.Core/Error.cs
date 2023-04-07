namespace Flamencode;

internal class Error : Exception
{
    public Error(string message) : base(message)
    {
    }

    public Error(string message, int linePosition, int positionAtLine) 
        : this ($"{message} at line {linePosition} at index {positionAtLine}")
    {
    }

    public override string ToString()
    {
        return Message;
    }
}

internal class CompileError : Error
{
    private const string ERROR_NAME = "Compile error: ";

    public CompileError(string message, int linePosition, int positionAtLine) 
        : base(ERROR_NAME + message, linePosition, positionAtLine)
    {
    }
}

internal class RuntimeError : Error
{
    private const string ERROR_NAME = "Runtime error: ";

    public RuntimeError(string message) : base(ERROR_NAME + message)
    {
    }

    public RuntimeError(string message, int linePosition, int positionAtLine) 
        : base(ERROR_NAME + message, linePosition, positionAtLine)
    {
    }
}