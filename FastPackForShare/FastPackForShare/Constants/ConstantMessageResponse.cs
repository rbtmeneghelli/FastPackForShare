using System.Collections.Frozen;

namespace FastPackForShare.Constants;

public static class ConstantMessageResponse
{
    public const string NO_CONTENT_CODE = "";
    public const string NOT_FOUND_CODE = "O Registro solicitado não foi encontrado";
    public const string BAD_REQUEST_CODE = "Ocorreu um erro durante o processamento dos dados pela API";
    public const string UNAUTHORIZED_CODE = "Você não tem permissão para efetuar o processamento dessa ação";
    public const string INTERNAL_ERROR_CODE = "Ocorreu um erro interno durante o processamento dos dados pela API. Entre em contato com o Administrador";
    public const string CREATE_CODE = "O Registro foi criado com sucesso";
    public const string FORBIDDEN_CODE = "Acesso negado! O token fornecido não possui privilegio para efetuar tal ação";
    public const string AUTHENTICATION_REQUIRED_CODE = "O token expirou. Por favor, faça uma nova autenticação para que seja gerado um novo token";
    public const string RESEARCH_CODE = "O registro foi consultado com sucesso";
    public const string UPDATE_CODE = "O registro foi atualizado com sucesso";
    public const string DELETE_CODE = "O registro foi excluido com sucesso";
    public const string STATUS_ACTIVE_CODE = "O registro foi ativado com sucesso";
    public const string STATUS_INACTIVE_CODE = "O registro foi desativado com sucesso";
    public const string SERVICE_RUNNING = "O serviço {0} está em execução";
    public const string SERVICE_NOT_RUNNING = "O serviço {0} não está em execução";
    public const string REQUEST_API = "Erro ao efetuar request da Api externa: {0}";

    public static string GetMessageResponse(int statusCode)
    {
        var dictionary = new Dictionary<int, string>
        {
            { ConstantHttpStatusCode.NOT_FOUND_CODE, NOT_FOUND_CODE },
            { ConstantHttpStatusCode.BAD_REQUEST_CODE, BAD_REQUEST_CODE },
            { ConstantHttpStatusCode.UNAUTHORIZED_CODE, UNAUTHORIZED_CODE },
            { ConstantHttpStatusCode.INTERNAL_ERROR_CODE, INTERNAL_ERROR_CODE },
            { ConstantHttpStatusCode.OK_CODE, INTERNAL_ERROR_CODE },
            { ConstantHttpStatusCode.AUTHENTICATION_REQUIRED_CODE, AUTHENTICATION_REQUIRED_CODE },
        }.ToFrozenDictionary();

        statusCode = statusCode.Equals(ConstantHttpStatusCode.OK_CODE)
                     ? ConstantHttpStatusCode.INTERNAL_ERROR_CODE
                     : statusCode;

        return dictionary[statusCode];
    }
}
