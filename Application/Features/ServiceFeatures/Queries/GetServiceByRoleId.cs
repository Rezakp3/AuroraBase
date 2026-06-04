using Application.Common.Interfaces.Generals;
using Application.Common.Models;
using Application.Common.Models.Pagination;
using Application.Features.ServiceFeatures.Models;
using System.ComponentModel;
using Utils.CustomAttributes;

namespace Application.Features.ServiceFeatures.Queries;

public class GetServiceByRoleIdQuery : PagingOption, IBaseRequest<PaginatedList<ServiceDto>>
{
    [RequiredFa, DisplayName("آیدی نقش")]
    public int RoleId { get; set; }
}

internal class GetServicByRoleIdHandler(IUnitOfWork uow) : IBaseHandler<GetServiceByRoleIdQuery, PaginatedList<ServiceDto>>
{
    public async Task<ApiResult<PaginatedList<ServiceDto>>> Handle(GetServiceByRoleIdQuery request, CancellationToken cancellationToken)
    {
        PaginatedList<ServiceDto> data = await uow.Services.GetByRoleIdAsync(request.RoleId, request, cancellationToken);

        return ApiResult<PaginatedList<ServiceDto>>.Success(data);
    }
}