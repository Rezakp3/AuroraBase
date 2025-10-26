using Application.Common.Interfaces.Repositories;
using Core.Entities.Auth;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class ServiceRepository(MyContext context) : Repository<Service, int>(context), IServiceRepository
{}