using Application.Common.Interfaces.Generals;
using Core.Entities.Auth;

namespace Application.Common.Interfaces.Repositories;

public interface IUserRepository : IRepository<User, long>
{
}