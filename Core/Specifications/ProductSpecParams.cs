namespace Core.Specifications
{
    // Setting up parameters for the Products controller
    public class ProductSpecParams
    {
        private const int MaxPageSize = 50;
        public int PageIndex { get; set; } = 1;
        private int _pageSize = 6;

        // Setting pagination props
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }
        
        // Sorting/ordering props
        public int? BrandId { get; set; }
        public int? TypeId { get; set; }
        public string Sort { get; set; }
        
        // Search props
        private string _search;

        public string Search
        {
            get => _search;
            set => _search = value.ToLower();
        }
    }
}