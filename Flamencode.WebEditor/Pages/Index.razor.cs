using Microsoft.JSInterop.Implementation;
using Microsoft.JSInterop;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using BlazorMonaco.Editor;
using Flamencode.WebEditor.Utils;
using System.Reflection;

namespace Flamencode.WebEditor.Pages
{
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

            string[] tokens = new string[] { "ole", "anda", "arsa", "asi", "arre", "dale", "mira", "toma" };
            string commentToken = "#";

            var jsmodule = await JSModuleLoader.GetModule();
            await jsmodule.InvokeVoidAsync("registerLanguage", LANGUAJE_NAME, EDITOR_THEME, tokens, commentToken);
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

            using Stream outputStream = new MemoryStream();
            using Stream errorStream = new MemoryStream();
            Interpreter interpreter = new Interpreter(Code, null, outputStream, errorStream);
            interpreter.Run();
            outputStream.Position = 0;
            errorStream.Position = 0;
            using TextReader outputReader = new StreamReader(outputStream);
            using TextReader errorReader = new StreamReader(errorStream);
            Error = errorReader.ReadToEnd();
            Result = outputReader.ReadToEnd();
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
    }
}