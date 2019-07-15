using System;

namespace YL.Core.Dto
{
    public class SysUserDto
    {
        public long UserId { get; set; }
        public long? RoleId { get; set; }
        public string UserNickname { get; set; }
        public string HeadImg { get; set; }
        public string Pwd { get; set; }

        public string UserName { get; set; }
        public string CName { get; set; }
        public string UName { get; set; }

        public string Email { get; set; }

        public string Tel { get; set; }

        public string Mobile { get; set; }

        public int? IsEabled { get; set; }

        public string LoginIp { get; set; }

        public DateTime? LoginDate { get; set; }

        public int? LoginTime { get; set; }

        public int? Sex { get; set; }

        public int? IsDel { get; set; }

        public string Remark { get; set; }

        public long? CreateBy { get; set; }

        public DateTime? CreateDate { get; set; }

        public long? ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }
    }
}