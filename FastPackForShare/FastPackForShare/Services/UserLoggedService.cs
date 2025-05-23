﻿using FastPackForShare.Interfaces;
using FastPackForShare.Services.Bases;
using Microsoft.AspNetCore.Http;

namespace FastPackForShare.Services;

public class UserLoggedService : BaseHandlerService, IUserLoggedService
{
    public long UserId { get; set; }
    public string UserName { get; set; }
    public bool UserAuthenticated { get; set; }

    private readonly IHttpContextAccessor _iHttpContextAccessor;

    public UserLoggedService(IHttpContextAccessor iHttpContextAccessor, INotificationMessageService iNotificationMessageService) : base(iNotificationMessageService)
    {
        _iHttpContextAccessor = iHttpContextAccessor;
        UserAuthenticated = UserIsAuthenticated();

        if (UserAuthenticated)
        {
            UserId = GetUserId();
            UserName = GetUserName();
        }
    }

    private long GetUserId()
    {
        string userId = _iHttpContextAccessor.HttpContext.User.FindFirst(x => x.Type.Equals("Id", StringComparison.OrdinalIgnoreCase))?.Value;
        return long.TryParse(userId, out _) ? long.Parse(userId) : 0;
    }

    private string GetUserName()
    {
        return _iHttpContextAccessor.HttpContext.User.Identity.Name;
    }

    private bool UserIsAuthenticated()
    {
        return _iHttpContextAccessor.HttpContext.User.Identity.IsAuthenticated;
    }
}
