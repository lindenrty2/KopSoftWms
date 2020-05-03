using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace YL.Utils.Check
{
    public static class Guard
    {

        public static string GuardEmpty(Expression<Func<string>> expr)
        {
            string value = expr.Compile().Invoke();
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new WMSException(Pub.PubMessages.E0008_PARAMETER_EMPTY, "'" + expr.Body.ToString() + "'不能为空");
            }
            return value;
        }

        public static string GuardNothing(Expression<Func<string>> expr)
        {
            string value = expr.Compile().Invoke();
            if (value == null)
            {
                throw new WMSException(Pub.PubMessages.E0009_PARAMETER_NULL, "'" + expr.Body.ToString() + "'不能为Null");
            }
            return value;
        }

        public static int GuardInteger(Expression<Func<string>> expr)
        {
            string value = expr.Compile().Invoke();
            int result;
            if (!int.TryParse(value,out result))
            {
                throw new WMSException(Pub.PubMessages.E0010_PARAMETER_TYPE_INVAILD, "'" + expr.Body.ToString() + "'不是Int形");
            }
            return result;
        }

        public static long GuardLong(Expression<Func<string>> expr)
        {
            string value = expr.Compile().Invoke();
            long result;
            if (!long.TryParse(value, out result))
            {
                throw new WMSException(Pub.PubMessages.E0010_PARAMETER_TYPE_INVAILD, "'" + expr.Body.ToString() + "'不是Long形");
            }
            return result;
        }
    }
}
