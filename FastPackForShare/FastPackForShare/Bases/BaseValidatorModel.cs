using FastPackForShare.Default;
using FluentValidation;

namespace FastPackForShare.Bases;

public class BaseValidatorModel<TBaseDtoModel> : AbstractValidator<TBaseDtoModel> where TBaseDtoModel : BaseDTOModel
{
}
