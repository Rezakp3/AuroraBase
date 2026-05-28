using Application.Common.Interfaces.Generals;
using Application.Common.Models;
using Core.Entities.Auth;

namespace Application.Features.MenuFeature.Commands;

public class AddUserCommand : IBaseRequest
{
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public int RoleId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}
internal class AddUserCommandHandler(IUnitOfWork uow) : IBaseHandler<AddUserCommand>
{
    public async Task<ApiResult> Handle(AddUserCommand request, CancellationToken cancellationToken)
    {
        var exist = await uow.Users.UserNameExistForAdd(request.UserName, cancellationToken);
        if (exist)
            return ApiResult.Fail("نام کاربری یا کلمه عبور از قبل وجود دارد");

        return ApiResult.Success();
    }
}