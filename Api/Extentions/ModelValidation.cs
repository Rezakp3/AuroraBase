using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Application.Common.Models;

namespace Api.Extentions;

public class ValidateModelStateAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            context.Result = new BadRequestObjectResult(new ApiResult(406, false)
            {
                Message = "خطای اعتبار سنجی",
                ValidationErrors = [.. context.ModelState.Select(s => new ValidationError()
                {
                    Errors = [.. s.Value.Errors.Select(e => e.ErrorMessage)],
                    PropertyName = s.Key
                })],
            });
        }
    }
}
