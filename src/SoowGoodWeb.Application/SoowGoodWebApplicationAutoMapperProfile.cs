using AutoMapper;
using SoowGoodWeb.DtoModels;
using SoowGoodWeb.InputDto;
using SoowGoodWeb.Models;
using Volo.Abp.Identity;

namespace SoowGoodWeb;

public class SoowGoodWebApplicationAutoMapperProfile : Profile
{
    public SoowGoodWebApplicationAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */

        CreateMap<IdentityUser, UserInfoDto>();
        CreateMap<UserInfoDto, IdentityUser>();

        //CreateMap<TestEntity, TestEntityDto>();
        //CreateMap<TestEntityDto, TestEntity>();
        //CreateMap<TestEntityInputDto, TestEntity>();
        //.ForMember(d => d.Name, o => o.MapFrom(s => s.DistrictNameEng));

        CreateMap<Degree, DegreeDto>();
        CreateMap<DegreeDto, Degree>();
        CreateMap<DegreeInputDto, Degree>();

        CreateMap<DoctorDegree, DoctorDegreeDto>()
            .ForMember(d => d.DoctorName, o => o.MapFrom(n => (n.DoctorProfileId > 0 && n.DoctorProfile != null) ? n.DoctorProfile.FullName:""))
            .ForMember(d => d.DegreeName, o => o.MapFrom(e => (e.DegreeId > 0 && e.Degree != null) ? e.Degree.DegreeName:""));
        CreateMap<DoctorDegreeDto, DoctorDegree>()
            .ForMember(d => d.Degree, o => o.Ignore())
            .ForMember(d => d.DoctorProfile, o => o.Ignore());
        CreateMap<DoctorDegreeInputDto, DoctorDegree>()
            .ForMember(d => d.Degree, o => o.Ignore())
            .ForMember(d => d.DoctorProfile, o => o.Ignore());

        CreateMap<Speciality, SpecialityDto>();
        CreateMap<SpecialityDto, Speciality>()
            .ForMember(d => d.Specializations, o => o.Ignore());
        CreateMap<SpecialityInputDto, Speciality>()
            .ForMember(d => d.Specializations, o => o.Ignore());

        CreateMap<Specialization, SpecializationDto>()
            .ForMember(d => d.SpecialityName, o => o.MapFrom(n => n.SpecialityId > 0 && n.Speciality != null ? n.Speciality.SpecialityName : ""));
        CreateMap<SpecializationDto, Specialization>()
            .ForMember(d => d.Speciality, o => o.Ignore());
        CreateMap<SpecializationInputDto, Specialization>()
            .ForMember(d => d.Speciality, o => o.Ignore());

        CreateMap<DoctorSpecialization, DoctorSpecializationDto>()
            .ForMember(d => d.DoctorName, o => o.MapFrom(n => (n.DoctorProfileId > 0 && n.DoctorProfile != null) ? n.DoctorProfile.FullName : ""))
            .ForMember(d => d.SpecialityName, o => o.MapFrom(n => (n.SpecialityId > 0 && n.Speciality != null) ? n.Speciality.SpecialityName : ""))
            .ForMember(d => d.SpecializationName, o => o.MapFrom(n => (n.SpecializationId > 0 && n.Specialization != null) ? n.Specialization.SpecializationName : ""));
        CreateMap<DoctorSpecializationDto, DoctorSpecialization>()
            .ForMember(d => d.DoctorProfile, o => o.Ignore())
            .ForMember(d => d.Speciality, o => o.Ignore())
            .ForMember(d => d.Specialization, o => o.Ignore());
        CreateMap<DoctorSpecializationInputDto, DoctorSpecialization>()
            .ForMember(d => d.DoctorProfile, o => o.Ignore())
            .ForMember(d => d.Speciality, o => o.Ignore())
            .ForMember(d => d.Specialization, o => o.Ignore());

