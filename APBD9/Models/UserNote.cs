namespace APBD9.Models;


public class UserNote
{
    public int Id { get; set; }
    public int AppUserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public AppUser? AppUser { get; set; }
}