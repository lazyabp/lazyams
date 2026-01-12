using Microsoft.OpenApi;
using System.Reflection;
using WebApi.Filters;

namespace WebApi.Init;


public static class SwaggerExtension
{
    /// <summary>
    /// add swagger to services
    /// </summary>
    /// <param name="services"></param>
    public static void AddSwaggerLazy(this IServiceCollection services)
    {
        //add swagger
        services.AddSwaggerGen(options =>
        {
            typeof(SwaggerGroup).GetEnumNames().ToList().ForEach(version =>
            {
                options.SwaggerDoc(version, new OpenApiInfo()
                {
                    Title = "Lazy Web Api",
                    Version = "V1.0",
                    Description = "Lazy WebApi The backend server provides data",
                    Contact = new OpenApiContact { Name = "Lazy Team", Url = new Uri("http://www.google.com") }
                });
                options.OperationFilter<FileUploadOperationFilter>();
            });

            //Reflection acquisition interface and method description
            var xmlFileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFileName), true);

            //use jwt
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "Please enter Bearer Token in the input box below to enable JWT authentication",
                Name = "Authorization", // Default name, cannot be modified
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            //Make Swagger comply with the JWT protocol
            options.AddSecurityRequirement((doc) => new OpenApiSecurityRequirement
            {
                {
                    // 传入 referenceId
                    new OpenApiSecuritySchemeReference("Bearer"),
                    new List<string>()
                }
            });
        });
    }


    /// <summary>
    ///  Join routing and pipeline
    /// </summary>
    /// <param name="app"></param>
    public static void UseSwaggerLazy(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            typeof(SwaggerGroup).GetEnumNames().ToList().ForEach(versoin =>
            {
                options.SwaggerEndpoint($"/swagger/{versoin}/swagger.json", $" {versoin}");
            });
        });
    }
}