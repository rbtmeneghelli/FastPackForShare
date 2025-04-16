using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastPackForShare.Models;

public sealed class NotificationMessageModel
{
    public string Message { get; }

    public NotificationMessageModel(string message)
    {
        Message = message;
    }
}
