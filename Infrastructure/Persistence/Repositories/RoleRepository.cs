using Application.Common.Interfaces.Repositories;
using Core.Entities.Auth;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class RoleRepository(MyContext context) : Repository<Role, int>(context), IRoleRepository
{}