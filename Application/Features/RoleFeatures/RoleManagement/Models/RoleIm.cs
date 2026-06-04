using Application.Common.Models.Pagination;

namespace Application.Features.RoleFeatures.RoleManagement.Models;

public class RoleIm : PagingOption
{
    public string Name { get; set; }
    public string Title { get; set; }
}
