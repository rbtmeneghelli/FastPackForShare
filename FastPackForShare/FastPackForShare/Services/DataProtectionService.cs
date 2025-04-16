using Microsoft.AspNetCore.DataProtection;

namespace FastPackForShare.Services
{
    /// Esse serviço tem metodos para poder proteger os dados sensiveis que são recebidos ou enviados por um API
    public sealed class DataProtectionService : IDataProtectionService
    {
        private readonly IDataProtector _dataProtector;

        public DataProtectionService(IDataProtectionProvider dataProtectionProvider)
        {
            //Esse codigo e para criar um protetor especifico para um tipo de informação que a gente queira
            _dataProtector = dataProtectionProvider.CreateProtector("Dados.Sensiveis.CPF.Cartao");
        }

        public string ApplyProtect(string input) => _dataProtector.Protect(input);

        public string RemoveProtect(string input) => _dataProtector.Unprotect(input);
    }
}
