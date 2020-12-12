using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace YL.Core.Dto
{
    public class OutsideLogisticsFinishResponse
    {
        /// <summary>
        /// 物流任务编号
        /// </summary>
        public string LogisticsId { get; set; }
        /// <summary>
        /// 物流完成时间
        /// </summary>
        public string LogisticsFinishTime { get; set; }
        /// <summary>
        /// 作业区
        /// </summary>
        public string WorkAreaName { get; set; }
        /// <summary>
        /// 错误编号
        /// </summary>
        public string ErrorId { get; set; }
        /// <summary>
        /// 错误内容
        /// </summary>
        public string ErrorInfo { get; set; } 
     

    }

    public class OutsideLogisticsFinishResponseResult
    {
        /// <summary>
        /// 物流任务编号
        /// </summary>
        [JsonProperty("LogisticsId")]
        public string LogisticsId { get; set; }
        /// <summary>
        /// 是否正常接收
        /// </summary>
        [JsonProperty("IsNormalExecution")]
        public bool IsNormalExecution { get; set; }
        /// <summary>
        /// 错误编号
        /// </summary>
        [JsonProperty("ErrorId")]
        public string ErrorId { get; set; }

    }
}
