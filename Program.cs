using Aos_1.Controller;
using Aos_1.Extensions;
using Aos_1.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSwaggerExplorer()
                .InjectDbContext(builder.Configuration)
                .AddAppConfig(builder.Configuration)
                .AddIdentityHandlerandStore()
                .ConfigureIdentityOptions()
                .AddIdentityAuth(builder.Configuration);

var app = builder.Build();

app.ConfigureSwaggerExplorer()
    .ConfigureCore(builder.Configuration)
    .AddIdentityAuthMiddlewares();

app.MapControllers();
app.MapGroup("/api").MapIdentityApi<AppUser>();
app.MapGroup("/api")
    .MapIdentityUserEndpoints()
    .MapAccountEndpoints()
    .MapAuthorizationDemoEndpoints();

app.Run();


