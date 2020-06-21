using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Outside
{
    public static class SortableExt
    {

        public static ISugarQueryable<T> Sort<T>(this ISugarQueryable<T> queryable,string[] sorts)
        {
            if (sorts == null) return queryable;
            foreach (string sort in sorts)
            {
                queryable = queryable.OrderBy(sort);
            }
            return queryable;
        }

        public static ISugarQueryable<T1, T2> Sort<T1,T2>(this ISugarQueryable<T1, T2> queryable, string[] sorts)
        {
            if (sorts == null) return queryable;
            foreach (string sort in sorts)
            {
                var sortStr = sort.ToUpper().Replace("ROW ", "ROWP ").Replace("COLUMN ", "COLUMNP ").Replace("DESC ", "DESCP ");
                queryable = queryable.OrderBy(sortStr);
            }
            return queryable;
        }
    }
}
