using System;
using System.Collections.Generic;
using System.Text;

namespace SoowGoodWeb.Enums
{
    public enum OtpStatus
    {
        New = 0,
        Send = 1,
        Varified = 2,
        Cancel = 3,
        TimeExpired = 4,
    }
    public enum Gender
    {
        Male = 1,
        Female = 2,
        Others = 3
    }
    public enum MaritalStatus
    {
        Single = 1,
        Maried = 2
    }
}
