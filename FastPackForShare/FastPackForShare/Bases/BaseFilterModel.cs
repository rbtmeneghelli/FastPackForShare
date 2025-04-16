namespace FastPackForShare.Default;

public abstract record BaseFilterModel
{
    public long? Id { get; set; }
    public bool? IsActive { get; set; }
    public int? PageIndex { get; set; }
    public int? PageSize { get; set; }
}
