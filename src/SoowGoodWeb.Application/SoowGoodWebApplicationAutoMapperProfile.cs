using AutoMapper;
using SoowGoodWeb.DtoModels;
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
    }
}
