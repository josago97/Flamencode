namespace Flamencode;

public class Interpreter
{
    private const int START_MEMORY_SIZE = 256;
    private static readonly Dictionary<Command, Action<Interpreter>> INSTRUCTIONS = new()
    {
        { Command.IncrementMemoryPointer, BuiltInInstructions.IncrementMemoryPointer },
        { Command.DecrementMemoryPointer, BuiltInInstructions.DecrementMemoryPointer },

        { Command.IncrementMemoryCell, BuiltInInstructions.IncrementMemoryCell },
        { Command.DecrementMemoryCell, BuiltInInstructions.DecrementMemoryCell },

        { Command.Read, BuiltInInstructions.Read },
        { Command.Write, BuiltInInstructions.Write },

        { Command.JumpForward, BuiltInInstructions.JumpForward },
        { Command.JumpBackward, BuiltInInstructions.JumpBackward }
    };

    private Code _code;

    public Memory Memory { get; init; }
    public int MemoryPointer { get; set; }
    public int CodePointer { get; set; }
    internal Instruction CurrentInstruction { get; private set; }
    public bool IsRunning { get; private set; }
    public string Error { get; private set; }
    public Stream Input { get; set; }
    public Stream Output { get; set; }
    public TextWriter ErrorOutput { get; set; }

    public Interpreter(string code, Stream input = null, Stream output = null, Stream errorOutput = null)
    { 
        Input = input;
        if (output != null) Output = output;
        if (errorOutput != null) ErrorOutput = GetWriter(errorOutput);
        Memory = new Memory(START_MEMORY_SIZE);

        _code = new Code(code);
        Initialize();
    }

    #region Initialization

    private void Initialize()
    {
        MemoryPointer = 0;
        Memory.Clear();
        
        CodePointer = 0;
        Error = null;
    }

    private TextWriter GetWriter(Stream stream)
    {
        return new StreamWriter(stream)
        {
            AutoFlush = true
        };
    }

    #endregion

    #region Execution

    public bool Compile()
    {
        bool result = false;

        try
        {
            _code.Compile();
            result = true;
        }
        catch (Exception ex)
        {
            ShowError(ex.Message);
        }
        
        return result;
    }

    public void Run()
    {
        IsRunning = true;

        try
        {
            if (Compile())
            {
                while (IsRunning && !EndOfProgram())
                {
                    NextStep();
                }
            }
        }
        catch (Exception ex) 
        {
            ShowError(ex.Message);
        }

        IsRunning = false;
    }

    public void NextStep()
    {
        string error = null;
        CurrentInstruction = _code[CodePointer];

        try
        {
            Command command = CurrentInstruction.Command;
            INSTRUCTIONS[command].Invoke(this);
        }
        catch (Error e)
        {
            error = e.ToString();
        }
        catch
        {
            error = "System error";
        }

        if (!string.IsNullOrEmpty(error))
        {
            int linePosition = CurrentInstruction.LinePosition;
            int positionAtLine = CurrentInstruction.PositionAtLine;

            throw new RuntimeError(error, linePosition, positionAtLine);
        }
        else
            CodePointer++; 
    }

    private bool EndOfProgram()
    {
        return CodePointer >= _code.Length || Error != null;
    }

    public void Pause()
    {
        IsRunning = false;
    }

    private void ShowError(string message)
    {
        Error = message;
        ErrorOutput?.WriteLine(message);
    }

    #endregion
}
