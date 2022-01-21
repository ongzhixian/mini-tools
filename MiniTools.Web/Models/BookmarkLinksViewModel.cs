using MiniTools.Web.MongoEntities;

namespace MiniTools.Web.Models;

public class BookmarkLinksViewModel
{
    public long DocumentCount { get; set; }
    public List<Bookmark>? BookmarkList { get; set; }
}
