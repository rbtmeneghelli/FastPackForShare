﻿using FastPackForShare.Enums;
using FastPackForShare.Extensions;
using FastPackForShare.Interfaces;
using FastPackForShare.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace FastPackForShare.Services;

public sealed class RdStationService : IRdStationService
{
    private readonly IHttpClientFactory _ihttpClientFactory;
    private RdStationConfigModel _rdStationConfigModel;
    private readonly IMemoryCache _memoryCache;

    public RdStationService(IHttpClientFactory ihttpClientFactory, IOptions<RdStationConfigModel> rdStationConfigModel, IMemoryCache memoryCache)
    {
        _ihttpClientFactory = ihttpClientFactory;
        _rdStationConfigModel = rdStationConfigModel?.Value ?? throw new ArgumentException("RdStation não pode ser nulo", nameof(rdStationConfigModel));
        _memoryCache = memoryCache;
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

    #region Generic Methods

    public async Task<RdStationResult> ApplyRequestHttpGet<TContent>(RdStationReqDto rdStationReqDto)
    {
        var httpClient = _ihttpClientFactory.CreateClient("Signed");
        await GenerateTokenAutentication(rdStationReqDto.EnumRdStationAutentication);
        var response = await httpClient.GetAsync(rdStationReqDto.URL);
        var resultado = await GetResponse<TContent>(response);
        return resultado;
    }

    public async Task<RdStationResult> ApplyRequestHttpPost<TContent>(RdStationReqDto rdStationReqDto)
    {
        var httpClient = _ihttpClientFactory.CreateClient("Signed");
        await GenerateTokenAutentication(rdStationReqDto.EnumRdStationAutentication);
        var response = await httpClient.PostAsJsonAsync(rdStationReqDto.URL, rdStationReqDto.PayLoad);
        var resultado = await GetResponse<TContent>(response);
        return resultado;
    }

    public async Task<RdStationResult> ApplyRequestHttpPut<TContent>(RdStationReqDto rdStationReqDto)
    {
        var httpClient = _ihttpClientFactory.CreateClient("Signed");
        await GenerateTokenAutentication(rdStationReqDto.EnumRdStationAutentication);
        var response = await httpClient.PutAsJsonAsync(rdStationReqDto.URL, rdStationReqDto.PayLoad);
        var resultado = await GetResponse<TContent>(response);
        return resultado;
    }

    public async Task<RdStationResult> ApplyRequestHttpDelete<TContent>(RdStationReqDto rdStationReqDto)
    {
        var httpClient = _ihttpClientFactory.CreateClient("Signed");
        await GenerateTokenAutentication(rdStationReqDto.EnumRdStationAutentication);
        var response = await httpClient.DeleteAsync(rdStationReqDto.URL);
        var resultado = await GetResponse<TContent>(response);
        return resultado;
    }

    private async Task GenerateTokenAutentication(EnumRdStationAutentication enumRdStationAutenticacao)
    {
        var httpClient = _ihttpClientFactory.CreateClient("Signed");
        DateTime dataHoraAtual = DateOnlyExtension.GetDateTimeNowFromBrazil();
        RdStationRespTokenDto rdStationAutenticacao = new();

        if (enumRdStationAutenticacao.Equals(EnumRdStationAutentication.AUTENTICACAO_BEARERTOKEN))
        {
            if (!_memoryCache.TryGetValue("MeuObjetoCache", out RdStationRespTokenDto cachedObject))
            {
                var payLoad = new
                {
                    client_id = _rdStationConfigModel.ClientId,
                    client_secret = _rdStationConfigModel.ClientSecret,
                    code = _rdStationConfigModel.Code
                };

                var response = await httpClient.PostAsJsonAsync($"https://api.rd.services/auth/token?token_by={_rdStationConfigModel.Code}", payLoad);

                if (response.StatusCode.Equals(HttpStatusCode.OK))
                {
                    rdStationAutenticacao = await response.Content.ReadFromJsonAsync<RdStationRespTokenDto>();

                    var cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(cachedObject.SecondsToExpire)
                    };

                    rdStationAutenticacao.ExpirationTokenDate = dataHoraAtual.AddSeconds(cachedObject.SecondsToExpire);
                    _memoryCache.Set("MeuObjetoCache", rdStationAutenticacao, cacheEntryOptions);
                    httpClient.DefaultRequestHeaders.Add("authorization", $"Bearer {rdStationAutenticacao.Token}");
                }
            }

            else if (cachedObject.ExpirationTokenDate.HasValue && cachedObject.ExpirationTokenDate > dataHoraAtual)
            {
                var payLoadAtualizado = new
                {
                    client_id = _rdStationConfigModel.ClientId,
                    client_secret = _rdStationConfigModel.ClientSecret,
                    refresh_token = cachedObject.RefreshToken
                };

                var responseAtualizado = await httpClient.PostAsJsonAsync($"https://api.rd.services/auth/token", payLoadAtualizado);
                if (responseAtualizado.StatusCode.Equals(HttpStatusCode.OK))
                {
                    rdStationAutenticacao = await responseAtualizado.Content.ReadFromJsonAsync<RdStationRespTokenDto>();

                    var cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(cachedObject.SecondsToExpire)
                    };

                    rdStationAutenticacao.ExpirationTokenDate = dataHoraAtual.AddSeconds(cachedObject.SecondsToExpire);
                    _memoryCache.Set("MeuObjetoCache", cachedObject, cacheEntryOptions);
                    httpClient.DefaultRequestHeaders.Add("authorization", $"Bearer {cachedObject.Token}");
                }
            }
        }
    }

    private async Task<RdStationResult> GetResponse<TContent>(HttpResponseMessage response)
    {
        if (response.StatusCode.Equals(HttpStatusCode.OK))
        {
            var resultadoOk = await response.Content.ReadFromJsonAsync<TContent>();
            return new RdStationResult { Code = (int)response.StatusCode, Data = resultadoOk, Message = "OK" };
        }

        var resultadoErro = await response.Content.ReadFromJsonAsync<dynamic>();
        return new RdStationResult { Code = (int)response.StatusCode, Data = resultadoErro, Message = "Erro" };
    }

    #endregion
}