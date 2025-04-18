using AutoMapper;
using FastPackForShare.Default;
using FastPackForShare.Extensions;
using FastPackForShare.Interfaces;

namespace FastPackForShare.Services;

public sealed class MapperService : IMapperService
{
    private readonly IMapper _iMapperService;

    public MapperService(IMapper iMapperService)
    {
        _iMapperService = iMapperService;
    }

    public TDestination ApplyMapToEntity<TSource, TDestination>(TSource source)
    {
        BaseDomainException.When(GuardClauseExtension.IsNull(source), "Ocorreu um erro no processo de mapeamento do objeto, pois ele está Null");
        return _iMapperService.Map<TDestination>(source);
    }
}
