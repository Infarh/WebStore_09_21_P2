using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace WebStore.Infrastructure.MiddleWare
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _Next;
        private readonly ILogger<TestMiddleWare> _Logger;

        public ErrorHandlerMiddleware(RequestDelegate next, ILogger<TestMiddleWare> Logger)
        {
            _Next = next;
            _Logger = Logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _Next(context);
            }
            catch (Exception e)
            {
                HandleError(e, context);
                throw;
            }
        }

        private void HandleError(Exception Error, HttpContext Context)
        {
            _Logger.LogError(Error, "Ошибка при обработке запроса к {0}", Context.Request.Path);
        }
    }
}
