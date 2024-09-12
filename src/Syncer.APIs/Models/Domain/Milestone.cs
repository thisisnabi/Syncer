using System.ComponentModel.DataAnnotations;

namespace Syncer.APIs.Models.Domain;

public class Milestone
{
    public long Id { get; set; }

    public required string PresentationId { get; set; }

    public MilestoneStatus Status { get; set; }

    public required string Title { get; set; }

    public required string Description { get; set; }

    public ICollection<MilestoneEmoji> Emojis { get; set; }

    public ICollection<Reaction> Reactions { get; set; }
}

public record MilestoneEmoji(string Code, string ShortName);

public enum MilestoneStatus
{ 
    InQueue = 1,
    Processing = 2,
    Done = 3
}

