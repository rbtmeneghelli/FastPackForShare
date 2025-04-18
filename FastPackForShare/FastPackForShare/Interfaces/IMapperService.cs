namespace FastPackForShare.Interfaces;

public interface IMapperService
{
    TDestination ApplyMapToEntity<TSource, TDestination>(TSource source);
}
