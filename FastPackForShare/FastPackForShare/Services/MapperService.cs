using AutoMapper;
using FastPackForShare.Default;
using FastPackForShare.Interfaces;

namespace FastPackForShare.Services;

public sealed class MapperService : IMapperService
{
    private readonly IMapper _iMapperService;

    public MapperService(IMapper iMapperService)
    {
        _iMapperService = iMapperService;
    }

    public TDestination MapDTOToEntity<TSource, TDestination>(TSource source) 
           where TSource : BaseDTOModel
           where TDestination : BaseEntityModel                                                                   
    {
        BaseDomainException.WhenIfNull(source, "Ocorreu um erro no processo de mapeamento do objeto, pois ele está Null");
        return _iMapperService.Map<TDestination>(source);
    }

    public TDestination MapEntityToDTO<TSource, TDestination>(TSource source)
       where TSource : BaseEntityModel
       where TDestination : BaseDTOModel
    {
        BaseDomainException.WhenIfNull(source, "Ocorreu um erro no processo de mapeamento do objeto, pois ele está Null");
        return _iMapperService.Map<TDestination>(source);
    }
}
