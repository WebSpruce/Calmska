using Calmska.Models.DTO;
using Calmska.Services.Interfaces;
using Calmska.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;

namespace Calmska.ViewModels
{
    internal partial class TipsViewModel : ObservableObject, IQueryAttributable
    {
        [ObservableProperty]
        private List<Types_TipsFrontendDTO> _types = new();
        [ObservableProperty]
        private Types_TipsFrontendDTO _selectedType = new();

        private readonly ITypesService<Types_TipsDTO> _typesTipsService;
        public TipsViewModel(ITypesService<Types_TipsDTO> typesTipsService)
        {
            _typesTipsService = typesTipsService;

            Task.Run(async () => await LoadTypes());
        }
        private async Task LoadTypes()
        {
            try
            {
                var types = await _typesTipsService.GetAllAsync(null, null);
                if (types != null && string.IsNullOrEmpty(types.Error) && types.Result != null)
                {
                    List<Types_TipsFrontendDTO> typesTemp = new List<Types_TipsFrontendDTO>();
                    foreach (var type in types.Result.Items)
                    {
                        string icon = string.Empty;

                        if (type.TypeId == 1)
                            icon = "\ue30a";
                        if (type.TypeId == 2)
                            icon = "\ue407";
                        if (type.TypeId == 3)
                            icon = "\ue8df";
                        if (type.TypeId == 4)
                            icon = "\ue406";
                        if (type.TypeId == 5)
                            icon = "\ue88a";
                        if (type.TypeId == 6)
                            icon = "\ue8f9";
                        if (type.TypeId == 7)
                            icon = "\ue7ef";
                        if (type.TypeId == 8)
                            icon = "\ue2db";
                        if (type.TypeId == 9)
                            icon = "\ue91d";

                        var type_tip = new Types_TipsFrontendDTO { Type = type.Type, TypeId = type.TypeId, IconName = icon };
                        typesTemp.Add(type_tip);
                    }
                    typesTemp = typesTemp.OrderBy(t => t.Type).ToList();
                    Types = typesTemp;
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Warning", $"Loading types of tips error: {ex.Message}\n{ex.InnerException}", "Close");
            }
            
        }
        [RelayCommand]
        private async Task TypeSelected()
        {
            try
            {
                if(SelectedType.TypeId != null && SelectedType.TypeId > 0)
                {
                    await Shell.Current.GoToAsync($"{nameof(TipsListPage)}", new Dictionary<string, object>
                    {
                        { "tipType", SelectedType ?? new Types_TipsFrontendDTO() },
                    });
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Warning", $"Changing page error: {ex.Message}\n{ex.InnerException}", "Close");
            }
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            SelectedType = new();
        }
    }
}
