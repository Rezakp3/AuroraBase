using Application.Common.Interfaces.Repositories;
using Core.Entities.Auth;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class RefreshTokenRepository(MyContext context) : Repository<RefreshToken, Guid>(context), IRefreshTokenRepository
{

}