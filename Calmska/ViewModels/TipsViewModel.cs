using Calmska.Models.DTO;
using Calmska.Services.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;

namespace Calmska.ViewModels
{
    internal partial class TipsViewModel : ObservableObject
    {
        [ObservableProperty]
        private List<Types_TipsFrontendDTO> _types = new();
        
        private Types_TipsFrontendDTO _selectedType = new();
        public Types_TipsFrontendDTO SelectedType
        {
            get => _selectedType;
            set
            {
                if(_selectedType != value)
                {
                    _selectedType = value;
                    Debug.WriteLine($"something: {_selectedType}");
                    OnPropertyChanged();
                }
            }
        }

        private readonly IService<TipsDTO> _tipsService;
        private readonly ITypesService<Types_TipsDTO> _typesTipsService;
        public TipsViewModel(IService<TipsDTO> tipsService, ITypesService<Types_TipsDTO> typesTipsService)
        {
            _tipsService = tipsService;
            _typesTipsService = typesTipsService;

            Task.Run(async () => await LoadTypes());
        }
        private async Task LoadTypes()
        {
            var types = await _typesTipsService.GetAllAsync(null, null);
            if (types != null && string.IsNullOrEmpty(types.Error) && types.Result != null)
            {
                List<Types_TipsFrontendDTO> typesTemp = new List<Types_TipsFrontendDTO>();
                foreach (var type in types.Result.Items)
                {
                    string icon = string.Empty;

                    if(type.TypeId == 1)
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
                Types = typesTemp;
            }
        }
        [RelayCommand]
        private void TypeSelected()
        {
            
        }
    }
}
