using System;
using Microsoft.AspNetCore.Http;

namespace CleanArchitecture.Common.Middleware
{
    public class ApiExceptionOptions
    {
        public Action<HttpContext, Exception, ApiError> AddResponseDetails { get; set; }
    }
}
