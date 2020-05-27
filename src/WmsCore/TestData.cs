using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using SqlSugar;
using System;
using YL.Core.Entity;
using YL.Utils.Configs;
using YL.Utils.Pub;

namespace YL
{
    public class TestData
    {
        public static void Create(SqlSugarClient sqlClient)  
        {
            if (sqlClient.Queryable<Wms_warehouse>().Any()) return;
            for (int i = 1; i <= 4; i++)
            {
                Wms_warehouse warehouse = new Wms_warehouse()
                {
                    WarehouseId = i,
                    WarehouseNo = "CK000" + i,
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
        }

        protected static void CreateReservoirArea(SqlSugarClient sqlClient,Wms_warehouse warehouse)
        {
            for (int i = 1; i <= 1; i++)
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
                CreateStoragerack(sqlClient, warehouse,reservoirarea);
            }

        }

        protected static void CreateStoragerack(SqlSugarClient sqlClient, Wms_warehouse warehouse, Wms_reservoirarea reservoirarea)
        {
            CreateStoragerack(sqlClient, warehouse, reservoirarea, 1, 10, 4);
            CreateStoragerack(sqlClient, warehouse, reservoirarea, 2, 10, 4);
            CreateStoragerack(sqlClient, warehouse, reservoirarea, 3, 14, 7);
            CreateStoragerack(sqlClient, warehouse, reservoirarea, 4, 14, 7);
            CreateStoragerack(sqlClient, warehouse, reservoirarea, 5, 14, 7);
            CreateStoragerack(sqlClient, warehouse, reservoirarea, 6, 14, 7);
            CreateStoragerack(sqlClient, warehouse, reservoirarea, 7, 14, 16);
            CreateStoragerack(sqlClient, warehouse, reservoirarea, 8, 14, 16);
        }

        protected static void CreateStoragerack(SqlSugarClient sqlClient, Wms_warehouse warehouse, Wms_reservoirarea reservoirarea,int row,int columnCount,int floorCount)
        {
            for (int column = 1; column <= columnCount; column++)
            {
                for (int floor = 1; floor <= floorCount; floor++)
                {
                    string code = warehouse.WarehouseId.ToString().PadLeft(2,'0') + row.ToString().PadLeft(2, '0') + column.ToString().PadLeft(2,'0') + floor.ToString().PadLeft(2,'0');
                    int id = Convert.ToInt32(code);
                    Wms_storagerack storagerack = new Wms_storagerack()
                    {
                        StorageRackId = id,
                        StorageRackNo = "KW-" + code,
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
                        InventoryBoxNo = "LX-" + code,
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