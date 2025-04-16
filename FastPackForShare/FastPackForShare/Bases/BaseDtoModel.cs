using FastPackForShare.Bases.Generics;
using FastPackForShare.Enums;
using FastPackForShare.Extensions;

namespace FastPackForShare.Default;

public abstract record BaseDtoModel : GenericDtoModel
{
    private long? _id;

    [Display(Name = "Id")]
    public long? Id { get { return _id; } set { _id = value.HasValue ? (value > 0 ? value : null) : null; } }

    [Display(Name = "Ativo")]
    public bool? IsActive { get; set; }

    public string GetStatus() => IsActive.HasValue ? 
                                (IsActive.Value ? EnumStatus.Active.GetDisplayShortName() : 
                                 EnumStatus.Inactive.GetDisplayShortName()) : EnumStatus.Inactive.GetDisplayShortName();
    public BaseDtoModel()
    {
    }

    public virtual BaseDtoModel ChangeStatusToInactive<T>(BaseDtoModel obj)
    {
        return obj with { IsActive = false };
    }

    public virtual BaseDtoModel ChangeStatusToActive<T>(BaseDtoModel obj)
    {
        return obj with { IsActive = true };
    }
}
