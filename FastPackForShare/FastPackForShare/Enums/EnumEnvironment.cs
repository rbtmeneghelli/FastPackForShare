using System;
using System.Collections.Generic;
namespace FastPackForShare.Enums;

public enum EnumEnvironment : byte
{
    [Display(Name = "Ambiente Produção")]
    PRD = 1,

    [Display(Name = "Ambiente Pre-Produção")]
    PRE_PROD = 2,

    [Display(Name = "Ambiente Homologação")]
    HML = 3,

    [Display(Name = "Ambiente Teste")]
    QA = 4,

    [Display(Name = "Ambiente Desenvolvimento")]
    DEV = 5,
}
