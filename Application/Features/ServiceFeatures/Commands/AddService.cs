using Application.Common.Interfaces.Generals;
using Application.Common.Models;
using Application.Features.ServiceFeatures.Models;
using Core.Entities.Auth;
using Mapster;

namespace Application.Features.ServiceFeatures.Commands;

public class AddServiceCommand : AddServiceIm , IBaseRequest;

internal class AddServiceHandler(IUnitOfWork uow) : IBaseHandler<AddServiceCommand>
{
    public async Task<ApiResult> Handle(AddServiceCommand request, CancellationToken cancellationToken)
    {
        var service = request.Adapt<Service>();

        await uow.Services.AddAsync(service,cancellationToken);

        var res = await uow.SaveChangesAsync(cancellationToken);

        return res.ToApiResult();
    }
}
