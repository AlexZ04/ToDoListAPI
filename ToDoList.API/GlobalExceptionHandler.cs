using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ToDoList.Common.Exceptions;

namespace ToDoList.API
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            ProblemDetails problemDetails = new ProblemDetails();

            if (exception is KeyNotFoundException)
                problemDetails = new ProblemDetails
                {
                    Status = 404,
                    Title = "Not found",
                    Detail = exception.Message
                };

            else if (exception is CustomException custom)
                problemDetails = new ProblemDetails
                {
                    Status = custom.Code,
                    Title = "Not found",
                    Detail = custom.Message
                };

            if (problemDetails.Status != null)
                httpContext.Response.StatusCode = problemDetails.Status.Value;

            await httpContext.Response
                .WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
    }
}
