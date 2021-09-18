using System.Collections.Generic;

namespace API.Helpers
{
    // Creating a generic type for pagination
    public class Pagination<T> where T : class
    {
        // Page number in sequence
        public Pagination(int pageIndex, int pageSize, int count, IReadOnlyList<T> data)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            Count = count;
            Data = data;
        }

        public int PageIndex { get; set; }
        
        // Quantity of items per page
        public int PageSize { get; set; }
        
        // Counts the # of items after the filter has been applied
        public int Count { get; set; }
        
        // Prop that contains data (page results)
        public IReadOnlyList<T> Data { get; set; }
    }
}