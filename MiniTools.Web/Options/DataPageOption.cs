using System.ComponentModel.DataAnnotations;

namespace MiniTools.Web.Options
{
    public enum SortDirection
    {
        Ascending,
        Descending
    }

    public class SortItem
    {
        public SortDirection SortDirection { get; set; } = SortDirection.Ascending;

        [Required]
        public string FieldName { get; set; } = string.Empty;
    }

    public class DataPageOption
    {
        [Required]
        public int Page { get; set; } = 1;
        
        public int PageSize { get; set; } = 1;

        public IEnumerable<SortItem> SortItems { get; set;}

        public DataPageOption()
        {
            this.SortItems = new List<SortItem>();
        }

        public DataPageOption(int pageSize) : this()
        {
            this.PageSize = pageSize;
        }
    }
}
