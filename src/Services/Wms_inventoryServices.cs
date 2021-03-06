using IRepository;
using IServices;
using SqlSugar;
using System;
using YL.Core.Dto;
using YL.Core.Entity;
using YL.Utils.Extensions;
using YL.Utils.Json;
using YL.Utils.Table;

namespace Services
{
    public class Wms_inventoryServices : BaseServices<Wms_inventory>, IWms_inventoryServices
    {
        private readonly IWms_inventoryRepository _repository;
        private readonly SqlSugarClient _client;

        public Wms_inventoryServices(
            SqlSugarClient client,
            IWms_inventoryRepository repository) : base(repository)
        {
            _client = client;
            _repository = repository;
        }

        public string PageList(PubParams.InventoryBootstrapParams bootstrap)
        {
            int totalNumber = 0;
            if (bootstrap.offset != 0)
            {
                bootstrap.offset = bootstrap.offset / bootstrap.limit + 1;
            }
            var query = _client.Queryable<Wms_inventory, Wms_inventorybox,Wms_material, Wms_storagerack, Sys_user, Sys_user>
                ((s,sb, m, sr, scu, smu) => new object[] {
                   JoinType.Left,s.InventoryBoxId==sb.InventoryBoxId,
                   JoinType.Left,s.MaterialId==m.MaterialId,
                   JoinType.Left,sb.StorageRackId==sr.StorageRackId,
                   JoinType.Left,s.CreateBy==scu.UserId,
                   JoinType.Left,s.ModifiedBy==smu.UserId,
                 })
                 .Where((s, sb, m, sr, scu, smu) => sb.WarehouseId == bootstrap.storeId && s.IsDel == 1 && sb.IsDel == 1 && sr.IsDel == 1 && scu.IsDel == 1)
                 .Select((s, sb , m, sr, scu, smu) => new
                 {
                     InventoryId = s.InventoryId.ToString(),
                     InventoryBoxId = sb.InventoryBoxId.ToString(),
                     s.Qty,
                     s.IsLocked,
                     MaterialId = m.MaterialId.ToString(),
                     m.MaterialNo,
                     m.MaterialName, 
                     StorageRackId = sr.StorageRackId.ToString(),
                     sr.StorageRackNo,
                     sr.StorageRackName,
                     sb.InventoryBoxNo,
                     sb.InventoryBoxName,
                     s.IsDel,
                     s.Remark,
                     CName = scu.UserNickname,
                     s.CreateDate,
                     UName = smu.UserNickname,
                     s.ModifiedDate
                 }).MergeTable();
            if (!bootstrap.search.IsEmpty())
            {
                query.Where((s) => s.MaterialNo.Contains(bootstrap.search) || s.MaterialName.Contains(bootstrap.search));
            }
            if (!bootstrap.StorageRackId.IsEmpty())
            {
                query.Where(s => s.StorageRackId == bootstrap.StorageRackId);
            }
            if (!bootstrap.InventoryBoxId.IsEmpty())
            {
                query.Where(s => s.InventoryBoxId == bootstrap.InventoryBoxId);
            }
            if (!bootstrap.MaterialId.IsEmpty())
            {
                query.Where(s => s.MaterialId == bootstrap.MaterialId);
            }
            if (!bootstrap.datemin.IsEmpty() && !bootstrap.datemax.IsEmpty())
            {
                query.Where(s => s.CreateDate > bootstrap.datemin.ToDateTimeB() && s.CreateDate <= bootstrap.datemax.ToDateTimeE());
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

        public string SearchInventory(PubParams.InventoryBootstrapParams bootstrap)
        {
            int totalNumber = 0;
            if (bootstrap.offset != 0)
            {
                bootstrap.offset = bootstrap.offset / bootstrap.limit + 1;
            }
            var query = _client.Queryable<Wms_inventory, Wms_inventorybox, Wms_material, Wms_storagerack, Sys_user, Sys_user>
                ((s, sb, p, d, c, u) => new object[] {
                   JoinType.Left,s.MaterialId==p.MaterialId,
                   JoinType.Left,s.InventoryBoxId==sb.InventoryBoxId,
                   JoinType.Left,sb.StorageRackId==d.StorageRackId,
                   JoinType.Left,s.CreateBy==c.UserId,
                   JoinType.Left,s.ModifiedBy==u.UserId,
                 })
                 .Where((s, sb, p, d, c, u) => s.IsDel == 1 && sb.IsDel == 1 && d.IsDel == 1 && c.IsDel == 1)
                 .Select((s, sb, p, d, c, u) => new
                 {
                     InventoryId = s.InventoryId.ToString(),
                     InventoryBoxId = s.InventoryBoxId.ToString(),
                     s.Qty,
                     MaterialId = p.MaterialId.ToString(),
                     p.MaterialNo,
                     p.MaterialName, 
                     StorageRackId = d.StorageRackId.ToString(),
                     d.StorageRackNo,
                     d.StorageRackName,
                     s.IsDel,
                     s.Remark,
                     CName = c.UserNickname,
                     s.CreateDate,
                     UName = u.UserNickname,
                     s.ModifiedDate
                 }).MergeTable();
            if (!bootstrap.search.IsEmpty())
            {
                query.Where((s) => s.MaterialId == bootstrap.search);
            }
            if (!bootstrap.StorageRackId.IsEmpty())
            {
                query.Where((s) => s.StorageRackId == bootstrap.StorageRackId);
            }
            if (!bootstrap.InventoryBoxId.IsEmpty())
            {
                query.Where((s) => s.InventoryBoxId == bootstrap.InventoryBoxId);
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
         
    }
}