var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
DotNetEnv.Env.Load();
builder.Configuration
.AddJsonFile("appSettings.json", optional: true, reloadOnChange: true)
.AddJsonFile($"appSettings.{builder.Environment}.json", optional: true, reloadOnChange: true)
//.AddUserSecrets(Assembly.GetExecutingAssembly())
.AddEnvironmentVariables();


var app = builder.Build();



app.UseHttpsRedirection();



app.MapGet("/testLogin", () =>
{
    return app.Configuration.GetSection("AppSettings").GetValue<string>("login");

})
.WithName("TestLogin");

app.Run();


