namespace FastPackForShare.Interfaces;

public interface IMetaService
{
    Task SendMessageToWhatsApp(string celPhone, string message);
}
