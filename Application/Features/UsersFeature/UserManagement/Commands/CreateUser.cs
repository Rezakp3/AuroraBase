using Application.Common.Interfaces.Generals;
using Application.Common.Models;
using Core.Entities.Auth;
using Core.Entities.Auth.Relation;
using Mapster;
using System.ComponentModel;
using Utils.CustomAttributes;

namespace Application.Features.UsersFeature.UserManagement.Commands;

public class CreateUserCommand : IBaseRequest
{
    private string _phoneNumber;
    [RequiredFa, DisplayName("شماره همراه"), MaxLengthFa(13)]
    public string PhoneNumber
    {
        get => _phoneNumber;
        set => _phoneNumber = value.Replace("+98", "0");
    }
    public int RoleId { get; set; }

    [RequiredFa, DisplayName("نام"), MaxLengthFa(50)]
    public string FirstName { get; set; }

    [DisplayName("نام خانوادگی"), MaxLengthFa(50)]
    public string LastName { get; set; }
}
internal class CreateUserCommandHandler(IUnitOfWork uow)
    : IBaseHandler<CreateUserCommand>
{
    public async Task<ApiResult> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var exist = await uow.Users.PhoneNumberExistForAdd(request.PhoneNumber, cancellationToken);
        
        if (exist)
            return ApiResult.Fail("شماره همراه از قبل موجود است");

        var user = request.Adapt<User>();
        user.UserRoles = [ new UserRole { RoleId = request.RoleId }];

        await uow.Users.AddAsync(user, cancellationToken);
        var res = await uow.SaveChangesAsync(cancellationToken);

        return res.ToApiResult();
    }
}