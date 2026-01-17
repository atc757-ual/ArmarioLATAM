using ArmarioLATAM.Services;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.JSInterop;

public class SessionStorageService
{
    private readonly ProtectedSessionStorage _pss;

    public SessionStorageService(ProtectedSessionStorage pss)
    {
        _pss = pss;
    }

    public async Task SetItemAsync(string key, string value)
    {
        if (value is null)
            return;

        await _pss.SetAsync(key, value);
    }

    public async Task<string?> GetItemAsync(string key)
    {
        var result = await _pss.GetAsync<string>(key);
        return result.Success ? result.Value : null;
    }

    public async Task RemoveItemAsync(string key)
        => await _pss.DeleteAsync(key);
}
