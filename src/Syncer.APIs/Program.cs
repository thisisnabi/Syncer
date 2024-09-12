

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<SyncerDbContext>((sp, configure) =>
{
    var connectionString = sp.GetRequiredService<IConfiguration>()
                             .GetConnectionString(SyncerDbContext.ConnectionStringName);

    configure.UseSqlServer(connectionString);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapEmojiEndpoints();

app.Run();
