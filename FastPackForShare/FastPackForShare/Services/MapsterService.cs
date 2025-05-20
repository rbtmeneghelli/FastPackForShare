using FastPackForShare.Bases.Generics;
using FastPackForShare.Constants;
using FastPackForShare.Default;
using FastPackForShare.Interfaces;
using Mapster;

namespace FastPackForShare.Services;

public sealed class MapsterService : IMapsterService
{
    public TDestination MapDTOToEntity<TSource, TDestination>(TSource source)
           where TSource : GenericDTOModel
           where TDestination : GenericEntityModel
    {
        BaseDomainException.WhenIfNull(source, ConstantValidation.MAPPER);
        return source.Adapt<TDestination>();
    }

    public TDestination MapDTOToEntityList<TSource, TDestination>(TSource source)
       where TSource : IEnumerable<GenericDTOModel>
       where TDestination : IEnumerable<GenericEntityModel>
    {
        BaseDomainException.WhenIfNull(source, ConstantValidation.MAPPER);
        return source.Adapt<TDestination>();
    }

    public TDestination MapEntityToDTO<TSource, TDestination>(TSource source)
       where TSource : GenericEntityModel
       where TDestination : GenericDTOModel
    {
        BaseDomainException.WhenIfNull(source, ConstantValidation.MAPPER);
        return source.Adapt<TDestination>();
    }

    public TDestination MapEntityToDTOList<TSource, TDestination>(TSource source)
        where TSource : IEnumerable<GenericEntityModel>
        where TDestination : IEnumerable<GenericDTOModel>
    {
        BaseDomainException.WhenIfNull(source, ConstantValidation.MAPPER);
        return source.Adapt<TDestination>();
    }

    public TDestination MapEntityToExcelList<TSource, TDestination>(TSource source)
           where TSource : IEnumerable<GenericEntityModel>
           where TDestination : IEnumerable<GenericReportModel>
    {
        BaseDomainException.WhenIfNull(source, ConstantValidation.MAPPER);
        return source.Adapt<TDestination>();
    }

    public TDestination MapDTOToExcelList<TSource, TDestination>(TSource source)
           where TSource : IEnumerable<GenericDTOModel>
           where TDestination : IEnumerable<GenericReportModel>
    {
        BaseDomainException.WhenIfNull(source, ConstantValidation.MAPPER);
        return source.Adapt<TDestination>();
    }
}

