namespace APBD9.VModels;
using System.ComponentModel.DataAnnotations;


public class NoteViewModel
{
    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;
}