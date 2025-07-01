namespace FastPackForShare.Models;

public record RdStationConfigModel
{
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
}

public record LeadModel
{
    public string Email { get; set; }
    public string Name { get; set; }
}