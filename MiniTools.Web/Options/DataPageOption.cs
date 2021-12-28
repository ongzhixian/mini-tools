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
        public ushort Page { get; set; } = 1;
        
        public ushort PageSize { get; set; } = 1;

        public IEnumerable<SortItem> SortItems { get; set;}

        public DataPageOption()
        {
            this.SortItems = new List<SortItem>();
        }

        public DataPageOption(ushort pageSize) : this()
        {
            this.PageSize = pageSize;
        }
    }
}
