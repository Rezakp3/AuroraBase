using Application.Common.Interfaces.Repositories;
using Core.Entities.Auth;
using Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class UserRepository(MyContext context) : Repository<User, long>(context), IUserRepository
{}