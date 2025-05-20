using FastPackForShare.Bases.Generics;

namespace FastPackForShare.Interfaces;

public interface IMapperService
{
    TDestination MapDTOToEntity<TSource, TDestination>(TSource source)
    where TSource : GenericDTOModel
    where TDestination : GenericEntityModel;

    TDestination MapDTOToEntityList<TSource, TDestination>(TSource source)
    where TSource : IEnumerable<GenericDTOModel>
    where TDestination : IEnumerable<GenericEntityModel>;

    TDestination MapEntityToDTO<TSource, TDestination>(TSource source)
    where TSource : GenericEntityModel
    where TDestination : GenericDTOModel;

    TDestination MapEntityToDTOList<TSource, TDestination>(TSource source)
    where TSource : IEnumerable<GenericEntityModel>
    where TDestination : IEnumerable<GenericDTOModel>;

    TDestination MapEntityToExcelList<TSource, TDestination>(TSource source)
    where TSource : IEnumerable<GenericEntityModel>
    where TDestination : IEnumerable<GenericReportModel>;

    TDestination MapDTOToExcelList<TSource, TDestination>(TSource source)
    where TSource : IEnumerable<GenericDTOModel>
    where TDestination : IEnumerable<GenericReportModel>;
}
