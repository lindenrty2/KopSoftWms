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
            for (int i = 1; i <= 10; i++)
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
            for (int i = 1; i <= 99; i++)
            {
                long id = reservoirarea.ReservoirAreaId * 100 + i;
                Wms_storagerack storagerack = new Wms_storagerack()
                {
                    StorageRackId = id,
                    StorageRackNo = "KW-00"+id.ToString().PadRight(2,'0'),
                    StorageRackName = "库位000" + i,
                    ReservoirAreaId = i,
                    ReservoirAreaName = reservoirarea.ReservoirAreaName,
                    WarehouseId = warehouse.WarehouseId,
                    Row = i,
                    Floor = i / 10 + 1,
                    Column = 1 % 10,
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
                    InventoryBoxNo = "LX-00" + id.ToString().PadRight(2, '0'),
                    InventoryBoxName = "料箱-00" + id.ToString().PadRight(2, '0'),
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