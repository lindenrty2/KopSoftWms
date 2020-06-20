using System;
using System.Collections.Generic;
using System.Text;

namespace YL.Core.Dto
{
    public class OutsideLogisticsEnquiryArg
    {
        /// <summary>
        /// 物流任务编号
        /// </summary>
        public string LogisticsId { get; set; }
        /// <summary>
        /// 设备编号
        /// </summary>
        public string  EquipmentId { get; set; }
        /// <summary>
        /// 设备名称
        /// </summary>
        public string EquipmentName { get; set; }

        public static OutsideLogisticsEnquiryArg Create(string logisticsId,string equipmentid, string equipmentname)
        {
            return new OutsideLogisticsEnquiryArg()
            {
                LogisticsId = logisticsId,
                EquipmentId = equipmentid,
                EquipmentName = equipmentname
            };
        }
    }

    public class OutsideLogisticsEnquiryResult
    {
        /// <summary>
        /// 设备编号
        /// </summary>
        public string EquipmentId { get; set; }
        /// <summary>
        /// 设备名称
        /// </summary>
        public string EquipmentName { get; set; }
        /// <summary>
        /// 当前位置
        /// </summary>
        public string Position { get; set; }
        /// <summary>
        /// 当前状态
        /// </summary>
        public string Status { get; set; } 


    }
}
