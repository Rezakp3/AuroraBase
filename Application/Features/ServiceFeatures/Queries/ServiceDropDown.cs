using Application.Common.Interfaces.Generals;
using Application.Common.Models;
using Application.Common.Models.Pagination;

namespace Application.Features.ServiceFeatures.Queries;

public class ServiceDropDownQuery
    : CursorPagingOption<int>, IBaseRequest<CursorPaginatedList<BaseDropDown<int>, int>>
{
    public string Search { get; set; }
}

internal class ServiceDropDownHandler(IUnitOfWork uow)
    : IBaseHandler<ServiceDropDownQuery, CursorPaginatedList<BaseDropDown<int>, int>>
{
    public async Task<ApiResult<CursorPaginatedList<BaseDropDown<int>, int>>> Handle(ServiceDropDownQuery request, CancellationToken cancellationToken)
    {
        var data = await uow.Services.DropDown(request.Search, request, cancellationToken);

        return ApiResult<CursorPaginatedList<BaseDropDown<int>, int>>.Success(data);
    }
}