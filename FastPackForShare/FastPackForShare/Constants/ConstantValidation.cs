namespace FastPackForShare.Constants;

public static class ConstantValidation
{
    public const string REQUIRED = "O campo {0} é obrigatório";
    public const string MINLENGTH = "O campo {0} deve ter no máximo de {1} caracteres";
    public const string MAXLENGTH = "O campo {0} deve ter no mínimo de {1} caracteres";
    public const string ENUM = "O valor fornecido não é uma opção válida";
    public const string RANGE = "O campo {0} deve estar na faixa {1} ou {2}";
    public const string DATE = "A data de nascimento não pode ser no futuro";
    public const string LISTNULL = "A lista não pode ser nula";
    public const string LISTEMPTY = "A lista deve conter pelo menos um registro";
    public const string BASE64 = "O Base64 informado não é valido";
    public const string EMAIL = "O email fornecido não é válido.";
}
