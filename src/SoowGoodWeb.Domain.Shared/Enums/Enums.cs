using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Linq;

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
    public enum DoctorTitle
    {
        [Display(Name = "Dr.")]
        Dr = 1,
        [Display(Name = "Asst. Prof. Dr.")]
        AsstProfDr =2,
        [Display(Name = "Assc. Prof. Dr.")]
        AssocProfDr =3,
        [Display(Name = "Prof. Dr.")]
        ProfDr=4
    }
    //Document Attachment
    public enum EntityType
    {
        //None = 0,
        Doctor = 1,
        Agent = 2,
        Patient = 3
    }
    public enum AttachmentType
    {
        //None = 0,
        DegreeDoc = 1,
        IdentityDoc = 2,
        SpecialityDoc = 3,        
    }
}
