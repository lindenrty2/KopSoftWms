using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using YL.Utils.Json;
using YL.Utils.Pub;
using YL.Utils.Table;

namespace YL.Core.Dto
{
    public class RouteData
    {
        public int Code { get; set; }

        public string CodeString {
            get
            {
                if(this.Code < 0)
                {
                    return "E" + this.Code.ToString();
                }
                return this.Code.ToString();
            }
        }
        public string Message { get; set; }
        public bool IsSccuess { get { return Code >= 0; } }

        public static RouteData From(int code, string message)
        {
            return new RouteData()
            {
                Code = code,
                Message = message
            };
        } 

        public static RouteData From(MessageItem messageItem)
        {
            return RouteData.From(messageItem.Code, messageItem.Message);
        }

        public static RouteData From(MessageItem messageItem, string addition)
        {
            return RouteData.From(messageItem.Code, messageItem.Message + "\r\n" + addition);
        }

        public static RouteData From(MessageItem messageItem, Exception ex)
        {
            return RouteData.From(messageItem.Code, messageItem.Message + "\r\n" + ex.ToString());
        }

        public static RouteData From()
        {
            return RouteData.From(1, "");
        }
        public static RouteData<T> From<T>(RouteData routeData)
        {
            return RouteData<T>.From(routeData.Code, routeData.Message);
        }
         
    }


    public class RouteData<T> : RouteData
    {
        public T Data { get; set; }

        public int TotalCount { get; set; }

        public static RouteData<T> From(int code, string message,T data = default(T),int totalCount = -1)
        {
            return new RouteData<T>()
            {
                Code = code,
                Message = message,
                Data = data,
                TotalCount = totalCount
            };
        } 

        public static RouteData<T> From(MessageItem messageItem, T data = default(T), int totalCount = -1)
        {
            return RouteData<T>.From(messageItem.Code, messageItem.Message, data, totalCount);
        }

        public static RouteData<T> From(MessageItem messageItem,string addition, T data = default(T), int totalCount = -1)
        {
            return RouteData<T>.From(messageItem.Code, messageItem.Message + "\r\n" + addition, data, totalCount);
        }

        public static RouteData<T> From(MessageItem messageItem, Exception ex, T data = default(T), int totalCount = -1)
        {
            return RouteData<T>.From(messageItem.Code, messageItem.Message + "\r\n" + ex.ToString(), data, totalCount);
        }

        public static RouteData<T> From(T data, int totalCount = -1)
        {
            return RouteData<T>.From(1, "", data, totalCount);
        }
        public static RouteData<T> From(RouteData route, int totalCount = -1)
        {
            return RouteData<T>.From(route.Code,route.Message,default(T), totalCount);
        }

        public PageGridData ToGridData()
        {
            return new PageGridData(this.Data, this.TotalCount);
        }

        public string ToGridJson()
        {
            return this.ToGridData().JilToJson();
        }
         
    }
}
