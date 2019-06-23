using System;
using System.Collections.Generic;
using System.Text;
using YL.Core.Entity;

namespace YL.Core.Dto
{
    public class RoleMenuDto
    {
        public string RoleId { get; set; }

        public string RoleName { get; set; }

        public string RoleType { get; set; }
        public string Remark { get; set; }

        public List<RoleMenuItemDto> Children { get; set; }
    }

    public class RoleMenuItemDto
    {
        public string RoleId { get; set; }
         
        public string WarehouseId { get; set; }
         
        public string MenuId { get; set; }
    }
}