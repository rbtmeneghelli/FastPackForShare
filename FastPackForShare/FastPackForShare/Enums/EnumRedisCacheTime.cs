namespace FastPackForShare.Enums;

public enum EnumRedisCacheTime : byte
{
    [Display(Name = "Muito Baixo")]
    Min_Low = 2,

    [Display(Name = "Baixo")]
    Low = 5,

    [Display(Name = "Médio")]
    Medium = 10,

    [Display(Name = "Alto")]
    High = 20,

    [Display(Name = "Muito Alto")]
    Max_High = 60,
}
