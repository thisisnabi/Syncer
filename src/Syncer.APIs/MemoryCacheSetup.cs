using Microsoft.Extensions.Caching.Memory;

namespace Syncer.APIs;
 
public class MemoryCacheSetup(IServiceScopeFactory serviceScopeFactory) : IHostedService
{
    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var scoped = _serviceScopeFactory.CreateScope();
        var dbContext = scoped.ServiceProvider.GetRequiredService<SyncerDbContext>();

        var emojies = await dbContext.Emojis.ToListAsync();

        if (emojies.Count == 0)
            return;

        var cache = scoped.ServiceProvider.GetRequiredService<IMemoryCache>();
        foreach (var emoji in emojies)
            cache.Set(emoji.Code, emoji.ShortName);
    }

    public Task StopAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;
}
 