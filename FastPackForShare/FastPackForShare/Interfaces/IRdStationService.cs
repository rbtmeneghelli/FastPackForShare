using FastPackForShare.Models;

namespace FastPackForShare.Interfaces;

public interface IRdStationService
{
    string GetCode();
    Task<string> GetTokenAsync(string code);
    Task<string> SendLeadAsync(string token, LeadModel leadModel);
}