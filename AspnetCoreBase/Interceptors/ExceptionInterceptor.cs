using AspnetCoreBase.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspnetCoreBase.Interceptors
{
    public class ExceptionInterceptor : ExceptionFilterAttribute
    {
        private ILogger<ExceptionInterceptor> _logger;

        public ExceptionInterceptor(ILogger<ExceptionInterceptor> logger)
        {
            _logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            var apiError = new ApiResponseModel();
            int code = 200;

            if (context.Exception is ApiException)
            {
                var ex = context.Exception as ApiException;
                apiError.Message = string.Format("Exception: {0}. Inner Exception: {1}", context.Exception.Message, context.Exception.InnerException?.Message);
                code = ex.StatusCode;
            }
            else if (context.Exception is UnauthorizedAccessException)
            {
                apiError.Message = "Unauthorized Access";
                code = 401;
            }
            else
            {
                apiError.Message = string.Format("Exception: {0}. Inner Exception: {1}. Stack: {2}", context.Exception.Message, context.Exception.InnerException?.Message, context.Exception.StackTrace);
                code = 500;
            }

            apiError.StatusCode = code;
            context.HttpContext.Response.StatusCode = code;
            context.Result = new JsonResult(apiError);

            _logger.LogError("{@Username} {@Activity} {@Message}", "System", "API Error", apiError.Message);

            base.OnException(context);
        }
    }
}