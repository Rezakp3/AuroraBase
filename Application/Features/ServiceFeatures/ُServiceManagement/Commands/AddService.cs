using Application.Common.Interfaces.Generals;
using Application.Common.Models;
using Application.Features.ServiceFeatures.Models;
using Core.Entities.Auth;
using Core.Entities.Auth.Relation;
using Mapster;

namespace Application.Features.ServiceFeatures.Commands;

public class AddServiceCommand : AddServiceIm, IBaseRequest
{
    public IEnumerable<int> Roles { get; set; }
}

internal class AddServiceHandler(IUnitOfWork uow) : IBaseHandler<AddServiceCommand>
{
    public async Task<ApiResult> Handle(AddServiceCommand request, CancellationToken cancellationToken)
    {
        var service = request.Adapt<Service>();

        if (request.Roles is not null && request.Roles.Any())
            service.RoleServices = [.. request.Roles
                .Select(x => new RoleService
                {
                    RoleId = x
                })];

        await uow.Services.AddAsync(service, cancellationToken);

        var res = await uow.SaveChangesAsync(cancellationToken);

        return res.ToApiResult();
    }
}
