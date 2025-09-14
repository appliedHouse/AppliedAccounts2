

using System.Collections.Specialized;
using System.Data;
using System.Text;
using static AppliedDB.Enums;

namespace AppliedDB
{
    public class PageQuery
    {
        public Tables Table { get; set; } = Tables.Nothing;
        public string Query { get; set; }
        public string Filter { get; set; }
        public string Sort { get; set; }
        public PageModel Pages { get; set; }
        public DataSource Source { get; set; }

        public List<DataRow> FilterList { get; set; }

        public async Task<DataTable> GetPageData()
        {
            var _Text = await GetCommandText();
            var _Table = Source.GetTable(_Text.ToString());
            //Pages.TotalRecords = _Table?.Rows.Count ?? 0;
            Pages.GetPageList();
            return await Task.FromResult(_Table ?? new());
        }

        public async Task<string> GetCommandText()
        {
            var _Text = new StringBuilder();
            if (Table != Tables.Nothing)
            {
                _Text.Append($"SELECT * FROM {Table} ");
            }
            else
            {
                if (Query != null) { _Text.Append(Query); }
            }
            if (_Text.Length == 0)
            {
                _Text.Append("Error... no Table Name or Query found.");
                return _Text.ToString();
            }
            if (Filter != null) { _Text.Append(Filter); }
            if (Sort != null) { _Text.Append(Sort); }
            if (Pages != null)
            {
                _Text.Append($"LIMIT {Pages.Size} OFFSET {(Pages.Current - 1) * Pages.Size}");
            }
            return await Task.FromResult(_Text.ToString());
        }


    }
}
