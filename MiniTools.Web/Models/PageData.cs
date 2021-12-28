namespace MiniTools.Web.Models;


public interface IPageData
{
    ushort Page { get; set; }
    ushort PageSize { get; set; }
    ulong TotalRecordCount { get; set; }
    ulong TotalPages { get; }

    ulong PageRecordCount { get; }
    
}

public class PageData<T> : IPageData where T : class
{
    public ushort Page { get; set; }

    public ushort PageSize { get; set; }

    public ulong TotalRecordCount { get; set; }

    public IEnumerable<T> DataList { get; set; }

    public ulong TotalPages
    {
        get
        {
            //ulong fullPagesCount = RecordCount / PageSize;
            //ulong partialPageCount = (ulong)((RecordCount % PageSize > 0) ? 1 : 0);
            //return fullPagesCount + partialPageCount;
            // succinct version
            return (TotalRecordCount / PageSize) + (ulong)((TotalRecordCount % PageSize > 0) ? 1 : 0);
        }
    }

    public ulong PageRecordCount
    {
        get
        {
            return (ulong)DataList.Count();
        }
    }
}

//public class PageDataViewModel<T> where T : class
//{
//    public ushort Page { get; set; }

//    public ushort PageSize { get; set; }

//    public IEnumerable<T> DataList { get; set; }

//    public PageDataViewModel(ushort page, ushort pageSize, IEnumerable<T> dataList)
//    {
//        this.Page = page;
//        this.PageSize = pageSize;
//        this.DataList = dataList;
//    }
//}
