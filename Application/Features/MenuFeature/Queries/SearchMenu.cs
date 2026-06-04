using Application.Common.Interfaces.Generals;
using Application.Common.Models;
using Application.Common.Models.Pagination;
using Application.Features.MenuFeature.Models;

namespace Application.Features.MenuFeature.Queries;

public class SearchMenuQuery : SearchMenuDto, IBaseRequest<PaginatedList<MenuDto>>;

internal class SearchMenuHandler(IUnitOfWork uow) : IBaseHandler<SearchMenuQuery, PaginatedList<MenuDto>>
{
    public async Task<ApiResult<PaginatedList<MenuDto>>> Handle(SearchMenuQuery request, CancellationToken cancellationToken)
    {
        var data = await uow.Menus.SearchAsync(request, cancellationToken);

        return ApiResult<PaginatedList<MenuDto>>.Success(data);
    }
}