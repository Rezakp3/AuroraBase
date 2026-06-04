using Application.Common.Interfaces.Generals;
using Application.Common.Models;
using Application.Common.Models.Pagination;
using Application.Features.ServiceFeatures.Models;
using System.ComponentModel;
using Utils.CustomAttributes;

namespace Application.Features.ServiceFeatures.Queries;

public class GetServicByMenuIdQuery : PagingOption, IBaseRequest<PaginatedList<ServiceDto>>
{
    [RequiredFa, DisplayName("آیدی منو")]
    public int MenuId { get; set; }
}

internal class GetServicByMenuIdHandler(IUnitOfWork uow)
    : IBaseHandler<GetServicByMenuIdQuery, PaginatedList<ServiceDto>>
{
    public async Task<ApiResult<PaginatedList<ServiceDto>>> Handle(GetServicByMenuIdQuery request, CancellationToken cancellationToken)
    {
        PaginatedList<ServiceDto> data = await uow.Services.GetByMenuIdAsync(request.MenuId, request, cancellationToken);

        return ApiResult<PaginatedList<ServiceDto>>.Success(data);
    }
}