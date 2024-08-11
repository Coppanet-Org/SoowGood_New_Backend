using SoowGoodWeb.Enums;
using Volo.Abp.Application.Dtos;

namespace SoowGoodWeb.DtoModels
{
    public class PlatformPackageDto : FullAuditedEntityDto<long>
    {
        public string? PackageTitle { get; set; }
        public string? PackageName { get; set; }
        public string? PackageDescription { get; set; }
        public string? PackageFacilities { get; set; }
        public string? Reason { get; set; }
        public decimal? Price { get; set; }
        public long? PackageProviderId { get; set; }
        public string? DoctorName { get; set; }
        public DoctorTitle? DoctorTitle { get; set; }
        public string? DoctorTitleName { get; set; }

    }
}


