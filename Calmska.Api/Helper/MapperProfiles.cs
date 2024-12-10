using AutoMapper;
using Calmska.Models.DTO;
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
            CreateMap<MoodDTO, Mood>();
            CreateMap<Mood, MoodDTO>();
            CreateMap<MoodHistoryDTO, MoodHistory>();
            CreateMap<MoodHistory, MoodHistoryDTO>();
            CreateMap<TipsDTO, Tips>();
            CreateMap<Tips, TipsDTO>();
        }
    }
}
