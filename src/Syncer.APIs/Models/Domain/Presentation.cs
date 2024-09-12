
namespace Syncer.APIs.Models.Domain;

public class Presentation
{
    public required string Id { get; set; }

    public required string Title { get; set; }

    public required string Description { get; set; }

    public required string Speaker { get; set; }

    public PresentationStatus Status { get; set; }

    public static Presentation Create
        (string id, string title, string description, string speaker)
        => new Presentation
        {
            Description = description,
            Id = id,
            Title = title,
            Speaker = speaker,
            Status = PresentationStatus.Created
        };

    internal void AddMilestone(Milestone milestone)
    {
        if (Status != PresentationStatus.Created)
            throw new Exception("invalid presentation status!");

        Milestones ??= [];
        Milestones.Add(milestone);
    }



    internal void AddJoiner(PresentationJoiner joiner)
    {
        if (Status != PresentationStatus.Present)
            throw new Exception("invalid presentation status!");

        Joiners ??= [];
        Joiners.Add(joiner);
    }

    internal void StartPresent()
    {
        if (Status != PresentationStatus.Created)
            throw new Exception("invalid presentation status!");

        Status = PresentationStatus.Present;
    }

    internal void Act(string code, string userid)
    {
        var activeMilestone = Milestones.First(x => x.Status == MilestoneStatus.Processing);

        if (!activeMilestone.Emojis.Any(d => d.Code == code))
            throw new InvalidOperationException();

        if (activeMilestone.Reactions.Any(d => d.Username == userid))
            return;

        activeMilestone.Reactions.Add(new Reaction
        {
            Username = userid,
            EmojiCode = code
        });
    }

    public ICollection<PresentationJoiner> Joiners { get; set; } = null!;

    public ICollection<Milestone> Milestones { get; set; } = null!;
}


public enum PresentationStatus
{
    Created = 1,
    Present = 2,
    Finished = 3
}


public record PresentationJoiner(string Username);