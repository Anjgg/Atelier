namespace Atelier.Api._Exception
{
    public class NoDataException : Exception
    {
        public int StatusCode { get; }
        public string Detail { get; }

        public NoDataException(string detail, int statusCode = 404)
            : base(detail)
        {
            Detail = detail;
            StatusCode = statusCode;
        }
    }
}
