﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastPackForShare.Models;

public sealed record TwilioModel
{
    public string AccountSid { get; set; }
    public string AuthToken { get; set; }
    public string TwilioNumber { get; set; }
}
