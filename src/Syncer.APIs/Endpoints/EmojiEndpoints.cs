using Syncer.APIs.Models.Domain;

namespace Syncer.APIs.Endpoints;

public static class EmojiEndpoints
{
    public static void MapEmojiEndpoints(this IEndpointRouteBuilder endpoint)
    {
        var group = endpoint.MapGroup("emoji")
                            .WithTags("Emoji");

        group.MapPost("/", async (string code, string name, SyncerDbContext dbContext) =>
        { 
            try
            {
                dbContext.Emojis.Add(Emoji.Create(code, name));
                await dbContext.SaveChangesAsync();
                return Results.Ok();
            }
            catch (Exception)
            {
                return Results.BadRequest();
            }
        });
    }
}
