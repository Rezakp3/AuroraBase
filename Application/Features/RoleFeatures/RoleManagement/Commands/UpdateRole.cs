using Application.Common.Interfaces.Generals;
using Application.Common.Models;
using System.ComponentModel;
using Utils.CustomAttributes;

namespace Application.Features.RoleFeatures.RoleManagement.Commands;

public class UpdateRoleCommand : IBaseRequest
{
    [RequiredFa, DisplayName("آیدی")]
    public int Id { get; set; }
    [RequiredFa, DisplayName("نام نقش"), MaxLengthFa(20)]
    public string Name { get; set; }
    [DisplayName("تیتر"), MaxLengthFa(50)]
    public string Title { get; set; }
}

internal class UpdateRoleHandler(IUnitOfWork uow) : IBaseHandler<UpdateRoleCommand>
{
    public async Task<ApiResult> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await uow.Roles.GetByIdAsync(request.Id, cancellationToken);
        if (role is null)
            return ApiResult.NotFound("نقش");

        role.Name = request.Name;
        role.Title = request.Title;

        uow.Roles.Update(role);
        var res = await uow.SaveChangesAsync(cancellationToken);

        return res.ToApiResult();
    }
}