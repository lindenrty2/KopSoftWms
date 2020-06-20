using System;
using System.Collections.Generic;
using System.Text;

namespace YL.Core.Dto
{
    public class OutsideLogisticsControlArg
    {
        /// <summary>
        /// 物流任务编号
        /// </summary>
        public string LogisticsId { get; set; }
        /// <summary>
        /// 出发位置
        /// </summary>
        public string StartPoint { get; set; }
        /// <summary>
        /// 终点位置
        /// </summary>
        public string Destination { get; set; }

        public static OutsideLogisticsControlArg Create(string logisticsId,string startpoint, string destination)
        {
            return new OutsideLogisticsControlArg()
            {
                LogisticsId = logisticsId,
                StartPoint = startpoint,
                Destination = destination 
            };
            
        }
    }

    public class OutsideLogisticsControlResult
    {
        /// <summary>
        /// 消息接收成功与否
        /// </summary>
        public bool Success { get; set; }
        /// <summary>
        /// 设备编号
        /// </summary>
        public string EquipmentId { get; set; }
        /// <summary>
        /// 设备名称
        /// </summary>
        public string EquipmentName { get; set; }
        /// <summary>
        /// 是否正常执行
        /// </summary>
        public bool IsNormalExecution { get; set; }
        /// <summary>
        /// 错误编号
        /// </summary>
        public string ErrorId { get; set; }
        /// <summary>
        /// 错误内容
        /// </summary>
        public string ErrorInfo { get; set; }
   

    }
}
