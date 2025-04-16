namespace FastPackForShare.Enums;

public enum EnumAction : byte
{
    [Display(Name = "Cadastro")]
    Insert = 1,

    [Display(Name = "Edição")]
    Update = 2,

    [Display(Name = "Exclusão")]
    Delete = 3,

    [Display(Name = "Procurar")]
    Research = 4,

    [Display(Name = "Exportar")]
    Export = 5,

    [Display(Name = "Importar")]
    Import = 6,
}
