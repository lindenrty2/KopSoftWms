using IRepository;
using IServices;
using SqlSugar;
using System;
using System.Threading.Tasks;
using YL.Core.Dto;
using YL.Core.Entity;
using YL.Utils.Extensions;
using YL.Utils.Json;
using YL.Utils.Pub;
using YL.Utils.Table;

namespace Services
{
    public class Wms_inventoryBoxServices : BaseServices<Wms_inventorybox>, IWms_inventoryBoxServices
    {
        private readonly IWms_inventoryBoxRepository _repository;
        private readonly SqlSugarClient _client;

        

        public Wms_inventoryBoxServices(
            SqlSugarClient client,
            IWms_inventoryBoxRepository repository) : base(repository)
        {
            _client = client;
            _repository = repository;
        }

        public string PageList(PubParams.InventoryBoxBootstrapParams bootstrap)
        {
            int totalNumber = 0;
            if (bootstrap.offset != 0)
            {
                bootstrap.offset = bootstrap.offset / bootstrap.limit + 1;
            }
            var query = _client.Queryable< Wms_inventorybox, Wms_storagerack, Sys_user, Sys_user>
                ((ib, sr, su, su2) => new object[] {
                   JoinType.Left,ib.StorageRackId==sr.StorageRackId && sr.IsDel == 1,
                   JoinType.Left,ib.CreateBy==su.UserId && su.IsDel == 1,
                   JoinType.Left,ib.ModifiedBy==su2.UserId,
                 })
                 .Where(( ib, sr, su, su2) => ib.WarehouseId == bootstrap.storeId && ib.IsDel == 1)
                 .Select(( ib , sr, su, su2) => new
                 {
                     InventoryBoxId = ib.InventoryBoxId.ToString(),
                     ib.InventoryBoxNo,
                     ib.InventoryBoxName,
                     ib.Size,
                     StorageRackId = sr.StorageRackId.ToString(),
                     sr.StorageRackNo,
                     sr.StorageRackName,
                     ib.IsDel,
                     Status = (int)ib.Status,
                     ib.Remark,
                     CName = su.UserNickname,
                     ib.CreateDate,
                     UName = su2.UserNickname,
                     ib.ModifiedDate
                 }).MergeTable();
            if (!bootstrap.StorageRackId.IsEmpty())
            {
                query.Where(s => s.StorageRackId == bootstrap.StorageRackId);
            }
            if (!bootstrap.datemin.IsEmpty() && !bootstrap.datemax.IsEmpty())
            {
                query.Where(s => s.CreateDate > bootstrap.datemin.ToDateTimeB() && s.CreateDate <= bootstrap.datemax.ToDateTimeE());
            }
            if (!bootstrap.search.IsEmpty())
            {
                query.Where(s => s.InventoryBoxNo.Contains(bootstrap.search) || s.InventoryBoxName.Contains(bootstrap.search));
            }
            if (bootstrap.order.Equals("desc", StringComparison.OrdinalIgnoreCase))
            {
                query.OrderBy($"MergeTable.{bootstrap.sort} desc");
            }
            else
            {
                query.OrderBy($"MergeTable.{bootstrap.sort} asc");
            }
            var list = query.ToPageList(bootstrap.offset, bootstrap.limit, ref totalNumber);
            return Bootstrap.GridData(list, totalNumber).JilToJson();
        }

        public async Task<Wms_inventorybox> AutoSelectBox(bool isFullBox)
        {
            var query = _client.Queryable<Wms_inventorybox, Wms_storagerack>
                ((ib, sr) => new object[] {
                   JoinType.Left,ib.StorageRackId==sr.StorageRackId && sr.IsDel == 1
                 })
                 .Where((ib, sr) => ib.UsedSize < ib.Size 
                 && ib.Status == InventoryBoxStatus.InPosition);
            if (isFullBox)
            {
                query = query.Where((ib, sr) => ib.Size == 1);
            }
            else
            {
                query = query.Where((ib, sr) => ib.Size > 1);  
            }
            return await query.FirstAsync();
        }

    }
}