using Grpc.Core;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using DiagnosKit.Core.Logging.Contracts;

namespace KwikNesta.Gateway.Svc.API.Middlewares
{
    public class GrpcExceptionFilter : IAsyncExceptionFilter
    {
        private readonly ILoggerManager _logger;

        public GrpcExceptionFilter(ILoggerManager logger)
        {
            _logger = logger;
        }

        public Task OnExceptionAsync(ExceptionContext context)
        {
            if (context.Exception is RpcException rpcEx)
            {
                var response = new
                {
                    Successful = false,
                    Message = rpcEx.Status.Detail,
                    Status = rpcEx.StatusCode switch
                    {
                        StatusCode.InvalidArgument => StatusCodes.Status400BadRequest,
                        StatusCode.Unauthenticated => StatusCodes.Status401Unauthorized,
                        StatusCode.PermissionDenied => StatusCodes.Status403Forbidden,
                        StatusCode.NotFound => StatusCodes.Status404NotFound,
                        _ => StatusCodes.Status500InternalServerError
                    }
                };

                _logger.LogError(rpcEx, rpcEx.Message);
                context.Result = new ObjectResult(response) { StatusCode = response.Status };
            }
            else
            {
                var response = new
                {
                    Successful = false,
                    context.Exception.Message,
                    Status = context.HttpContext.Response.StatusCode
                };

                _logger.LogError(context.Exception, context.Exception.Message);
                context.Result = new ObjectResult(response) { StatusCode = response.Status };
            }

            context.ExceptionHandled = true;
            return Task.CompletedTask;
        }
    }
}
