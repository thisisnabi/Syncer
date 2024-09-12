
using Syncer.APIs.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(configure => {

    configure.AddPolicy("localhost", configurePolicy =>
    {
        configurePolicy.WithOrigins("https://localhost:7086")
                       .AllowAnyHeader()
                       .AllowAnyMethod()
                       .AllowCredentials();
    });
});

builder.Services.AddSignalR();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMemoryCache();

builder.Services.AddHostedService<MemoryCacheSetup>();
builder.Services.AddDbContext<SyncerDbContext>((sp, configure) =>
{
    var connectionString = sp.GetRequiredService<IConfiguration>()
                             .GetConnectionString(SyncerDbContext.ConnectionStringName);

    configure.UseSqlServer(connectionString);
});

builder.Services.AddSingleton<PresentationService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("localhost");

app.UseHttpsRedirection();
app.MapEmojiEndpoints();
app.MapPresentationEndpoints();

app.MapHub<BoardHub>("/board");

app.Run();