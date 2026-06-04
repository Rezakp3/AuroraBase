using Application.Common.Interfaces.Generals;
using Application.Common.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Utils.CustomAttributes;

namespace Application.Features.ServiceFeatures.Commands;

public class DeleteServiceCommand : IBaseRequest
{
    [RequiredFa,DisplayName("آیدی")]
    public int Id { get; set; }
}

internal class DeleteServiceHandler(IUnitOfWork uow) : IBaseHandler<DeleteServiceCommand>
{
    public async Task<ApiResult> Handle(DeleteServiceCommand request, CancellationToken cancellationToken)
    {
        var service = await uow.Services.GetByIdAsync(request.Id, cancellationToken);

        if (service is null)
            return ApiResult.NotFound("سرویس");

        uow.Services.Delete(service);

        var res = await uow.SaveChangesAsync(cancellationToken);

        return res.ToApiResult();
    }
}