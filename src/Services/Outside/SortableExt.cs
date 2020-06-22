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
                sortStr = DoColumnMapping(sortStr, DEFAULT_MAPPING);
                sortStr = DoColumnMapping(sortStr, mapping);
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
                sortStr = DoColumnMapping(sortStr, DEFAULT_MAPPING);
                sortStr = DoColumnMapping(sortStr, mapping); 
                queryable = queryable.OrderBy(sortStr);
            }
            return queryable;
        }

        private static string DoColumnMapping(string value,string[,] mapping)
        {
            if (mapping == null) return value;
            for (int i = 0; i < mapping.GetLength(0); i++)
            {
                string before = mapping[i, 0];
                string after = mapping[i, 1];
                if (value.StartsWith(before + " "))
                {
                    value = value.Replace(before + " ", after + " ");
                }
            }
            return value;
        }

    }
}
