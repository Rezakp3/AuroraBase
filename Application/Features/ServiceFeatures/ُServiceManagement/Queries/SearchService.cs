using Application.Common.Interfaces.Generals;
using Application.Common.Models;
using Application.Common.Models.Pagination;
using Application.Features.ServiceFeatures.Models;

namespace Application.Features.ServiceFeatures.Queries;

public class SearchServiceQuery 
    : SearchServiceIm, IBaseRequest<PaginatedList<ServiceDto>>;

internal class SearchServiceHandler(IUnitOfWork uow)
    : IBaseHandler<SearchServiceQuery, PaginatedList<ServiceDto>>
{
    public async Task<ApiResult<PaginatedList<ServiceDto>>> Handle(SearchServiceQuery request, CancellationToken cancellationToken)
    {
        var data = await uow.Services.Search(request, cancellationToken);

        return ApiResult<PaginatedList<ServiceDto>>.Success(data);
    }
}