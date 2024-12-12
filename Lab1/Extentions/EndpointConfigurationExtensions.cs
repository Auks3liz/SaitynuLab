using Lab1.EndPoints;

namespace Lab1.Auth;

public static class EndpointConfigurationExtensions
{
    public static WebApplication ConfigureApiEndpoints(this WebApplication app)
    {
        app.AddMealApi();
        app.AddRecipieApi();
        app.AddCommentApi();
        app.AddAuthApi();

        return app;
    }
}