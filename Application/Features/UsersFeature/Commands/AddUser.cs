using Application.Common.Interfaces.Generals;
using Application.Common.Models;
using Core.Entities.Auth;
using System.ComponentModel;
using Utils.CustomAttributes;

namespace Application.Features.MenuFeature.Commands;

public class AddUserCommand : IBaseRequest
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
internal class AddUserCommandHandler(IUnitOfWork uow) : IBaseHandler<AddUserCommand>
{
    public async Task<ApiResult> Handle(AddUserCommand request, CancellationToken cancellationToken)
    {
        var exist = await uow.Users.PhoneNumberExistForAdd(request.PhoneNumber, cancellationToken);
        if (exist)
            return ApiResult.Fail("شماره همراه از قبل موجود است");

        return ApiResult.Success();
    }
}