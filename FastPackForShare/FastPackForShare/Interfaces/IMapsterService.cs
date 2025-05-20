using FastPackForShare.Default;

namespace FastPackForShare.Interfaces;

public interface IMapsterService
{
    TDestination MapDTOToEntity<TSource, TDestination>(TSource source)
    where TSource : BaseDTOModel
    where TDestination : BaseEntityModel;

    TDestination MapEntityToDTO<TSource, TDestination>(TSource source)
    where TSource : BaseEntityModel
    where TDestination : BaseDTOModel;
}

