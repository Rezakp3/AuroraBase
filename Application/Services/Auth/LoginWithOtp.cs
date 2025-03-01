using Core.ViewModel.AuthVm;
using Core.ViewModel.Base;
using MediatR;

namespace Application.Services.Auth;

public class LoginWithOtpRequest : IRequest<ApiResult<LoginDto>>
{
    public string PhoneNumber { get; set; }
    public string Otp { get; set; }
}