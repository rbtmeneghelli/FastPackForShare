using FastPackForShare.Interfaces;
using FastPackForShare.Services.Bases;
using ILogger = Serilog.ILogger;

namespace FastPackForShare.Services;

public class SeriLogService : BaseHandlerService, ISeriLogService
{
    private readonly ILogger _iSeriLogService;

    public SeriLogService(ILogger iSeriLogService, INotificationMessageService iNotificationMessageService) : base(iNotificationMessageService)
    {
        _iSeriLogService = iSeriLogService;
    }

    public void WriteErrorLogger(string message) => _iSeriLogService.Fatal(message);
    public void WriteFatalLogger(string message) => _iSeriLogService.Error(message);
    public void WriteInformationLogger(string message) => _iSeriLogService.Information(message);
}