        CreateMap<DoctorProfile, DoctorProfileDto>()
            .ForMember(d => d.DoctorTitleName, o => o.MapFrom(n => n.DoctorTitle > 0 ? Utilities.Utility.GetDisplayName(n.DoctorTitle) : ""))
            .ForMember(d => d.GenderName, o => o.MapFrom(n => n.Gender > 0 ? n.Gender : 0))
            .ForMember(d => d.MaritalStatusName, o => o.MapFrom(n => n.MaritalStatus > 0 ? n.MaritalStatus : 0))
            .ForMember(d => d.SpecialityName, o => o.MapFrom(n => n.SpecialityId > 0 && n.Speciality != null ? n.Speciality.SpecialityName : ""));
        CreateMap<DoctorProfileDto, DoctorProfile>()
            .ForMember(d => d.Speciality, o => o.Ignore())
            .ForMember(d => d.Degrees, o => o.Ignore())
            .ForMember(d => d.DoctorSpecialization, o => o.Ignore());
        CreateMap<DoctorProfileInputDto, DoctorProfile>()
            .ForMember(d => d.Speciality, o => o.Ignore())
            .ForMember(d => d.Degrees, o => o.Ignore())
            .ForMember(d => d.DoctorSpecialization, o => o.Ignore());

        CreateMap<DoctorChamber, DoctorChamberDto>();
        CreateMap<DoctorChamberDto, DoctorChamber>()
            .ForMember(d => d.DoctorProfile, o => o.Ignore());
        CreateMap<DoctorChamberInputDto, DoctorChamber>()
            .ForMember(d => d.DoctorProfile, o => o.Ignore());

        CreateMap<DocumentsAttachment, DocumentsAttachmentDto>();
            //.ForMember(d => d.EntityTypeName, o => o.MapFrom(n => n.EntityType))
            //.ForMember(d => d.AttachmentTypeName, o => o.MapFrom(n => n.AttachmentType));
        CreateMap<DocumentsAttachmentDto, DocumentsAttachment>();

        CreateMap<DoctorSchedule, DoctorScheduleDto>()
            .ForMember(d => d.DoctorName, o => o.MapFrom(n => (n.DoctorProfileId > 0 && n.DoctorProfile != null) ? n.DoctorProfile.FullName : ""))
            .ForMember(d => d.ScheduleTypeName, o => o.MapFrom(e => (e.ScheduleType > 0 && e.ScheduleType != null) ? e.ScheduleType : 0))
            .ForMember(d => d.ConsultancyTypeName, o => o.MapFrom(e => (e.ConsultancyType > 0 && e.ConsultancyType != null) ? e.ConsultancyType : 0))
            .ForMember(d => d.Chamber, o => o.MapFrom(e => (e.DoctorChamberId > 0 && e.DoctorChamber != null) ? e.DoctorChamber.ChamberName : ""));
            ;
        CreateMap<DoctorScheduleDto, DoctorSchedule>()
            .ForMember(d => d.DoctorProfile, o => o.Ignore());
        CreateMap<DoctorScheduleInputDto, DoctorSchedule>()
            .ForMember(d => d.DoctorProfile, o => o.Ignore());

        //CreateMap<DoctorScheduledDayOff, DoctorScheduledDayOffDto>();
        //CreateMap<DoctorScheduledDayOffDto, DoctorScheduledDayOff>()
        //    .ForMember(d => d.DoctorSchedule, o => o.Ignore());
        //CreateMap<DoctorScheduledDayOffInputDto, DoctorScheduledDayOff>()
        //    .ForMember(d => d.DoctorSchedule, o => o.Ignore());

        CreateMap<DoctorScheduleDaySession, DoctorScheduleDaySessionDto>();
        CreateMap<DoctorScheduleDaySessionDto, DoctorScheduleDaySession>()
            .ForMember(d => d.DoctorSchedule, o => o.Ignore());
        CreateMap<DoctorScheduleDaySessionInputDto, DoctorScheduleDaySession>()
            .ForMember(d => d.DoctorSchedule, o => o.Ignore());

        CreateMap<DoctorFeesSetup, DoctorFeesSetupDto>()
            .ForMember(d => d.AppointmentTypeName, o => o.MapFrom(e => (e.AppointmentType > 0 && e.AppointmentType != null) ? e.AppointmentType : 0));
        CreateMap<DoctorFeesSetupDto, DoctorFeesSetup>()
            .ForMember(d => d.DoctorSchedule, o => o.Ignore());
        CreateMap<DoctorFeesSetupInputDto, DoctorFeesSetup>()
            .ForMember(d => d.DoctorSchedule, o => o.Ignore());
    }
}
