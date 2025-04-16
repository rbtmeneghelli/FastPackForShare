using FastPackForShare.Bases.Generics;
using FastPackForShare.Enums;
using FastPackForShare.Extensions;

namespace FastPackForShare.Default;

/// <summary>
/// A partir do NET 10 com C# 14 não será necessario criar uma propriedade privada, esse processo será substituido pela palavra chave field.
/// </summary>
public abstract class BaseEntityModel : GenericEntityModel
{
    private long? _id;
    public long? Id { get { return _id; } set { _id = value.HasValue ? (value > 0 ? value : null) : null; } }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool? IsActive { get; set; } = false;

    public string GetStatus() => IsActive.HasValue ?
                                (IsActive.Value ? EnumStatus.Active.GetDisplayShortName() :
                                 EnumStatus.Inactive.GetDisplayShortName()) : EnumStatus.Inactive.GetDisplayShortName();
    public BaseEntityModel()
    {
    }

    protected abstract void CreateEntityIsValid();
    protected abstract void UpdateEntityIsValid();
}
