using AutoMapper;
using Calmska.Domain.Entities;
using Calmska.Domain.Filters;
using Calmska.Infrastructure.Persistence.Models;

namespace Calmska.Infrastructure.Mapping;

public class MapperProfiles : Profile
{
    public MapperProfiles()
    {
        CreateMap<AccountDocument, Account>()
            .ConstructUsing(doc => new Account()
            {
                UserId = doc.UserId ?? Guid.NewGuid(),
                UserName = doc.UserName ?? "", 
                Email = doc.Email ?? "",
                PasswordHashed = doc.PasswordHashed ?? ""
            });
        CreateMap<Account, AccountDocument>();

        CreateMap<MoodDocument, Mood>()
            .ConstructUsing(doc => new Mood()
            {
                MoodId = doc.MoodId,
                MoodName = doc.MoodName ?? "",
                MoodTypeId = doc.MoodTypeId
            });
        CreateMap<Mood, MoodDocument>();
        
        CreateMap<MoodHistoryDocument, MoodHistory>()
            .ConstructUsing(doc => new MoodHistory()
            {
                MoodHistoryId = doc.MoodHistoryId,
                Date = doc.Date,
                MoodId = doc.MoodId,
                UserId = doc.UserId
            });
        CreateMap<MoodHistory, MoodHistoryDocument>();
        
        CreateMap<SettingsDocument, Settings>()
            .ConstructUsing(doc => new Settings()
            {
                SettingsId = doc.SettingsId,
                Color = doc.Color ?? "",
                UserId = doc.UserId,
                PomodoroBreak = doc.PomodoroBreak ?? "",
                PomodoroTimer = doc.PomodoroTimer ?? ""
            });
        CreateMap<Settings, SettingsDocument>();
        
        CreateMap<TipsDocument, Tips>()
            .ConstructUsing(doc => new Tips()
            {
                TipId = doc.TipId,
                Content = doc.Content,
                TipsTypeId = doc.TipsTypeId
            });
        CreateMap<Tips, TipsDocument>();
        
        CreateMap<Types_MoodDocument, Types_Mood>()
            .ConstructUsing(doc => new Types_Mood()
            {
                TypeId = doc.TypeId,
                Type = doc.Type
            });
        CreateMap<Types_Mood, Types_MoodDocument>();
        
        CreateMap<Types_TipsDocument, Types_Tips>()
            .ConstructUsing(doc => new Types_Tips()
            {
                TypeId = doc.TypeId,
                Type = doc.Type
            });
        CreateMap<Types_Tips, Types_TipsDocument>();

    }
}