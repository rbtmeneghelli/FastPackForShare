using System.ComponentModel;

namespace FastPackForShare.Enums;

public enum EnumRdStationAutentication : byte
{
    [Description("Sem Autenticação")]
    SEM_AUTENTICACAO = 0,

    [Description("Autenticação Bearer Token")]
    AUTENTICACAO_BEARERTOKEN = 1,
}
