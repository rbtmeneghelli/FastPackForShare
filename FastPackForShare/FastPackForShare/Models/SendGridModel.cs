namespace FastPackForShare.Models;

public record SendGridModel
{
    public required string EmailFor { get; set; }
    public required string Subject { get; set; }
    public string Message { get; set; }
    public string TemplateId { get; set; }
    public Dictionary<string, object> Data { get; set; }
}
