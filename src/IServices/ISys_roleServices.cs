using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;
using YL.Core.Dto;
using YL.Core.Entity;
using YL.Utils.Table;

namespace IServices
{
    public interface ISys_roleServices : IBaseServices<Sys_role>
    {
        string PageList(Bootstrap.BootstrapParams bootstrap);

        List<PermissionMenu> GetMenu();

        List<PermissionMenu> GetMenu(long warehouseId,long roleId, string menuType = "menu");

        DbResult<bool> Insert(Sys_role role, long userId, string[] menuId);

        DbResult<bool> Update(Sys_role role, long userId, string[] menuId);
    }
}