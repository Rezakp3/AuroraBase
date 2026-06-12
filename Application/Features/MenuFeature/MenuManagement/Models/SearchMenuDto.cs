using Application.Common.Models.Pagination;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Utils.CustomAttributes;

namespace Application.Features.MenuFeature.MenuManagement.Models;

public class SearchMenuDto : PagingOption
{
    [DisplayName("تیتر"), MaxLengthFa(50)]
    public string Title { get; set; }
    [DisplayName("آدرس"), MaxLengthFa(150)]
    public string Route { get; set; }
    public int? ParentId { get; set; }

    public int? Priority { get; set; }
    public bool? IsActive { get; set; } 
}
