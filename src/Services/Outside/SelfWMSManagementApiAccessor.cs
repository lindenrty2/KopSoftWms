using IServices.Outside;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using YL.Core.Dto;
using YL.Core.Entity;
using YL.Utils.Pub;

namespace Services.Outside
{
    public class SelfWMSManagementApiAccessor : IWMSManagementApiAccessor
    {


        private ISqlSugarClient _sqlClient;
        private Wms_warehouse _warehouse;
        public Wms_warehouse Warehouse { get { return _warehouse; } }

        private SysUserDto _userDto;
        private SysUserDto UserDto { get { return _userDto; } }

        public SelfWMSManagementApiAccessor(Wms_warehouse warehouse, SqlSugar.ISqlSugarClient sqlClient, SysUserDto userDto)
        {
            _warehouse = warehouse;
            _sqlClient = sqlClient;
            _userDto = userDto;
        }

        public async Task<RouteData<Wms_inventorybox>> AddInventoryBox(Wms_inventorybox box)
        {
            if (_sqlClient.Queryable<Wms_inventorybox>().Any(c => c.InventoryBoxNo == box.InventoryBoxNo && c.IsDel == DeleteFlag.Normal))
            {
                return RouteData<Wms_inventorybox>.From(PubMessages.E1022_INVENTORYBOX_NO_DUPLICATE);
            }
            Wms_storagerack rack = await _sqlClient.Queryable<Wms_storagerack>()
                .FirstAsync(x => x.ReservoirAreaId == box.ReservoirAreaId && x.StorageRackId == box.StorageRackId);
            if (rack == null)
            {
                return RouteData<Wms_inventorybox>.From(PubMessages.E0006_DATA_VAILD_FAIL);
            }
            box.ReservoirAreaName = rack.ReservoirAreaName;
            box.StorageRackName = rack.StorageRackName;
            box.InventoryBoxId = PubId.SnowflakeId;
            box.CreateBy = UserDto.UserId;
            box.CreateDate = DateTime.Now;
            box.CreateUser = UserDto.UserName;
            box.ModifiedBy = UserDto.UserId;
            box.ModifiedDate = DateTime.Now;
            box.ModifiedUser = UserDto.UserName;
            if (await _sqlClient.Insertable(box).ExecuteCommandAsync() == 0)
            {
                return RouteData<Wms_inventorybox>.From(PubMessages.E1023_INVENTORYBOX_ADD_FAIL);
            }
            else
            {
                return RouteData<Wms_inventorybox>.From(PubMessages.I1003_INVENTORYBOX_ADD_SCCUESS); 
            }
        }

        public async Task<RouteData<Wms_inventorybox>> UpdateInventoryBox(long inventoryBoxId,Wms_inventorybox box)
        {
            Wms_storagerack rack = await _sqlClient.Queryable<Wms_storagerack>()
            .FirstAsync(x => x.ReservoirAreaId == box.ReservoirAreaId && x.StorageRackId == box.StorageRackId);
            if (rack == null)
            {
                return RouteData<Wms_inventorybox>.From(PubMessages.E0006_DATA_VAILD_FAIL);
            }
            box.ReservoirAreaName = rack.ReservoirAreaName;
            box.StorageRackName = rack.StorageRackName;
            box.InventoryBoxId = inventoryBoxId; 
            box.ModifiedBy = UserDto.UserId;
            box.ModifiedDate = DateTime.Now;
            box.ModifiedUser = UserDto.UserName;
            if (await _sqlClient.Updateable(box).ExecuteCommandAsync() == 0)
            {
                return RouteData<Wms_inventorybox>.From(PubMessages.E1024_INVENTORYBOX_UPDATE_FAIL);
            }
            return RouteData<Wms_inventorybox>.From(PubMessages.I1004_INVENTORYBOX_UPDATE_SCCUESS);
        } 

