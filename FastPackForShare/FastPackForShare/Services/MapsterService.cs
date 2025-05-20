using FastPackForShare.Default;
using FastPackForShare.Interfaces;
using Mapster;

namespace FastPackForShare.Services;

public sealed class MapsterService : IMapsterService
{
    public TDestination MapDTOToEntity<TSource, TDestination>(TSource source)
           where TSource : BaseDTOModel
           where TDestination : BaseEntityModel
    {
        BaseDomainException.WhenIfNull(source, "Ocorreu um erro no processo de mapeamento do objeto, pois ele está Null");
        return source.Adapt<TDestination>();
    }

    public TDestination MapEntityToDTO<TSource, TDestination>(TSource source)
       where TSource : BaseEntityModel
       where TDestination : BaseDTOModel
    {
        BaseDomainException.WhenIfNull(source, "Ocorreu um erro no processo de mapeamento do objeto, pois ele está Null");
        return source.Adapt<TDestination>();
    }
}

