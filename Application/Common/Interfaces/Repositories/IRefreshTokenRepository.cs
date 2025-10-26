using Application.Common.Interfaces.Generals;
using Core.Entities.Auth;

namespace Application.Common.Interfaces.Repositories;

public interface IRefreshTokenRepository : IRepository<RefreshToken, Guid>
{

}