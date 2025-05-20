using AutoMapper;
using FastPackForShare.Bases.Generics;
using FastPackForShare.Constants;
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
           where TSource : GenericDTOModel
           where TDestination : GenericEntityModel
    {
        BaseDomainException.WhenIfNull(source, ConstantValidation.MAPPER);
        return _iMapperService.Map<TDestination>(source);
    }

    public TDestination MapDTOToEntityList<TSource, TDestination>(TSource source)
           where TSource : IEnumerable<GenericDTOModel>
           where TDestination : IEnumerable<GenericEntityModel>
    {
        BaseDomainException.WhenIfNull(source, ConstantValidation.MAPPER);
        return _iMapperService.Map<TDestination>(source);
    }

    public TDestination MapEntityToDTO<TSource, TDestination>(TSource source)
           where TSource : GenericEntityModel
           where TDestination : GenericDTOModel
    {
        BaseDomainException.WhenIfNull(source, ConstantValidation.MAPPER);
        return _iMapperService.Map<TDestination>(source);
    }

    public TDestination MapEntityToDTOList<TSource, TDestination>(TSource source)
           where TSource : IEnumerable<GenericEntityModel>
           where TDestination : IEnumerable<GenericDTOModel>
    {
        BaseDomainException.WhenIfNull(source, ConstantValidation.MAPPER);
        return _iMapperService.Map<TDestination>(source);
    }

    public TDestination MapEntityToExcelList<TSource, TDestination>(TSource source)
       where TSource : IEnumerable<GenericEntityModel>
       where TDestination : IEnumerable<GenericReportModel>
    {
        BaseDomainException.WhenIfNull(source, ConstantValidation.MAPPER);
        return _iMapperService.Map<TDestination>(source);
    }

    public TDestination MapDTOToExcelList<TSource, TDestination>(TSource source)
           where TSource : IEnumerable<GenericDTOModel>
           where TDestination : IEnumerable<GenericReportModel>
    {
        BaseDomainException.WhenIfNull(source, ConstantValidation.MAPPER);
        return _iMapperService.Map<TDestination>(source);
    }
}
