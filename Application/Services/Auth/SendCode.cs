using Core.CustomAttributes;
using Core.ViewModel.Base;
using DNTPersianUtils.Core;
using Gateway;
using Infrastructure;
using MediatR;

namespace Application.Services.Auth;

public class SendCodeRequest : IRequest<ApiResult>
{
    [MaxLengthFa(13), RequiredFa]
    public string PhoneNumber { get; set; }
} 

public class SendCodeRequestHandler(SmsSender smsSender, DBContext context) : IRequestHandler<SendCodeRequest, ApiResult>
{
    public async Task<ApiResult> Handle(SendCodeRequest request, CancellationToken cancellationToken)
    {
        await smsSender.SendOtp(request.PhoneNumber, "1234");
        return new();
    }
}
