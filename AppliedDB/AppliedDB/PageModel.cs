

namespace AppliedDB
{
    public class PageModel
    {
        public int TotalRecords { get; set; } = 1;
        public int Current { get; set; } = 1;
        public int Size { get; set; } = 12; // Items per page
        public int Count  => (int)Math.Round(TotalRecords / (double)Size);
        public int MaxButtons { get; set; } = 10; // Number of page buttons to show in Pagination tags
        public List<int> PageList { get; set; } = [];   // List of Pages 1,2,3,4,5........n


        #region Text
        //public void ChangePage1(int page)
        //{
        //    if (page > 0 && page <= Count)
        //    {
        //        Current = page;
        //    }

        //    PageList.Clear();

        //    if(Current <= MaxButtons)
        //    {
        //        for (int i = 1; i <= Count; i++)
        //        {
        //            if(i<=MaxButtons)
        //            { PageList.Add(i); }
        //        }
        //    }
        //    else
        //    {
        //        PageList.Add(Current-9);
        //        PageList.Add(Current-8);
        //        PageList.Add(Current-7);
        //        PageList.Add(Current-6);
        //        PageList.Add(Current-5);
        //        PageList.Add(Current-4);
        //        PageList.Add(Current-3);
        //        PageList.Add(Current-2);
        //        PageList.Add(Current-1);
        //        PageList.Add(Current);
        //    }
        //}
        #endregion

        public void ChangePage(int page)
        {
            // Ensure Count is at least 1 to avoid division by zero
            int pageCount = Math.Max(Count, 1);

            // Clamp the requested page between 1 and pageCount
            if (page < 1)
                Current = 1;
            else if (page > pageCount)
                Current = pageCount;
            else
                Current = page;

            PageList.Clear();

            if (Current <= MaxButtons)
            {
                for (int i = 1; i <= pageCount && i <= MaxButtons; i++)
                {
                    PageList.Add(i);
                }
            }
            else
            {
                for (int i = Current - MaxButtons + 1; i <= Current; i++)
                {
                    if (i > 0 && i <= pageCount)
                        PageList.Add(i);
                }
            }
        }

        public void Refresh(int _TotalRecords)
        {
            TotalRecords = _TotalRecords;
            ChangePage(Current);
        }

        public void GetPageList()
        {
            

            PageList = [];

            if(Size <= Count)
            {
                // Select full length of pages if Size is less than Count
                PageList.AddRange(Enumerable.Range(1, Size));
            }
            else
            {
                // Select full length of Count if Size is greater than Count
                PageList.AddRange(Enumerable.Range(1, Count));
            }

               
        }
    }
}
