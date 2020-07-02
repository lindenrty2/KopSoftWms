using Newtonsoft.Json;
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

        public static DateTime GuardDate(Expression<Func<string>> yearExpr, Expression<Func<string>> monthExpr)
        {
            string year = yearExpr.Compile().Invoke();
            string month = monthExpr.Compile().Invoke();
            int result;
            try
            {
                return new DateTime(Convert.ToInt32(year), Convert.ToInt32(month), 1);
            }
            catch(Exception)
            {
                throw new WMSException(Pub.PubMessages.E0010_PARAMETER_TYPE_INVAILD, $"{yearExpr.Body.ToString()},{monthExpr.Body.ToString()}不能解析为有效日期");
            }
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

        public static T GuardType<T>(Expression<Func<string>> expr)
        {
            string value = expr.Compile().Invoke();
            try
            {
                return JsonConvert.DeserializeObject<T>(value);
            }
            catch (Exception)
            {
                throw new WMSException(Pub.PubMessages.E0010_PARAMETER_TYPE_INVAILD, $"{expr.Body.ToString()}不是{typeof(T).FullName}形");
            }
             
        }
    }
}
