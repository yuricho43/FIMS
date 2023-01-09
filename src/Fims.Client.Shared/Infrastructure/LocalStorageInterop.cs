using Microsoft.JSInterop;
using System.Text.Json;

namespace Fims.Client.Shared.Infrastructure;

public sealed class LocalStorageInterop
{
    private readonly IJSRuntime _jsRuntime;

    public LocalStorageInterop(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public ValueTask Clear() =>
        _jsRuntime.InvokeVoidAsync("localStorage.clear");

    /////////////////////////////////////////////////////////////////////////////////////
    //JBH SEE: https://github.com/dotnet/maui/issues/2547
    //JBH FIXME
    //  _jsRuntime.InvokeAsync
    //      - OK in the genuine Web Browser
    //      - Exception in WebView
    //
    //  Exception from blazor.webview.js
    //
    //  Cannot invoke JavaScript outside of a WebView context.
    //     at Microsoft.AspNetCore.Components.WebView.Services.WebViewJSRuntime.BeginInvokeJS(Int64 taskId, String identifier, String argsJson, JSCallResultType resultType, Int64 targetInstanceId)
    //     at Microsoft.JSInterop.JSRuntime.InvokeAsync[TValue](Int64 targetInstanceId, String identifier, CancellationToken cancellationToken, Object[] args)
    //     at Microsoft.JSInterop.JSRuntime.InvokeAsync[TValue](Int64 targetInstanceId, String identifier, Object[] args)
    //     at Fims.Client.Shared.Infrastructure.LocalStorageInterop.GetItem[T](String key)
    //     at Fims.Client.Shared.Infrastructure.AuthenticationHeaderHandler.SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    //
    //  How to fix?
    //     Use JSInterop after the app was rendered, when JSRuntime is available.
    //     See @enetstudio comments: https://github.com/dotnet/maui/issues/2547
    /////////////////////////////////////////////////////////////////////////////////////
    public async ValueTask<T?> GetItem<T>(string key)
    {
        var data = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", key);
        return data != null
            ? JsonSerializer.Deserialize<T>(data)
            : default;
    }

    public ValueTask<string> Key(int index) =>
        _jsRuntime.InvokeAsync<string>("localStorage.key", index);

    public ValueTask<bool> ContainKey(string key) =>
        _jsRuntime.InvokeAsync<bool>("localStorage.hasOwnProperty", key);

    public ValueTask<int> Length() =>
        _jsRuntime.InvokeAsync<int>("eval", "localStorage.length");

    public ValueTask RemoveItem(string key) =>
        _jsRuntime.InvokeVoidAsync("localStorage.removeItem", key);

    /////////////////////////////////////////////////////////////////////////////////////
    //JBH: Weired!!
    //     SetItem works !! , but GetItem does not!!
    //
    //     Why works?
    //     JSInterop does not work before "rendered"
    //     This SetItem is being called after rendered, while GetItem by CategoriesClientService is called before rendered so does not work!
    //
    //     How to fix?
    //     Use JSInterop after the app was rendered, when JSRuntime is available.
    //     See @enetstudio comments: https://github.com/dotnet/maui/issues/2547
    /////////////////////////////////////////////////////////////////////////////////////
    public async ValueTask SetItem<T>(string key, T? data)
    {
        var obj = JsonSerializer.Serialize(data);
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", key, obj);
    }
}
