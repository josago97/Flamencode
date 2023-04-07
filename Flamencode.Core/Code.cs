using System.Text.RegularExpressions;

namespace Flamencode;

internal class Code
{
    private const string COMMENTS_REGEX = @"\#.*";
    private const string REMOVE_CHARACTERS_REGEX = "\r|\t";
    private const string DUPLICATE_WHITESPACES_REGEX = @"\s\s+";
    private const string TOKEN_SEPARATOR = " ";

    private Regex _removeRegex = new Regex(string.Join('|', COMMENTS_REGEX, REMOVE_CHARACTERS_REGEX));
    private Regex _duplicateWhitespacesRegex = new Regex(DUPLICATE_WHITESPACES_REGEX);
    private bool _isCompiled = false;
    private List<Instruction> _instructions;

    public string RawCode { get; private set; }
    public int Length => _instructions.Count;
    public Instruction this[int index] => _instructions[index];

    public Code(string code)
    {
        RawCode = code;
    }

    public string GetCleanCode()
    {
        string result = RawCode;

        result = _removeRegex.Replace(result, string.Empty);
        result = result.Replace('\n', ' ');
        result = _duplicateWhitespacesRegex.Replace(result, " ");

        return result;
    }

    public void Compile()
    {
        if (!_isCompiled)
        {
            _instructions = new List<Instruction>();
            Stack<Function> functionStack = new Stack<Function>();

            GetInstructions(RawCode, functionStack);
            CheckInstructions(functionStack);
            _isCompiled = true;
        }
    }

    public void GetInstructions(string code, Stack<Function> functions)
    {
        string[] rawLines = code.Split('\n');

        for (int i = 0; i < rawLines.Length; i++)
        {
            string line = _removeRegex.Replace(rawLines[i], string.Empty);
            line = _duplicateWhitespacesRegex.Replace(line, " ");
            line = line.Trim();

            if (!string.IsNullOrEmpty(line))
            {
                GetInstructions(line.ToLower(), i, functions);
            }
        }
    }

    private void GetInstructions(string line, int linePosition, Stack<Function> functions)
    {
        string[] commands = line.Split(TOKEN_SEPARATOR);
        int position = 0;

        foreach (string command in commands)
        {
            try
            {
                Instruction instruction = GetInstruction(command, functions);
                instruction.LinePosition = linePosition;
                instruction.PositionAtLine = position;

                _instructions.Add(instruction);
            }
            catch (Exception e)
            {
                throw new CompileError(e.Message, linePosition, position);
            }

            position += command.Length + TOKEN_SEPARATOR.Length;
        }
    }

    private Instruction GetInstruction(string opCode, Stack<Function> functions)
    {
        Instruction result;
        Function function;
        Command command = Settings.COMMANDS.GetValueOrDefault(opCode);

        switch (command)
        {
            case Command.None:
                throw new Error($"Unknown command '{opCode}'");

            case Command.JumpForward:
                function = new Function() { Start = _instructions.Count };
                functions.Push(function);
                result = function;
                break;

            case Command.JumpBackward:
                if (functions.TryPop(out function))
                {
                    function.End = _instructions.Count;
                    result = new Function(function);
                }
                else
                    throw new Error("No jump start found");
                break;

            default:
                result = new Instruction();
                break;
        }

        result.Name = opCode;
        result.Command = command;

        return result;
    }

    private void CheckInstructions(Stack<Function> fuctions)
    {
        if (fuctions.TryPop(out Function function))
            throw new CompileError("No jump end found", function.LinePosition, function.PositionAtLine);
    }
}

internal class Instruction
{
    public string Name { get; set; }
    public Command Command { get; set; }
    public int LinePosition { get; set; }
    public int PositionAtLine { get; set; }

}

internal class Function : Instruction
{
    public int Start { get; set; } = -1;
    public int End { get; set; } = -1;

    public Function() { }

    public Function(Function function)
    {
        Start = function.Start;
        End = function.End;
    }
}