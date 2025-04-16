namespace FastPackForShare.Models;

public sealed record KissLogModel
{
    public string OrganizationId { get; set; }
    public string ApplicationId { get; set; }
    public string ApiUrl { get; set; }
}
