using Application.Common.Interfaces.Generals;
using Application.Common.Models;
using Core.Entities.Auth;
using Mapster;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Utils.CustomAttributes;

namespace Application.Features.UsersFeature.ClaimManagement.Commands;

public class AddUserClaimCommand : IBaseRequest
{
    [RequiredFa,DisplayName("کاربر")]
    public long UserId { get; set; }
    [RequiredFa, DisplayName("تایپ"), MaxLengthFa(10)]
    public string Type { get; set; }
    [RequiredFa, DisplayName("مقدار"), MaxLengthFa(30)]
    public string Value { get; set; }
}

internal class AddUserClaimHandler(IUnitOfWork uow) : IBaseHandler<AddUserClaimCommand>
{
    public async Task<ApiResult> Handle(AddUserClaimCommand request, CancellationToken cancellationToken)
    {
        var userClaim = new UserClaim
        {
            ClaimType = request.Type,
            ClaimValue = request.Value,
            UserId = request.UserId
        };

        uow.Users.AddClaim(userClaim);
        var res = await uow.SaveChangesAsync(cancellationToken);
    
        return res.ToApiResult();
    }

}