        public async Task<RouteData> DeleteInventoryBox(long inventoryBoxId)
        {
            //判断库存数量，库存数量小于等于0，才能删除
            var isExist = _sqlClient.Queryable<Wms_inventorybox>().Any(c => c.InventoryBoxId == SqlFunc.ToInt64(inventoryBoxId));
            if (isExist)
            {
                return RouteData.From(PubMessages.E1011_INVENTORYBOX_NOTFOUND);
            }
            bool hasItems = _sqlClient.Queryable<Wms_inventory>().Any(c => c.InventoryBoxId == SqlFunc.ToInt64(inventoryBoxId) && c.Qty != 0);
            if (hasItems)
            {
                return RouteData.From(PubMessages.E1025_INVENTORYBOX_DELETE_FAIL,"料箱中存在物料"); 
            }

            Wms_inventorybox box = new Wms_inventorybox {
                InventoryBoxId = inventoryBoxId,
                IsDel = 0,
                ModifiedBy = UserDto.UserId,
                ModifiedDate = DateTime.Now
            };
            
            if(await _sqlClient.Updateable(box).UpdateColumns(c => new { c.IsDel, c.ModifiedBy, c.ModifiedDate }).ExecuteCommandAsync() == 0)
            {
                return RouteData<Wms_inventorybox>.From(PubMessages.E1025_INVENTORYBOX_DELETE_FAIL);
            }
            return RouteData<Wms_inventorybox>.From(PubMessages.I1005_INVENTORYBOX_DELETE_SCCUESS);
        }

        public async Task<RouteData<Sys_dict[]>> GetMaterialTypes(string search)
        {
            ISugarQueryable<Sys_dict> query = _sqlClient.Queryable<Sys_dict>().Where(x => x.DictType == SqlFunc.ToString(Convert.ToInt32(PubDictType.material)));
            if(string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x => x.DictName.Contains(search));
            }
            List<Sys_dict> result = await query.ToListAsync();
            return RouteData<Sys_dict[]>.From(result.ToArray(), result.Count);
        }

        public async Task<RouteData<Wms_StockCountInventoryBoxDto[]>> GetStockCountInventoryBoxList(long? materialTypeId, string materialName)
        {
            ISugarQueryable< Wms_inventory, Wms_material, Wms_inventorybox, Wms_storagerack> query = _sqlClient.Queryable<Wms_inventory, Wms_material,Wms_inventorybox, Wms_storagerack>(
                (i,m, ib, s) => new object[] {
                   JoinType.Left,i.InventoryBoxId==ib.StorageRackId,
                   JoinType.Left,i.MaterialId==m.MaterialId,
                   JoinType.Left,ib.StorageRackId==s.StorageRackId
                 })
                .Where((i, ib) => i.IsDel == DeleteFlag.Normal);
            if (materialTypeId.HasValue)
            {
                query = query.Where((i, m, ib, s) => m.MaterialType  == materialTypeId.Value);
            }
            if (!string.IsNullOrEmpty(materialName))
            {
                query = query.Where((i, m, ib, s) =>  i.MaterialName.Contains(materialName) );
            }
            List<Wms_StockCountInventoryBoxDto> result = await query.Select(
                (i, m, ib, s) => new Wms_StockCountInventoryBoxDto
                {
                    StorageRackNo = s.StorageRackNo,
                    InventoryBoxId = ib.InventoryBoxId.ToString(),
                    InventoryBoxNo = s.StorageRackNo,

                    Row = ib.Row ?? 0,
                    Column = ib.Column ?? 0,
                    Floor = ib.Floor ?? 0,
                    Position = i.Position,

                    MaterialNo = m.MaterialNo,
                    MaterialName = m.MaterialName,
                    MaterialType = m.MaterialTypeName,
                    Qty = i.Qty,

                    ModifiedBy = i.ModifiedBy,
                    ModifiedUser = i.ModifiedUser,
                    ModifiedDate = i.ModifiedDate,

                }).ToListAsync();

            return RouteData<Wms_StockCountInventoryBoxDto[]>.From(result.ToArray(), result.Count);
        }
    }
}
