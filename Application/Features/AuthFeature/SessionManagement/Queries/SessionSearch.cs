using Application.Common.Interfaces.Generals;
using Application.Common.Models;
using Application.Common.Models.Pagination;
using Application.Features.AuthFeature.SessionManagement.Models;

namespace Application.Features.AuthFeature.SessionManagement.Queries;

public class SessionSearchQuery : SessionIm, IBaseRequest<PaginatedList<SessionDto>>;

internal class SessionSearchHandler(IUnitOfWork uow)
    : IBaseHandler<SessionSearchQuery, PaginatedList<SessionDto>>
{
    public async Task<ApiResult<PaginatedList<SessionDto>>> Handle(SessionSearchQuery request, CancellationToken cancellationToken)
    {
        var sessions = await uow.Sessions.Search(request, cancellationToken);

        return ApiResult<PaginatedList<SessionDto>>.Success(sessions);
    }
}