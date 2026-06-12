using Application.Common.Interfaces.Generals;
using Application.Common.Models;
using Application.Features.ServiceFeatures.Models;

namespace Application.Features.ServiceFeatures.Commands;

public class UpdateServiceCommand : UpdateServiceIm, IBaseRequest;

internal class UpdateServiceHandler(IUnitOfWork uow) : IBaseHandler<UpdateServiceCommand>
{
    public async Task<ApiResult> Handle(UpdateServiceCommand request, CancellationToken cancellationToken)
    {
        var service = await uow.Services.GetByIdAsync(request.Id, cancellationToken);

        if (service is null)
            return ApiResult.NotFound("سرویس");

        service.ServiceName = request.ServiceName;
        service.ServiceIdentifier = request.ServiceIdentifier;
        service.Address = request.Address;
        service.Description = request.Description;

        var res = await uow.SaveChangesAsync(cancellationToken);
        return res.ToApiResult();
    }
}