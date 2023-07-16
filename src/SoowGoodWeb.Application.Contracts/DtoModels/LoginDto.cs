using System;
using System.Collections.Generic;
using System.Text;

namespace SoowGoodWeb.DtoModels
{
    public class LoginDto
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string password { get; set; }
        public bool RememberMe { get; set; }
    }
}
