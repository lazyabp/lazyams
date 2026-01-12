using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WebApi.Filters;

public class FileUploadOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var formParameters = context.MethodInfo.GetParameters()
            .Where(p =>
                p.ParameterType == typeof(IFormFile) ||
                p.CustomAttributes.Any(attr => attr.AttributeType == typeof(FromFormAttribute)));

        if (formParameters.Any())
        {
            // 在新版 OpenAPI 库中，推荐使用这种方式构建 Schema
            var props = new Dictionary<string, IOpenApiSchema>();

            foreach (var p in formParameters)
            {
                props.Add(p.Name ?? "file", new OpenApiSchema
                {
                    Type = JsonSchemaType.String, // 注意此处枚举的使用
                    Format = "binary"
                });
            }

            operation.RequestBody = new OpenApiRequestBody
            {
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["multipart/form-data"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = JsonSchemaType.Object,
                            Properties = props
                        }
                    }
                }
            };
        }
    }

}
