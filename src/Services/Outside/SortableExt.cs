using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Outside
{
    public static class SortableExt
    {

        private static string[,] DEFAULT_MAPPING = new string[,] {
            { "ROW", "ROWP" },
            { "COLUMN", "COLUMNP" },
            { "FLOOR", "FLOORP" },
            { "SIZE", "SIZEP" },
            { "DESC", "DESCP" },
        };

        public static ISugarQueryable<T> Sort<T>(this ISugarQueryable<T> queryable,string[] sorts,string[,] mapping = null)
        {
            if (sorts == null) return queryable;
            foreach (string sort in sorts)
            {
                var sortStr = sort.ToUpper();
                for(int i =0;i< DEFAULT_MAPPING.GetLength(0); i++)
                {
                    sortStr = sortStr.Replace(DEFAULT_MAPPING[i,0] + " ", DEFAULT_MAPPING[i,1] + " ");
                }
                if (mapping != null)
                {
                    for (int i = 0; i < mapping.GetLength(0); i++)
                    {
                        sortStr = sortStr.Replace(mapping[i, 0] + " ", mapping[i, 1] + " ");
                    }
                }
                queryable = queryable.OrderBy(sortStr);
            }
            return queryable;
        }

        public static ISugarQueryable<T1, T2> Sort<T1,T2>(this ISugarQueryable<T1, T2> queryable, string[] sorts = null, string[,] mapping = null)
        {
            if (sorts == null) return queryable;
            foreach (string sort in sorts)
            {
                var sortStr = sort.ToUpper();
                for (int i = 0; i < DEFAULT_MAPPING.GetLength(0); i++)
                {
                    sortStr = sortStr.Replace(DEFAULT_MAPPING[i, 0] + " ", DEFAULT_MAPPING[i, 1] + " ");
                }
                if (mapping != null)
                {
                    for (int i = 0; i < mapping.GetLength(0); i++)
                    {
                        sortStr = sortStr.Replace(mapping[i, 0] + " ", mapping[i, 1] + " ");
                    }
                }
                queryable = queryable.OrderBy(sortStr);
            }
            return queryable;
        }

    }
}
