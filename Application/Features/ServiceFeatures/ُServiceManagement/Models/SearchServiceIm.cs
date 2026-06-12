using Application.Common.Models.Pagination;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Utils.CustomAttributes;

namespace Application.Features.ServiceFeatures.Models;

public class SearchServiceIm : PagingOption
{
    public string ServiceName { get; set; }
    public string Description { get; set; }
    public string Address { get; set; }
    public string ServiceIdentifier { get; set; }
}
