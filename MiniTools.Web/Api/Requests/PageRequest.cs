using System.ComponentModel.DataAnnotations;

namespace MiniTools.Web.Api.Requests;

public class PageRequest
{
    private ushort pageNumber = 1;
    private ushort pageSize = 25;

    [Required]
    [Range(1, ushort.MaxValue)]
    public ushort PageNumber
    {
        get
        {
            return pageNumber;
        }
        set
        {
            pageNumber = (value <= 0) ? (ushort)1 : value;
        }
    }

    [Required]
    [Range(1, ushort.MaxValue)]
    public ushort PageSize
    {
        get
        {
            return pageSize;
        }
        set
        {
            pageSize = (value <= 0) ? (ushort)1 : value;
        }
    }

    public uint MinRecord
    {
        get
        {
            return (((uint)pageNumber - 1) * (uint)pageSize) + 1;
        }
    }

    public uint MaxRecord
    {
        get
        {
            return (uint)pageNumber * (uint)pageSize;
        }
    }

    public PageRequest()
    {
        this.pageNumber = 1;
        this.pageSize = 25;

        //  1 - 25  -- 1 * 25 = 25; 1-1 = 0 * 25
        // 26 - 50  -- 2 * 25 = 50; 2-1 = 1 
        // 51 - 75  -- 3 * 25 = 75; 3-1 = 2
        //rangeMin = (((uint)pageNumber - 1) * (uint)pageSize) + 1;
        //rangeMax = (uint)pageNumber * (uint)pageSize;
    }
}
