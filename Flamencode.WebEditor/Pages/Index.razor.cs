using System.Diagnostics;
using BlazorMonaco.Editor;
using Flamencode.WebEditor.Utils;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace Flamencode.WebEditor.Pages;

public partial class Index
{
    private const string FILE_EXTENSION = ".flam";
    private const string EXAMPLE_PATH = "./data/hello_world.flam";
    private const string JS_PATH = "./app.js";
    private const string JS_REGISTER_LANGUAGE_FUNCTION = "registerLanguage";
    private const string JS_SAVE_FUNCTION = "saveFile";
    private const string DEFAULT_SAVE_FILENAME = "code" + FILE_EXTENSION;
    private const string LANGUAGE_NAME = "flamencode";
    private const string EDITOR_THEME = LANGUAGE_NAME + "-theme";
    private const string COMMENT_TOKEN = "#";
    private static readonly string[] SINGLE_TOKENS = { "ole", "anda", "arsa", "asi", "mira", "toma" };
    private static readonly string[] PAIR_TOKENS = { "dale", "arre" };
    private const string EXECUTE_BUTTON_TEXT = "Execute";
    private const string PAUSE_BUTTON_TEXT = "Pause";
    private const string HIDE_CLASS = "hide";

    private Interpreter _interpreter;

    [Inject]
    private IJSRuntime JSRuntime { get; set; }
    [Inject]
    private HttpClient HttpClient { get; set; }

    private JSModuleLoader JSModuleLoader { get; set; }
    private StandaloneCodeEditor CodeEditor { get; set; }
    private string ExecutePauseButtonText { get; set; } = EXECUTE_BUTTON_TEXT;
    private string ExecuteSpinClass { get; set; } = HIDE_CLASS;
    private string ExecuteTimeClass { get; set; } = HIDE_CLASS;
    private string Input { get; set; }
    private string Error { get; set; }
    private string Result { get; set; }
    private string Time { get; set; }

    protected override void OnInitialized()
    {
        JSModuleLoader = new JSModuleLoader(JSRuntime, JS_PATH);
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        string codeExample = await HttpClient.GetStringAsync(EXAMPLE_PATH);
        await CodeEditor.SetValue(codeExample);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
        {
            var jsmodule = await JSModuleLoader.GetModule();
            await jsmodule.InvokeVoidAsync(JS_REGISTER_LANGUAGE_FUNCTION, LANGUAGE_NAME,
                EDITOR_THEME, COMMENT_TOKEN, SINGLE_TOKENS, PAIR_TOKENS);
        }
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
        await jsmodule.InvokeVoidAsync(JS_SAVE_FUNCTION, code, DEFAULT_SAVE_FILENAME);
    }

    private async Task ExecutePauseAsync()
    {
        bool isRunning = _interpreter != null && _interpreter.IsRunning;

        if (isRunning)
            Pause();
        else
            await ExecuteAsync();
    }

    // TODO: Use web worker when supported in .NET7 (https://github.com/Tewr/BlazorWorker/issues/85)
    // or when multithreading is supported (https://github.com/dotnet/runtime/issues/68162)
    private async Task ExecuteAsync()
    {
        Error = Result = Time = string.Empty;
        ExecuteSpinClass = null;
        ExecuteTimeClass = HIDE_CLASS;
        UpdateExecutePauseButton(true);

        using Stream inputStream = new InputStream(Input);
        using Stream outputStream = new MemoryStream();
        using Stream errorStream = new MemoryStream();
        string code = await CodeEditor.GetValue();
        Stopwatch stopwatch = Stopwatch.StartNew();

        _interpreter = new Interpreter(code, inputStream, outputStream, errorStream);
        await Task.Run(() => _interpreter.Run());

        outputStream.Position = 0;
        errorStream.Position = 0;
        using TextReader outputReader = new StreamReader(outputStream);
        using TextReader errorReader = new StreamReader(errorStream);

        Error = errorReader.ReadToEnd();
        Result = outputReader.ReadToEnd();
        TimeSpan elapsed = stopwatch.Elapsed;
        Time = $"{(int)elapsed.TotalMinutes}:{elapsed:ss\\.ffffff}";
        ExecuteSpinClass = HIDE_CLASS;
        ExecuteTimeClass = null;

        UpdateExecutePauseButton(false);
        StateHasChanged();
    }

    private void Pause()
    {
        _interpreter?.Pause();
    }

    private void UpdateExecutePauseButton(bool isRunning)
    {
        if (isRunning)
            ExecutePauseButtonText = PAUSE_BUTTON_TEXT;
        else
            ExecutePauseButtonText = EXECUTE_BUTTON_TEXT;
    }

    private StandaloneEditorConstructionOptions EditorConstructionOptions(StandaloneCodeEditor editor)
    {
        return new StandaloneEditorConstructionOptions
        {
            AutomaticLayout = true,
            Theme = EDITOR_THEME,
            Language = LANGUAGE_NAME
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
