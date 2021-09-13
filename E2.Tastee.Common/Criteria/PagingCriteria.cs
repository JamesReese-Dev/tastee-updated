namespace E2.Tastee.Common
{
    public class PagingCriteria
    {
        public PagingCriteria()
        {
            MaxResults = AppConstants.UNBOUNDED_RESULT_ROW_COUNT;
            PageNumber = 1;
        }
        public int MaxResults { get; set; }
        public int PageNumber { get; set; }
    }
}
