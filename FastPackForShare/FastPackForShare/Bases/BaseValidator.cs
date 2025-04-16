using FastPackForShare.Default;
using FluentValidation;

namespace FastPackForShare.Bases;

public sealed class BaseValidator<TBaseDtoModel> : AbstractValidator<TBaseDtoModel> where TBaseDtoModel : BaseDtoModel
{
}
