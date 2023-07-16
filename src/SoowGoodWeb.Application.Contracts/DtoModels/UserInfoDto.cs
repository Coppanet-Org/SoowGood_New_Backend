using System;
using System.Collections.Generic;
using System.Text;

namespace SoowGoodWeb.DtoModels
{
    public class UserInfoDto
    {
        public Guid? TenantId { get; set; }

        public string UserName { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public string Email { get; set; }

        public bool EmailConfirmed { get; set; }

        public string PhoneNumber { get; set; }

        public bool PhoneNumberConfirmed { get; set; }

        public bool IsActive { get; set; }

        public bool LockoutEnabled { get; set; }

        public DateTimeOffset? LockoutEnd { get; set; }

        public string ConcurrencyStamp { get; set; }

        //public int EntityVersion { get; set; }
        //public string UserName { get; set; }
        //public string Name { get; set; }
        //public string Surname { get; set; }
        //public string Email { get; set; }
        //public string PhoneNumber { get; set; }
        //public bool IsActive { get; set; }
        //public IList<string> RoleNames { get; set; }
        //public string Password { get; set; }
    }

    public class UserRegInfoDto
    {
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsActive { get; set; }
        public bool lockoutEnabled { get; set; }
        public IList<string> RoleNames { get; set; }
        public string Password { get; set; }
    }
}
