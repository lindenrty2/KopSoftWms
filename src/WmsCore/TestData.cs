using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using NLog;
using SqlSugar;
using System;
using YL.Core.Entity;
using YL.Utils.Configs;
using YL.Utils.Pub;

namespace YL
{
    public class TestData
    {
        public static void Create(SqlSugarClient sqlClient,ILogger logger)  
        {
            if (sqlClient.Queryable<Wms_warehouse>().Any())
            {
                logger.Info("无需创建初始仓库数据");
                return;
            }
            logger.Info("开始创建初始仓库数据");
            try
            {
                sqlClient.BeginTran();
                for (int i = 1; i <= 4; i++)
                {
                    Wms_warehouse warehouse = new Wms_warehouse()
                    {
                        WarehouseId = i,
                        WarehouseNo = $"A0{i - 1}" ,
                        WarehouseName = "仓库000" + i,
                        IFAddress = "",
                        CreateBy = 1,
                        CreateDate = DateTime.Now,
                        ModifiedBy = 1,
                        ModifiedDate = DateTime.Now,
                        IsDel = DeleteFlag.Normal,
                        Remark = ""
                    };
                    sqlClient.Insertable(warehouse).ExecuteCommand();
                    if (i == 1)
                    {
                        CreateReservoirArea(sqlClient, warehouse);
                    }
                }
                sqlClient.CommitTran();
                logger.Info("创建初始仓库数据成功");
            }
            catch(Exception ex)
            {
                logger.Error(ex, "创建初始仓库数据失败");
                sqlClient.RollbackTran();
            }
        }

        protected static void CreateReservoirArea(SqlSugarClient sqlClient,Wms_warehouse warehouse)
        {
            for (int i = 1; i <= 3; i++)
            {
                try
                {
                    Wms_reservoirarea reservoirarea = new Wms_reservoirarea()
                    {
                        ReservoirAreaId = i,
                        ReservoirAreaNo = "KQ000" + i,
                        ReservoirAreaName = "库区000" + i,
                        WarehouseId = warehouse.WarehouseId,
                        Remark = "",
                        CreateBy = 1,
                        CreateUser = "初始数据",
                        CreateDate = DateTime.Now,
                        ModifiedBy = 1,
                        ModifiedUser = "初始数据",
                        ModifiedDate = DateTime.Now,
                        IsDel = DeleteFlag.Normal,
                    };
                    sqlClient.Insertable(reservoirarea).ExecuteCommand();

                    if(i == 1)
                    {
                        CreateStoragerack(sqlClient, warehouse, reservoirarea, 1, 10, 4);
                        CreateStoragerack(sqlClient, warehouse, reservoirarea, 2, 10, 4);
                    }
                    else if(i == 2)
                    {
                        CreateStoragerack(sqlClient, warehouse, reservoirarea, 3, 14, 7);
                        CreateStoragerack(sqlClient, warehouse, reservoirarea, 4, 14, 7);
                        CreateStoragerack(sqlClient, warehouse, reservoirarea, 5, 14, 7);
                        CreateStoragerack(sqlClient, warehouse, reservoirarea, 6, 14, 7);

                    }
                    else if (i == 3)
                    {
                        CreateStoragerack(sqlClient, warehouse, reservoirarea, 7, 14, 16);
                        CreateStoragerack(sqlClient, warehouse, reservoirarea, 8, 14, 16); 
                    }
                }
                catch(Exception e)
                {
                    throw e;
                }
            }

        }
         
        protected static void CreateStoragerack(SqlSugarClient sqlClient, Wms_warehouse warehouse, Wms_reservoirarea reservoirarea,int row,int columnCount,int floorCount)
        {
            for (int column = 1; column <= columnCount; column++)
            {
                for (int floor = 1; floor <= floorCount; floor++)
                {
                    string code = warehouse.WarehouseId.ToString().PadLeft(2,'0') + row.ToString().PadLeft(2, '0') + column.ToString().PadLeft(2,'0') + floor.ToString().PadLeft(2,'0');
                    string no = row.ToString().PadLeft(2, '0') + column.ToString().PadLeft(2, '0') + floor.ToString().PadLeft(2, '0');
                    int id = Convert.ToInt32(code);
                    Wms_storagerack storagerack = new Wms_storagerack()
                    {
                        StorageRackId = id,
                        StorageRackNo = "KW-" + no,
                        StorageRackName = "库位" + code,
                        ReservoirAreaId = reservoirarea.ReservoirAreaId,
                        ReservoirAreaName = reservoirarea.ReservoirAreaName,
                        WarehouseId = warehouse.WarehouseId,
                        Row = row,
                        Floor = floor,
                        Column = column,
                        Remark = "",
                        CreateBy = 1,
                        CreateUser = "初始数据",
                        CreateDate = DateTime.Now,
                        ModifiedBy = 1,
                        ModifiedUser = "初始数据",
                        ModifiedDate = DateTime.Now,
                        IsDel = DeleteFlag.Normal,
                    };
                    sqlClient.Insertable(storagerack).ExecuteCommand();
                    Wms_inventorybox box = new Wms_inventorybox()
                    {
                        InventoryBoxId = id,
                        InventoryBoxNo = "LK_LX_" + code,
                        InventoryBoxName = "料箱-" + code,
                        WarehouseId = reservoirarea.WarehouseId,
                        ReservoirAreaId = reservoirarea.ReservoirAreaId,
                        ReservoirAreaName = reservoirarea.ReservoirAreaName,
                        StorageRackId = storagerack.StorageRackId,
                        StorageRackName = storagerack.StorageRackName,
                        Row = storagerack.Row,
                        Column = storagerack.Column,
                        Floor = storagerack.Floor,
                        Size = 1,
                        UsedSize = 0,
                        Remark = "",
                        IsDel = DeleteFlag.Normal,
                        Status = InventoryBoxStatus.InPosition,
                        CreateBy = 1,
                        CreateUser = "初始数据",
                        CreateDate = DateTime.Now,
                        ModifiedBy = 1,
                        ModifiedUser = "初始数据",
                        ModifiedDate = DateTime.Now
                    };
                    sqlClient.Insertable(box).ExecuteCommand();
                }
               
            }
             
        }
    }
}