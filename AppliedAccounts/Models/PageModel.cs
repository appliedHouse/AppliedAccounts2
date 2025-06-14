using AppliedAccounts.Data;

namespace AppliedAccounts.Models
{
    public class PageModel
    {
        public int TotalRecords { get; set; } = 1;
        public int Current { get; set; } = 1;
        public int Size { get; set; } = 12; // Items per page
        public int Count  => (int)Math.Round(TotalRecords / (double)Size);
        public int MaxButtons { get; set; } = 10; // Number of page buttons to show in Pagination tags
        public List<int> PageList { get; set; } = new();
        public void ChangePage(int page)
        {
            if (page > 0 && page <= Count)
            {
                Current = page;
            }

            PageList.Clear();

            if(Current <= MaxButtons)
            {
                for (int i = 1; i <= Count; i++)
                {
                    if(i<=MaxButtons)
                    { PageList.Add(i); }
                }


                //PageList.Add(1);
                //PageList.Add(2);
                //PageList.Add(3);
                //PageList.Add(4);
                //PageList.Add(5);
                //PageList.Add(6);
                //PageList.Add(7);
                //PageList.Add(8);
                //PageList.Add(9);
                //PageList.Add(10);
            }
            else
            {
                PageList.Add(Current-9);
                PageList.Add(Current-8);
                PageList.Add(Current-7);
                PageList.Add(Current-6);
                PageList.Add(Current-5);
                PageList.Add(Current-4);
                PageList.Add(Current-3);
                PageList.Add(Current-2);
                PageList.Add(Current-1);
                PageList.Add(Current);
            }
        }

        public void Refresh(int _TotalRecords)
        {
            TotalRecords = _TotalRecords;
            ChangePage(Current);
        }
    }
}
