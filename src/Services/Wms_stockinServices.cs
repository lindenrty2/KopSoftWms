using IRepository;
using IServices;
using Microsoft.AspNetCore.Hosting;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using YL.Core.Dto;
using YL.Core.Entity;
using YL.Utils.Extensions;
using YL.Utils.Files;
using YL.Utils.Json;
using YL.Utils.Pub;
using YL.Utils.Table;

namespace Services
{
    public class Wms_stockinServices : BaseServices<Wms_stockin>, IWms_stockinServices
    {
        private readonly IWms_stockinRepository _repository;
        private readonly IWms_stockindetailRepository _detail;
        private readonly SqlSugarClient _client;
        private readonly IWms_inventoryRepository _inventory;
        private readonly IWms_inventoryrecordRepository _inventoryrecord;
        private readonly IHostingEnvironment _env;

        public Wms_stockinServices(
            SqlSugarClient client,
            IWms_inventoryRepository inventoryRepository,
            IWms_inventoryrecordRepository inventoryrecordRepository,
            IWms_stockindetailRepository detail,
            IHostingEnvironment env,
            IWms_stockinRepository repository) : base(repository)
        {
            _client = client;
            _repository = repository;
            _detail = detail;
            _inventory = inventoryRepository;
            _inventoryrecord = inventoryrecordRepository;
            _env = env;
        }

