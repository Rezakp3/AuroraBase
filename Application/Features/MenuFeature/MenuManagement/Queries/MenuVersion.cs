using Application.Common.Interfaces.Generals;
using Application.Common.Models;

namespace Application.Features.MenuFeature.MenuManagement.Queries;

public class MenuVersionQuery : IBaseRequest<int>;

internal class MenuVersionHandler(IUnitOfWork uow) : IBaseHandler<MenuVersionQuery, int>
{
    public async Task<ApiResult<int>> Handle(MenuVersionQuery request, CancellationToken cancellationToken)
    {
        var version = await uow.Settings.GetValueAsync<int>("MenuVersion", cancellationToken: cancellationToken);

        return ApiResult<int>.Success(version);
    }
}