using FastPackForShare.Default;
using FluentValidation;

namespace FastPackForShare.Bases;

public sealed class BaseValidatorModel<TBaseDtoModel> : AbstractValidator<TBaseDtoModel> where TBaseDtoModel : BaseDTOModel
{
}
