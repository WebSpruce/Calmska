﻿using AutoMapper;
using Calmska.Models.DTO;
using Calmska.Models.Models;

namespace Calmska.Api.Helper
{
    public class MapperProfiles : Profile
    {
        public MapperProfiles()
        {
            CreateMap<Account, AccountDTO>();
            CreateMap<AccountDTO, Account>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId ?? Guid.NewGuid()))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.UserName) ? string.Empty : src.UserName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.Email) ? string.Empty : src.Email))
            .ForMember(dest => dest.PasswordHashed, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.PasswordHashed) ? string.Empty : src.PasswordHashed));


            CreateMap<MoodDTO, Mood>();
            CreateMap<Mood, MoodDTO>();

            CreateMap<MoodHistoryDTO, MoodHistory>();
            CreateMap<MoodHistory, MoodHistoryDTO>();

            CreateMap<TipsDTO, Tips>();
            CreateMap<Tips, TipsDTO>();
            
            CreateMap<Types_TipsDTO, Types_Tips>();
            CreateMap<Types_Tips, Types_TipsDTO>();

            CreateMap<Types_MoodDTO, Types_Mood>();
            CreateMap<Types_Mood, Types_MoodDTO>();

            CreateMap<Settings, SettingsDTO>()
            .ForMember(dest => dest.PomodoroTimer, opt => opt.MapFrom(src => ConvertToFloat(src.PomodoroTimer)))
            .ForMember(dest => dest.PomodoroBreak, opt => opt.MapFrom(src => ConvertToFloat(src.PomodoroBreak)));

            CreateMap<SettingsDTO, Settings>()
                .ForMember(dest => dest.PomodoroTimer, opt => opt.MapFrom(src => !string.IsNullOrEmpty(src.PomodoroTimer) ? src.PomodoroTimer : string.Empty))
                .ForMember(dest => dest.PomodoroBreak, opt => opt.MapFrom(src => !string.IsNullOrEmpty(src.PomodoroBreak) ? src.PomodoroBreak : string.Empty));
        }
        private static float? ConvertToFloat(string value)
        {
            if (float.TryParse(value, out var result))
            {
                return result;
            }
            return null;
        }
    }
}
