namespace Atelier.Api._DTOs
{
    public class ResponseTypeDto
    {
        public string Title { get; set; } = string.Empty;
        public int Status { get; set; }
        public string? Detail { get; set; }
    }

    public class ResponseType400Dto : ResponseTypeDto
    {
        public Dictionary<string, string[]>? Errors { get; set; }
        public ResponseType400Dto(string? detail = null, Dictionary<string, string[]>? errors = null)
        {
            Title = "Bad Request";
            Status = StatusCodes.Status400BadRequest;
            Detail = detail;
            Errors = errors;
        }
    }

    public class ResponseType401Dto : ResponseTypeDto
    {
        public ResponseType401Dto(string? detail = null)
        {
            Title = "Unauthorized";
            Status = StatusCodes.Status401Unauthorized;
            Detail = detail;
        }
    }

    public class ResponseType403Dto : ResponseTypeDto
    {
        public ResponseType403Dto(string? detail = null)
        {
            Title = "Forbidden";
            Status = StatusCodes.Status403Forbidden;
            Detail = detail;
        }
    }

    public class ResponseType404Dto : ResponseTypeDto
    {
        public ResponseType404Dto(string? detail = null)
        {
            Title = "Not Found";
            Status = StatusCodes.Status404NotFound;
            Detail = detail;
        }
    }

    public class ResponseType500Dto : ResponseTypeDto
    {
        public ResponseType500Dto(string? detail = null)
        {
            Title = "Internal Server Error";
            Status = StatusCodes.Status500InternalServerError;
            Detail = detail;
        }
    }
}
