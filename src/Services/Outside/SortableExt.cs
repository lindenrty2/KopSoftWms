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
            foreach (string sort in sorts)
            {
                queryable = queryable.OrderBy(sort);
            }
            return queryable;
        }

    }
}
