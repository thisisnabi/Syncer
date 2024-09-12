using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using Syncer.APIs.Hubs;
using Syncer.APIs.Models.Domain;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

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

                return Results.Ok("valid action");
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

                return Results.Ok("valid user");
            }
            catch (Exception)
            {
                return Results.BadRequest();
            }
        });


        group.MapPost("/{presentation_id}/reaction",
        async ([FromRoute(Name = "presentation_id")] string presentationId,
            CreateReactionRequest request, 
            IHubContext<BoardHub> HubCallerContext,
            PresentationService presentationService,
            SyncerDbContext dbContext, IMemoryCache cache) =>
        {
            try
            {
                var presentation = await dbContext.Presentations
                                                  .Include(d => d.Joiners)
                                                  .FirstAsync(x => x.Id == presentationId);

                presentation.Act(request.code, request.username);
                var hasChanges = await dbContext.SaveChangesAsync() > 0;
                if (!hasChanges)
                    return Results.Ok("valid user");

                var connectionId = presentationService.GetConnectionIdByPresentationId(presentationId);

                if (string.IsNullOrEmpty(connectionId))
                    return Results.Ok("valid user");

                await HubCallerContext.Clients.Client(connectionId).SendAsync("OnReaction", request.username, request.code);
                return Results.Ok("valid user");
            }
            catch (Exception)
            {
                return Results.BadRequest();
            }
        });




        group.MapGet("/{presentation_id}",
        async ([FromRoute(Name = "presentation_id")] string presentationId, SyncerDbContext dbContext) =>
        {
            try
            {
                var presentation = await dbContext.Presentations
                                                  .Include(x => x.Milestones)
                                                  .FirstAsync(x => x.Id == presentationId);

                return Results.Ok(new
                {
                    Title = presentation.Title,
                    Description = presentation.Description,
                    Milestones = presentation.Milestones.Select(x => new { 
                        Id = x.Id,
                        Title = x.Title,
                        Description = x.Description,
                        Status = Enum.GetName(x.Status),
                    })
                });
            }
            catch (Exception)
            {
                return Results.BadRequest();
            }
        });

        group.MapGet("/{presentation_id}/{milestone_id}/emojis",
async ([FromRoute(Name = "presentation_id")] string presentationId,
       [FromRoute(Name = "milestone_id")] long milestoneId,
SyncerDbContext dbContext) =>
{
    try
    {
        var presentation = await dbContext.Presentations
                                          .Include(x => x.Milestones)
                                          .FirstAsync(x => x.Id == presentationId);
        var milestone = presentation.Milestones.FirstOrDefault(x => x.Id == milestoneId);
        return Results.Ok(milestone!.Emojis.ToList());
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
public record CreateReactionRequest(string code, string username);

