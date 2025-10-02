namespace FastPackForShare.Enums;

public enum EnumStatus : byte
{
    [Display(Name = "Todos")]
    All = 0,

    [Display(Name = "Inativo")]
    Inactive = 1,

    [Display(Name = "Ativo")]
    Active = 2
}
