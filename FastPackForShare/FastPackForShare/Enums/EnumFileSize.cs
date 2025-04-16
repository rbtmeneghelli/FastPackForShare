namespace FastPackForShare.Enums;

public enum EnumFileSize : byte
{
    [Display(Name = "Tamanho em kilobytes")]
    KB = 1,

    [Display(Name = "Tamanho em megabytes")]
    MB = 2,

    [Display(Name = "Tamanho em gigabytes")]
    GB = 3,

    [Display(Name = "Tamanho em Terabytes")]
    TB = 4,
}
