namespace FastPackForShare.Enums;

public enum EnumPeriod : byte
{
    [Display(Name = "Nunca")]
    Never = 0,

    [Display(Name = "Diário")]
    Daily = 1,

    [Display(Name = "Semanal")]
    Weekly = 2,

    [Display(Name = "Mensal")]
    Monthly = 3,

    [Display(Name = "Anual")]
    Yearly = 4,

    [Display(Name = "Trimestral")]
    Quarterly = 5,
}
