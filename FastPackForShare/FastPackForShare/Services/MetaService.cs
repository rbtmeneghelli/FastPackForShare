using FastPackForShare.Interfaces;
using FastPackForShare.Services.Bases;

namespace FastPackForShare.Services;

public sealed class MetaService : BaseHandlerService, IMetaService
{
    private readonly IHttpClientFactory _httpClientFactory;

    private const string URL = "https://graph.facebook.com/v18.0/SEU_PHONE_NUMBER_ID/messages";
    private const string TOKEN_META = "";

    public MetaService(IHttpClientFactory httpClientFactory, INotificationMessageService notificationMessageService) : base(notificationMessageService)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task SendMessageToWhatsApp(string celPhone, string message)
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(90));
        var client = _httpClientFactory.CreateClient("Signed");

        var body = new
        {
            messaging_product = "whatsapp",
            to = celPhone,
            type = "text",
            text = new { body = message }
        };

        var json = JsonSerializer.Serialize(body);

        var request = new HttpRequestMessage(HttpMethod.Post, URL);
        request.Headers.Add("Authorization", $"Bearer {TOKEN_META}");
        request.Content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.SendAsync(request, cts.Token);
        var result = await response.Content.ReadAsStringAsync();
    }
}
