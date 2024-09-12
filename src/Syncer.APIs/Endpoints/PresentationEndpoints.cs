using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Syncer.APIs.Models.Domain;

namespace Syncer.APIs.Endpoints;

public static class PresentationEndpoints
{
    public static void MapPresentationEndpoints(this IEndpointRouteBuilder endpoint)
    {
        var group = endpoint.MapGroup("presentation")
                            .WithTags("Presentation");

        group.MapPost("/{speaker}",
            async (CreatePresentationRequest request, string speaker, SyncerDbContext dbContext) =>
        {
            try
            {
                var presentation = Presentation
                    .Create(request.UnifiedId, request.Title, request.Description, speaker);

                dbContext.Presentations.Add(presentation);
                await dbContext.SaveChangesAsync();
                return Results.Ok();
            }
            catch (Exception)
            {
                return Results.BadRequest();
            }
        });

        group.MapPost("/{presentation_id}/milestone",
                async ([FromRoute(Name = "presentation_id")] string presentationId,
        CreateMilestoneRequest request, SyncerDbContext dbContext, IMemoryCache cache) =>
        {
            try
            {
                var presentation = await dbContext.Presentations
                                                  .Include(z => z.Milestones)
                                                  .FirstAsync(x => x.Id == presentationId);

                if (!request.AllowedEmojis.All(d => cache.TryGetValue(d, out _)))
                    return Results.BadRequest("invalid emojies!");

                var milestone = Milestone.Create(request.Title, request.Description, request.AllowedEmojis);

                presentation.AddMilestone(milestone);


                await dbContext.SaveChangesAsync();

                return Results.Ok();
            }
            catch (Exception)
            {
                return Results.BadRequest();
            }
        });


        group.MapPut("/{presentation_id}/present",
        async ([FromRoute(Name = "presentation_id")] string presentationId, SyncerDbContext dbContext, IMemoryCache cache) =>
        {
            try
            {
                var presentation = await dbContext.Presentations
                                                  .FirstAsync(x => x.Id == presentationId);

                presentation.StartPresent();
                await dbContext.SaveChangesAsync();

                return Results.Ok();
            }
            catch (Exception)
            {
                return Results.BadRequest();
            }
        });


        group.MapPost("/{presentation_id}/join",
        async ([FromRoute(Name = "presentation_id")] string presentationId,
            CreateJoinRequest request, SyncerDbContext dbContext, IMemoryCache cache) =>
        {
            try
            {
                var presentation = await dbContext.Presentations
                                                  .Include(d => d.Joiners)
                                                  .FirstAsync(x => x.Id == presentationId);

                presentation.AddJoiner(new PresentationJoiner(request.joinerId));
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

public record CreatePresentationRequest
    (string UnifiedId, string Title, string Description);

public record CreateMilestoneRequest(string Title, string Description, List<string> AllowedEmojis);
public record CreateJoinRequest(string joinerId);

