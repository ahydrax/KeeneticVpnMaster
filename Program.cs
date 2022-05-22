var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();
builder.Configuration.AddJsonFile(args[1]);

var configuration = builder.Configuration.Get<Configuration>();

builder.Services.AddSingleton(configuration);
builder.Services.AddControllers();
builder.Services.Configure<RouteOptions>(options => options.ConstraintMap.Add("mac", typeof(MacRouteConstraint)));

var app = builder.Build();

app.MapControllers();

app.Run();
