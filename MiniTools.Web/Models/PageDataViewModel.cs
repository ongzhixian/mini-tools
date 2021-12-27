using System.ComponentModel.DataAnnotations;

namespace MiniTools.Web.Models
{
    public class PageDataViewModel<T> where T : class
    {
        public ushort Page { get; set; }
        
        public ushort PageSize { get; set;  }

        public IEnumerable<T> DataList { get; set; }

        public PageDataViewModel(ushort page, ushort pageSize, IEnumerable<T> dataList)
        {
            this.Page = page;
            this.PageSize = pageSize;
            this.DataList = dataList;
        }
    }
}
