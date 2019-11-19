namespace YL.Utils.Table
{
    public class Bootstrap
    {
        /// <summary>
        /// bootstrap-table search=&sort=CreateDate&order=desc&offset=0&limit=10&_=1537247998287
        /// </summary>
        public class BootstrapParams
        {
            /// <summary>
            /// 10*(2-1)
            /// 页码*页面显示行数=offset
            /// </summary>
            public int offset { get; set; } = 0;

            /// <summary>
            /// 页面显示行数
            /// </summary>
            public int limit { get; set; }
            /// <summary>
            /// 页码
            /// </summary>
            public int pageIndex
            {
                get
                {
                    if (offset != 0)
                    {
                        return offset / limit + 1;
                    }
                    return 1;
                }
            }

            /// <summary>
            /// 排序字段
            /// </summary>
            public string sort { get; set; } = "";

            /// <summary>
            /// 排序方式
            /// </summary>
            public string order { get; set; } = "";

            public string search { get; set; } = "";

            public string datemin { get; set; } = "";
            public string datemax { get; set; } = "";
            public string keyword { get; set; } = "";

            public long storeId { get; set; }
        }

        public static PageGridData GridData(object data, int total)
        {
            return new PageGridData(data, total);
        }
    }

    public class PageGridData
    {
        public int total { get; set; }
        public object rows { get; set; }

        public PageGridData()
        {
            rows = new object[0];
            total = 0;
        }

        public PageGridData(object r, int t)
        {
            total = t;
            rows = r;
        }
    }
}