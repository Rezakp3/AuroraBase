using Application.Common.Interfaces.Generals;
using Application.Common.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Utils.CustomAttributes;

namespace Application.Features.ServiceFeatures.ServiceRelation.Queries;

public class ServiceRoleDropDownQuery : IBaseRequest<IEnumerable<BaseDropDown<int>>>
{
    [RequiredFa, DisplayName("سرویس")]
    public int ServiceId { get; set; }
    public string Search { get; set; }
}

internal class ServiceRoleDropDownHandler(IUnitOfWork uow)
    : IBaseHandler<ServiceRoleDropDownQuery, IEnumerable<BaseDropDown<int>>>
{
    public async Task<ApiResult<IEnumerable<BaseDropDown<int>>>> Handle(ServiceRoleDropDownQuery request, CancellationToken cancellationToken)
    {
        var roles = await uow.Services.RoleDropDown(request.ServiceId, request.Search, cancellationToken);

        return ApiResult<IEnumerable<BaseDropDown<int>>>.Success(roles);
    }
}