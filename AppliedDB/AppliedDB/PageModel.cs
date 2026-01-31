namespace AppliedDB
{
    public class PageModel
    {
        public int TotalRecords { get; set; } = 0;
        public int Current { get; private set; } = 1;
        public int Size { get; set; } = 12;
        public int MaxButtons { get; set; } = 10;

        public int Count => Math.Max(1, (int)Math.Ceiling(TotalRecords / (double)Size));

        public List<int> PageList { get; private set; } = new();

        public event Action<int>? PageChanged;


        public void ChangePage(int page)
        {
            int newPage = Math.Clamp(page, 1, Count);

            if (newPage == Current) { return; }

            Current = newPage;
            BuildPageList();

            PageChanged?.Invoke(Current);
        }

        public void Refresh(int totalRecords)
        {
            TotalRecords = totalRecords;
            BuildPageList();
            ChangePage(Current);
        }

        private void BuildPageList()
        {
            PageList.Clear();

            int half = MaxButtons / 2;
            int start = Math.Max(1, Current - half);
            int end = Math.Min(Count, start + MaxButtons - 1);
            start = Math.Max(1, end - MaxButtons + 1);

            for (int i = start; i <= end; i++)
                PageList.Add(i);
        }
    }
}
