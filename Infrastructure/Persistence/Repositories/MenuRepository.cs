using Application.Common.Interfaces.Repositories;
using Core.Entities.Auth;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class MenuRepository(MyContext context) : Repository<Menu, int>(context), IMenuRepository
{ }