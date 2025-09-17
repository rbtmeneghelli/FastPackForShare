using FastPackForShare.Bases.Generics;
using FastPackForShare.Enums;
using FastPackForShare.Extensions;

namespace FastPackForShare.Default;

public abstract record BaseDTOModel : GenericDTOModel
{
    private long? _id;

    [Display(Name = "Id")]
    public long? Id { get { return _id; } set { _id = value.HasValue ? (value > 0 ? value : null) : null; } }

    //#region Código valido a partir do NET 10

    ///// <summary>
    ///// A palavra chave field faz o papel do campo private, nesse caso seria o _id
    ///// </summary>

    //[Display(Name = "Id")]
    //public long? Id { get { return field; } set { field = value.HasValue ? (value > 0 ? value : null) : null; } }

    //#endregion

    [Display(Name = "Ativo")]
    public bool? IsActive { get; set; }

    public string GetStatus() => IsActive.HasValue ? 
                                (IsActive.Value ? EnumStatus.Active.GetDisplayShortName() : 
                                 EnumStatus.Inactive.GetDisplayShortName()) : EnumStatus.Inactive.GetDisplayShortName();
    public BaseDTOModel()
    {
    }

    public virtual BaseDTOModel ChangeStatusToInactive<T>(BaseDTOModel obj)
    {
        return obj with { IsActive = false };
    }

    public virtual BaseDTOModel ChangeStatusToActive<T>(BaseDTOModel obj)
    {
        return obj with { IsActive = true };
    }
}
