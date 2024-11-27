using AutoMapper;
using Calmska.Api.DTO;
using Calmska.Models.Models;

namespace Calmska.Api.Helper
{
    public class MapperProfiles : Profile
    {
        public MapperProfiles()
        {
            CreateMap<Account, AccountDTO>();
            CreateMap<AccountDTO, Account>();
            CreateMap<Settings, SettingsDTO>();
            CreateMap<SettingsDTO, Settings>();
        }
    }
}
