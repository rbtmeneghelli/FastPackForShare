-- Passos para utilizar essa implementa��o

>> Na classe de Handler para comandos de command, herdar o contrato de command passando a classe de comando.
Ex: public class CreateUser : IDispatcherMediatorCommandHandler<UserCreateCommand>

>> Na classe de Handler de query, herdar o contrato de query passando a classe de query.
Ex: public class CreateUser : IDispatcherMediatorQueryHandler<UserGetAllQuery, UserDTO>

>> E necessario utilizar o pacote Scrutor para facilitar a inje��o de dependencia dessa implementa��o.

>> Fazer o registro do servi�o na classe startup ou program do projeto.
Ex: ContainerFPFDispatcherMediator.AddDispatcherMediatorHandlers(assemblyName); builder.AddScoped<IDispatcherMediatorService, DispatcherMediatorService>();

>> Por fim utilizar o IDispatcherMediatorService igual � feito com o IMediator que deve funcionar!

>> Referencia: http://macoratti.net/25/04/vda300425.htm