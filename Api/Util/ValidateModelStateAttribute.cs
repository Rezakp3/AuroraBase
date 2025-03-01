using Core.ViewModel.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Web.Util.ValidateModel;

public class ValidateModelStateAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            context.Result = new BadRequestObjectResult(new ApiResult(406, false)
            {
                ValidationErrors = context.ModelState.Select(static s => new ValidateProperty()
                {
                    Errors = s.Value?.Errors.Select(static e => e.ErrorMessage).ToList(),
                    PropertyName = s.Key
                }),
            });
        }
    }
}