using System.ComponentModel.DataAnnotations;

namespace MiniTools.Web.Models;

public class AddBookmarkViewModel
{
    [Display(Name ="Bookmark Links")]
    public string BookmarkLinks { get; set; } = string.Empty;
}
