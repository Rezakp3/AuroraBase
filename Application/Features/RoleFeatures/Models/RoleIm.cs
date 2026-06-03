using Application.Common.Models.Pagination;

namespace Application.Features.RoleFeatures.Models;

public class RoleIm : PagingOption
{
    public string Name { get; set; }
    public string Title { get; set; }
}
