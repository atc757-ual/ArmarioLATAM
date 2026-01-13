using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

public class ProtectedLocalStorageService
{
    private readonly Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage.ProtectedLocalStorage _pls;

    public ProtectedLocalStorageService(Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage.ProtectedLocalStorage pls)
    {
        _pls = pls;
    }

    public async Task SetItemAsync<T>(string key, T value)
    {
        if (value is null)
            return;
        await _pls.SetAsync(key, value);
    }

    public async Task<T?> GetItemAsync<T>(string key)
    {
        var result = await _pls.GetAsync<T>(key);
        return result.Success ? result.Value : default;
    }

    public async Task RemoveItemAsync(string key)
        => await _pls.DeleteAsync(key);
}
