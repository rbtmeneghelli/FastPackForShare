using Microsoft.AspNetCore.Mvc;

namespace FastPackForShare.Models;

public class ExceptionErrorModel : ProblemDetails
{
    public string ExceptionError { get; set; }
    public int StatusCode { get; set; }
    public bool Success { get; set; }
}
