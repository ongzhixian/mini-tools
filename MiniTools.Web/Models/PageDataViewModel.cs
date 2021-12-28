using System.ComponentModel.DataAnnotations;

namespace MiniTools.Web.Models;

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

public class PageDataViewModel<T> : PageData<T> where T : class
{
    public PageDataViewModel(PageData<T> pageData)
    {
        Page = pageData.Page;
        PageSize = pageData.PageSize;
        DataList = pageData.DataList;
        TotalRecordCount = pageData.TotalRecordCount;
    }
}

public class PaginatorViewModel
{
    public readonly ulong CurrentPage;      // Data from model
    public readonly ulong TotalPages;       // Data from model
    public readonly ulong PageRecordCount;  // Data from model
    public readonly ulong TotalRecordCount; // Data from model

    readonly ulong PageWindowSize;          // User parameter

    public readonly string AreaName;
    public readonly string ControllerName;
    public readonly string ActionName;

    // Calculated fields

    readonly ulong windowMid;// = (pageWindow / 2);
    readonly ulong pageCenter;// = (pageWindow / 2) + 1;

    public ulong pagerEnd;
    public ulong pagerStart;
    public ulong prevPage;
    public ulong nextPage;

    public ulong recordStart;
    public ulong recordEnd;




    public PaginatorViewModel(
        ulong pagerWindowSize,
        ulong currentPage, ulong totalPages, ulong pageRecordCount, ulong pageSize, ulong totalRecordCount,
        string areaName, string controllerName, string actionName
        )
    {
        PageWindowSize = pagerWindowSize;
        CurrentPage = currentPage;
        TotalPages = totalPages;
        PageRecordCount = pageRecordCount;
        TotalRecordCount = totalRecordCount;

        recordStart = ((currentPage - 1) * pageSize) + 1;
        recordEnd = ((currentPage - 1) * pageSize) + pageRecordCount;

        AreaName = areaName;
        ControllerName = controllerName;
        ActionName = actionName;

        windowMid = (PageWindowSize / 2);
        pageCenter = (PageWindowSize / 2) + 1;

        CalculatePagerEnd();
        CalculatePagerStart();
        CalculatePrev();
        CalculateNext();
    }

    public PaginatorViewModel(IPageData model, ulong pagerWindowSize, string areaName, string controllerName, string actionName)
        : this(
              pagerWindowSize,
              model.Page, model.TotalPages, model.PageRecordCount, model.PageSize, model.TotalRecordCount,
              areaName, controllerName, actionName) {}

    public PaginatorViewModel(IPageData model, ulong pagerWindowSize, string controllerName, string actionName)
        : this(
              pagerWindowSize,
              model.Page, model.TotalPages, model.PageRecordCount, model.PageSize, model.TotalRecordCount,
              string.Empty, controllerName, actionName) {}

    private void CalculatePagerEnd()
    {
        if (CurrentPage <= pageCenter)
            pagerEnd = PageWindowSize;
        else
            pagerEnd = CurrentPage + windowMid;

        if (pagerEnd > TotalPages)
            pagerEnd = TotalPages;
    }

    private void CalculatePagerStart()
    {
        if (pagerEnd >= PageWindowSize)
            pagerStart = (pagerEnd - PageWindowSize) + 1;
        else
            pagerStart = 1;
    }

    private void CalculatePrev()
    {
        if (pagerStart > windowMid)
            prevPage = pagerStart - windowMid;
        else
            prevPage = 1;
    }

    private void CalculateNext()
    {
        nextPage = pagerEnd + windowMid;

        if (nextPage > TotalPages)
            nextPage = TotalPages;
    }
}