using WebApplication1;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseWindowsService();

// Add services to the container.

builder.Host.ConfigureServices(services =>
{
    //services.Configure<CacheOptions>(builder.Configuration.GetSection("Cache"));
    services.AddSingleton(builder.Configuration.GetSection("Cache").Get<CacheOptions>());

    services.AddSignalR();
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapHub<SampleHub>("/hubs/sample");

app.Run();