        public string PageList(PubParams.StockInBootstrapParams bootstrap)
        {
            int totalNumber = 0;
            if (bootstrap.offset != 0)
            {
                bootstrap.offset = bootstrap.offset / bootstrap.limit + 1;
            }
            var query = _client.Queryable<Wms_stockin, Wms_supplier, Sys_dict, Sys_user, Sys_user>
                ((s, p, d, c, u) => new object[] {
                   JoinType.Left,s.SupplierId==p.SupplierId,
                   JoinType.Left,s.StockInType==d.DictId,
                   JoinType.Left,s.CreateBy==c.UserId,
                   JoinType.Left,s.ModifiedBy==u.UserId,
                 })
                 .Where((s, p, d, c, u) => s.WarehouseId == bootstrap.storeId && s.IsDel == 1 && d.IsDel == 1 && c.IsDel == 1)
                 .Select((s, p, d, c, u) => new
                 {
                     StockInId = s.StockInId.ToString(),
                     StockInType = d.DictName,
                     StockInTypeId = s.StockInType.ToString(),
                     s.StockInStatus,
                     s.StockInNo,
                     s.OrderNo,
                     s.SupplierId,
                     p.SupplierNo,
                     p.SupplierName,
                     s.IsDel,
                     s.Remark,
                     CName = c.UserNickname,
                     s.CreateDate,
                     UName = u.UserNickname,
                     s.ModifiedDate
                 }).MergeTable();
            if (!bootstrap.search.IsEmpty())
            {
                query.Where((s) => s.StockInNo.Contains(bootstrap.search) || s.OrderNo.Contains(bootstrap.search));
            }
            if (!bootstrap.datemin.IsEmpty() && !bootstrap.datemax.IsEmpty())
            {
                query.Where(s => s.CreateDate > bootstrap.datemin.ToDateTimeB() && s.CreateDate <= bootstrap.datemax.ToDateTimeE());
            }
            if (!bootstrap.StockInType.IsEmpty())
            {
                query.Where((s) => s.StockInTypeId.Contains(bootstrap.StockInType));
            }
            if (!bootstrap.StockInStatus.IsEmpty())
            {
                query.Where((s) => s.StockInStatus == bootstrap.StockInStatus.ToByte());
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

        public string PrintList(string stockInId)
        {
            var list1 = _client.Queryable<Wms_stockin, Wms_supplier, Sys_dict, Sys_user, Sys_user>
                ((s, p, d, c, u) => new object[] {
                   JoinType.Left,s.SupplierId==p.SupplierId,
                   JoinType.Left,s.StockInType==d.DictId,
                   JoinType.Left,s.CreateBy==c.UserId,
                   JoinType.Left,s.ModifiedBy==u.UserId,
                 })
                 .Where((s, p, d, c, u) => s.IsDel == 1 && d.IsDel == 1 && c.IsDel == 1)
                 .Select((s, p, d, c, u) => new
                 {
                     StockInId = s.StockInId.ToString(),
                     StockInType = d.DictName,
                     StockInTypeId = s.StockInType.ToString(),
                     s.StockInStatus,
                     s.StockInNo,
                     s.OrderNo,
                     s.SupplierId,
                     p.SupplierNo,
                     p.SupplierName,
                     s.IsDel,
                     s.Remark,
                     CName = c.UserNickname,
                     s.CreateDate,
                     UName = u.UserNickname,
                     s.ModifiedDate
                 }).MergeTable().Where(s => s.StockInId == stockInId).ToList();
            bool flag1 = true;
            bool flag2 = true;
            var list2 = _client.Queryable<Wms_stockindetail, Wms_material, Wms_stockin, Sys_user, Sys_user >
                 ((s, m, p,  c, u) => new object[] {
                   JoinType.Left,s.MaterialId==m.MaterialId && m.IsDel == 1,
                   JoinType.Left,s.StockInId==p.StockInId && p.IsDel == 1,
                   JoinType.Left,s.CreateBy==c.UserId && c.IsDel == 1,
                   JoinType.Left,s.ModifiedBy==u.UserId && u.IsDel == 1
                  })
                  //.Where((s, m, p, g, c, u, a) => s.IsDel == 1 && p.IsDel == 1 && g.IsDel == 1 && c.IsDel == 1)
                  .Select((s, m, p, c, u) => new
                  {
                      StockInId = s.StockInId.ToString(),
                      StockInDetailId = s.StockInDetailId.ToString(),
                      m.MaterialNo,
                      m.MaterialName,
                      Status = SqlFunc.IF(s.Status == 1).Return(StockInStatus.initial.GetDescription())
                      .ElseIF(s.Status == 2).Return(StockInStatus.task_confirm.GetDescription())
                      .ElseIF(s.Status == 3).Return(StockInStatus.task_canceled.GetDescription())
                      .ElseIF(s.Status == 4).Return(StockInStatus.task_working.GetDescription())
                      .End(StockInStatus.task_finish.GetDescription()),
                      s.PlanInQty,
                      s.ActInQty,
                      s.IsDel,
                      s.Remark,
                      CName = c.UserNickname,
                      s.CreateDate,
                      UName = u.UserNickname,
                      s.ModifiedDate
                  }).MergeTable().Where(c => c.StockInId == stockInId).OrderBy(c => c.CreateDate, OrderByType.Desc).ToList();
            if (!list1.Any())
            {
                flag1 = false;
            }
            if (!list2.Any())
            {
                flag2 = false;
            }
            var html = FileUtil.ReadFileFromPath(Path.Combine(_env.WebRootPath, "upload", "StockIn.html"));
            return (flag1, list1, flag2, list2, html).JilToJson();
        }

        public bool Auditin(long UserId, long stockInId)
        { 
            var flag = _client.Ado.UseTran(() =>
            { 
             
                //�޸���ϸ״̬ 2
                _client.Updateable(new Wms_stockindetail { Status = StockInStatus.task_confirm.ToByte(), ModifiedBy = UserId, ModifiedDate = DateTimeExt.DateTime }).UpdateColumns(c => new { c.Status, c.ModifiedBy, c.ModifiedDate })
                .Where(c => c.StockInId == stockInId && c.IsDel == 1).ExecuteCommand();
                //�޸������е�״̬��Ϊ������ 2
                _client.Updateable(new Wms_stockin { StockInId = stockInId, StockInStatus = StockInStatus.task_confirm.ToByte(), ModifiedBy = UserId, ModifiedDate = DateTimeExt.DateTime }).UpdateColumns(c => new { c.StockInStatus, c.ModifiedBy, c.ModifiedDate }).ExecuteCommand();
            }).IsSuccess;
            return flag;
        }
    }
}