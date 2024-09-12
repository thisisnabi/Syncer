namespace Syncer.APIs.Models.Domain;

public class Milestone
{
    public long Id { get; set; }

    public string PresentationId { get; set; }

    public MilestoneStatus Status { get; set; }

    public required string Title { get; set; }

    public required string Description { get; set; }

    public ICollection<MilestoneEmoji> Emojis { get; set; }

    public ICollection<Reaction> Reactions { get; set; }

    internal static Milestone Create(string title, string description, List<string> allowedEmojies)
    {
        return new Milestone
        {
            Description = description,
            Title = title,
            Emojis = allowedEmojies.Select(d => new MilestoneEmoji(d)).ToList()
        };
    }
}

public record MilestoneEmoji(string Code);

public enum MilestoneStatus
{
    InQueue = 1,
    Processing = 2,
    Done = 3
}

