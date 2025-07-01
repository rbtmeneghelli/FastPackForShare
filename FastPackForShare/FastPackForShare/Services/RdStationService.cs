using FastPackForShare.Interfaces;
using FastPackForShare.Models;
using Microsoft.Extensions.Options;
using Org.BouncyCastle.Ocsp;
using System.Net.Http;
using System.Net.Http.Headers;

namespace FastPackForShare.Services;

public sealed class RdStationService : IRdStationService
{
    private readonly IHttpClientFactory _ihttpClientFactory;
    private RdStationConfigModel _rdStationConfigModel;

    public RdStationService(IHttpClientFactory ihttpClientFactory, IOptions<RdStationConfigModel> rdStationConfigModel)
    {
        _ihttpClientFactory = ihttpClientFactory;
        _rdStationConfigModel = rdStationConfigModel?.Value ?? throw new ArgumentException("RdStation não pode ser nulo", nameof(rdStationConfigModel));
    }

    public string GetCode()
    {
        var redirectUri = "http://localhost:5000/rdstation/callback";
        var url = $"https://api.rd.services/auth/dialog/client_id={_rdStationConfigModel.ClientId}&redirect_uri={redirectUri}";
        return url; //Ao chamar esse metodo pela controller, aplicar o redirect para chamar o metodo GetTokenAsync com o [FromQuery]
    }

    public async Task<string> GetTokenAsync(string code)
    {
        var values = new Dictionary<string, string>
        {
            { "client_id", _rdStationConfigModel.ClientId },
            { "client_secret", _rdStationConfigModel.ClientSecret },
            { "code", code }
        };

        var content = new FormUrlEncodedContent(values);

        var httpClient = _ihttpClientFactory.CreateClient("Signed");
        var response = await httpClient.PostAsync("https://api.rd.services/auth/token", content);
        var result = await response.Content.ReadAsStringAsync();

        return result;
    }

    public async Task<string> SendLeadAsync(string token, LeadModel leadModel)
    {
        var leadJson = new
        {
            event_type = "CONVERSION",
            event_family = "CDP",
            payload = new
            {
                email = leadModel.Email,
                name = leadModel.Name
            }
        };

        var content = new StringContent(JsonSerializer.Serialize(leadJson), Encoding.UTF8, "application/json");
        var httpClient = _ihttpClientFactory.CreateClient("Signed");
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await httpClient.PostAsync("https://api.rd.services/platform/events", content);
        var json = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Ocorreu um erro durante o envio do lead: {json}");

        return json;
    }

    public async Task<string> ApplyRefreshToken(string currentToken)
    {
        var values = new Dictionary<string, string>
        {
            { "client_id", _rdStationConfigModel.ClientId },
            { "client_secret", _rdStationConfigModel.ClientSecret },
            { "refresh_token", currentToken },
            { "grant_type", "refresh_token" }
        };

        var content = new FormUrlEncodedContent(values);
        var httpClient = _ihttpClientFactory.CreateClient("Signed");
        var response = await httpClient.PostAsync("https://api.rd.services/auth/token", content);
        var json = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Erro ao renovar token: {json}");

        return json;
    }
}