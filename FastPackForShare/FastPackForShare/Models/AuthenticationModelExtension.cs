using FastPackForShare.Extensions;
using FastPackForShare.Models;
using System.Security.Claims;

namespace FastPackForShare.Models;

/// <summary>
/// Recurso do NET 10, utilizando diretamente os metodos de extensão associados a classe ou tipo nativo associados
/// </summary>

public static class AuthenticationModelExtension
{
    extension(AuthenticationModel authenticationModel)
    {
        /* Propriedade de instância */
        public bool IsValidLogin => GuardClauseExtension.IsNotNullOrWhiteSpace(authenticationModel.Login);
        public bool IsValidInitials => GuardClauseExtension.IsNotNullOrWhiteSpace(authenticationModel.Initials);
        public bool IsValidProfile => GuardClauseExtension.IsNotNullOrWhiteSpace(authenticationModel.Profile);

        /* Metodo de instância */
        public DateTime GetCurrentDateTime() => DateOnlyExtension.GetDateTimeNowFromBrazil();
    }

    extension(AuthenticationModel)
    {
        /* Membro estático associado ao tipo */
        public static AuthenticationModel Default
        => new AuthenticationModel
        {
            Id = 0,
            Login = string.Empty,
            AccessDate = DateOnlyExtension.GetDateTimeNowFromBrazil(),
            CodeTwoFactoryCode = string.Empty,
            ExpirationDate = string.Empty,
            Initials = string.Empty,
            Profile = string.Empty,
            Roles = Enumerable.Empty<Claim>().ToList(),
            Token = string.Empty,
        };
    }
}