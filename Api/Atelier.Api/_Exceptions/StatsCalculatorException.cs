namespace Atelier.Api._Exceptions
{
    public class StatsCalculatorException : Exception
    {
        public int StatusCode { get; }
        public string Detail { get; }

        public StatsCalculatorException(string detail, int statusCode = 500)
            : base(detail)
        {
            Detail = detail;
            StatusCode = statusCode;
        }
    }
}
