using KiBoards.Management.Models.Objects;
using KiBoards.Management.Models.Settings;
using KiBoards.Management.Models.Spaces;
using System.Net.Http.Json;
using System.Text.Json;

namespace KiBoards.Management;

public class KibanaHttpClient 
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonPropertyNameCaseInsensitive = new() { PropertyNameCaseInsensitive = true };
    private readonly JsonSerializerOptions _jsonCamelCasePropertyNamingPolicy = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public KibanaHttpClient(HttpClient httpClinet)
    {
        _httpClient = httpClinet;    
        _httpClient.DefaultRequestHeaders.Add("kbn-xsrf", "true");
    }

    private string GetSpaceBaseUrl(string spaceId) => !string.IsNullOrEmpty(spaceId) ? $"/s/{spaceId.ToLower()}" : string.Empty;



    /// <summary>
    /// Set Kibana theme for default space.
    /// </summary>
    /// <param name="darkMode"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task SetDarkModeAsync(bool darkMode, CancellationToken cancellationToken = default) => await SetDarkModeAsync(darkMode, null, cancellationToken);



    private JsonContent GetSettingsChangeJsonContent<T>(T changes) where T : class => JsonContent.Create(new KibanaSettingsRequest<T>()
    {
        Changes = changes
    });

    /// <summary>
    /// Set Kibana theme for given space id
    /// </summary>
    /// <param name="darkMode"></param>
    /// <param name="spaceId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task SetDarkModeAsync(bool darkMode, string spaceId, CancellationToken cancellationToken = default)
    {
        var content = GetSettingsChangeJsonContent(new KibanaSettingsDarkMode() { ThemeDarkMode = darkMode });
        var response = await _httpClient.PostAsync($"{GetSpaceBaseUrl(spaceId)}/api/kibana/settings", content, cancellationToken);
        response.EnsureSuccessStatusCode();
    }


    public async Task<bool> TrySetDefaultRoute(string defaultRoute, string spaceId, CancellationToken cancellationToken = default)
    {
        var content = GetSettingsChangeJsonContent(new KibanaSettingsDefaultRoute() { DefaultRoute = defaultRoute });
        var response = await _httpClient.PostAsync($"{GetSpaceBaseUrl(spaceId)}/api/kibana/settings", content, cancellationToken);
        return response.IsSuccessStatusCode;
    }


    public async Task<KibanaImportObjectsResponse> ImportSavedObjectsAsync(string ndjsonFile, CancellationToken cancellationToken = default) => await ImportSavedObjectsAsync(ndjsonFile, false, null, cancellationToken);
    public async Task<KibanaImportObjectsResponse> ImportSavedObjectsAsync(string ndjsonFile, string spaceId, CancellationToken cancellationToken = default) => await ImportSavedObjectsAsync(ndjsonFile, false, spaceId, cancellationToken);
    public async Task<KibanaImportObjectsResponse> ImportSavedObjectsAsync(string ndjsonFile, bool overwrite, string spaceId = null, CancellationToken cancellationToken = default)
    {
        var multipartContent = new MultipartFormDataContent();
        var streamContent = new StreamContent(File.Open(ndjsonFile, FileMode.Open));
        multipartContent.Add(streamContent, "file", ndjsonFile);

        var response = await _httpClient.PostAsync($"{GetSpaceBaseUrl(spaceId)}/api/saved_objects/_import?overwrite={overwrite.ToString().ToLower()}", multipartContent);

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<KibanaImportObjectsResponse>(_jsonPropertyNameCaseInsensitive, cancellationToken);

        return result;
    }

    public async Task<KibanaStatusResponse> GetStatus(CancellationToken cancellationToken = default) => await _httpClient.GetFromJsonAsync<KibanaStatusResponse>("api/status", cancellationToken);

    public async Task<bool> TryCreateSpaceAsync(KibanaSpace space, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("api/spaces/space", space, _jsonCamelCasePropertyNamingPolicy, cancellationToken );
        return response.IsSuccessStatusCode;
    }

    public async Task<KibanaSpace> GetSpaceAsync(string spaceId, CancellationToken cancellationToken = default) 
        => await _httpClient.GetFromJsonAsync<KibanaSpace>($"api/spaces/space/{spaceId}", _jsonPropertyNameCaseInsensitive, cancellationToken);
}
