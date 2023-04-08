using BlazorMonaco.Editor;
using Flamencode.WebEditor.Utils;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace Flamencode.WebEditor.Pages;

public partial class Index
{
    private const string LANGUAJE_NAME = "flamencode";
    private const string EDITOR_THEME = LANGUAJE_NAME + "-theme";

    [Inject]
    public IJSRuntime JSRuntime { get; set; }
    [Inject]
    public HttpClient HttpClient { get; set; }

    private JSModuleLoader JSModuleLoader { get; set; }
    private StandaloneCodeEditor CodeEditor { get; set; }

    private string Code { get; set; }
    private string Input { get; set; }
    private string Error { get; set; }
    private string Result { get; set; }

    protected override void OnInitialized()
    {
        JSModuleLoader = new JSModuleLoader(JSRuntime, "./app.js");
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        string codeExample = await HttpClient.GetStringAsync("./data/hello_world.flam");
        await CodeEditor.SetValue(codeExample);
    }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        string commentToken = "#";
        string[] singleTokens = { "ole", "anda", "arsa", "asi", "mira", "toma" };
        string[] pairTokens = { "dale", "arre" };

        var jsmodule = await JSModuleLoader.GetModule();
        await jsmodule.InvokeVoidAsync("registerLanguage", LANGUAJE_NAME, EDITOR_THEME, commentToken, singleTokens, pairTokens);
    }

    private async Task LoadFileAsync(InputFileChangeEventArgs e)
    {
        using Stream stream = e.File.OpenReadStream();
        using TextReader reader = new StreamReader(stream);

        string code = await reader.ReadToEndAsync();
        await CodeEditor.SetValue(code);
    }

    private async Task SaveFileAsync()
    {
        string code = await CodeEditor.GetValue();

        var jsmodule = await JSModuleLoader.GetModule();
        await jsmodule.InvokeVoidAsync("saveFile", code, "myCode.flam");
    }

    private async Task ExecuteAsync()
    {
        Code = await CodeEditor.GetValue();

        using Stream inputStream = new InputStream(Input);
        using Stream outputStream = new MemoryStream();
        using Stream errorStream = new MemoryStream();

        Interpreter interpreter = new Interpreter(Code, inputStream, outputStream, errorStream);
        interpreter.Run();

        outputStream.Position = 0;
        errorStream.Position = 0;
        using TextReader outputReader = new StreamReader(outputStream);
        using TextReader errorReader = new StreamReader(errorStream);

        Error = errorReader.ReadToEnd();
        Result = Error + outputReader.ReadToEnd();
    }

    private StandaloneEditorConstructionOptions EditorConstructionOptions(StandaloneCodeEditor editor)
    {
        return new StandaloneEditorConstructionOptions
        {
            AutomaticLayout = true,
            Theme = EDITOR_THEME,
            Language = LANGUAJE_NAME
        };
    }

    private class InputStream : Stream
    {
        private string _content;

        public override bool CanRead => true;
        public override bool CanSeek => throw new NotImplementedException();
        public override bool CanWrite => throw new NotImplementedException();
        public override long Length => _content.Length;
        public override long Position { get; set; }

        public InputStream(string content)
        {
            _content = content;
            Position = 0;
        }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (Position < Length)
            {
                buffer[offset] = (byte)_content[(int)Position];
                Position++;
            }
            else
            {
                buffer[offset] = 0;
            }

            return 1;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }
    }
}
