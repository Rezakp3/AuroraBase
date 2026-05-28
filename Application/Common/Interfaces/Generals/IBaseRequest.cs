using Application.Common.Models;
using MediatR;

namespace Application.Common.Interfaces.Generals;

public interface IBaseRequest : IRequest<ApiResult>;
public interface IBaseRequest<T> : IRequest<ApiResult<T>>;

public interface IBaseHandler<TRequest> : IRequestHandler<TRequest, ApiResult>
    where TRequest : IBaseRequest;
public interface IBaseHandler<TRequest, TResponse> : IRequestHandler<TRequest, ApiResult<TResponse>>
    where TRequest : IBaseRequest<TResponse>;

