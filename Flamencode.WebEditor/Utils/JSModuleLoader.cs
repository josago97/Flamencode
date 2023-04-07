using Microsoft.JSInterop;

namespace Flamencode.WebEditor.Utils;

public class JSModuleLoader : IAsyncDisposable
{
    private readonly Lazy<Task<IJSObjectReference>> _moduleTask;

    public JSModuleLoader(IJSRuntime jsRuntime, string jsPath)
    {
        _moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>("import", jsPath).AsTask());
    }

    public async Task<IJSObjectReference> GetModule()
    {
        return await _moduleTask.Value;
    }

    public async ValueTask DisposeAsync()
    {
        if (_moduleTask.IsValueCreated)
        {
            var module = await _moduleTask.Value;
            await module.DisposeAsync();
        }
    }
}
