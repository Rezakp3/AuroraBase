using Application.Common.Interfaces.Generals;
using Application.Common.Models;
using Core.Entities.Auth;
using Mapster;
using System.ComponentModel;
using Utils.CustomAttributes;

namespace Application.Features.RoleFeatures.RoleManagement.Commands;

public class AddRoleCommand : IBaseRequest
{
    [RequiredFa, DisplayName("نام نقش"), MaxLengthFa(20)]
    public string Name { get; set; }
    [DisplayName("تیتر"), MaxLengthFa(50)]
    public string Title { get; set; }
}

internal class AddRoleHandler(IUnitOfWork uow) : IBaseHandler<AddRoleCommand>
{
    public async Task<ApiResult> Handle(AddRoleCommand request, CancellationToken cancellationToken)
    {
        var role = request.Adapt<Role>();

        await uow.Roles.AddAsync(role,cancellationToken);

        var res = await uow.SaveChangesAsync(cancellationToken);

        return res.ToApiResult();
    }
}