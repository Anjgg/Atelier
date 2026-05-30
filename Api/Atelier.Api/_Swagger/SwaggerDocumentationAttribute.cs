namespace Atelier.Api._Swagger
{
    [AttributeUsage(AttributeTargets.Method)]
    public class SwaggerDocumentationAttribute : Attribute
    {
        public string Summary { get; }
        public string Description { get; }
        public SwaggerDocumentationAttribute(string summary, string description)
        {
            Summary = summary;
            Description = description;
        }
    }
}
