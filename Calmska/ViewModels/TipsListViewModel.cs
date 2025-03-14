using Calmska.Helper;
using Calmska.Models.DTO;
using Calmska.Services.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Calmska.ViewModels
{
    internal partial class TipsListViewModel : ObservableObject, IQueryAttributable
    {
        [ObservableProperty]
        private List<ExpandableItem<TipsDTO?>> _tips;
        [ObservableProperty]
        private string _title = string.Empty;
        private ExpandableItem<TipsDTO> _previouslyExpandedItem;
        private ExpandableItem<TipsDTO?> _selectedTip = new();
        public ExpandableItem<TipsDTO?> SelectedTip
        {
            get => _selectedTip;
            set
            {
                if (value != null)
                {
                    if (_previouslyExpandedItem != null && _previouslyExpandedItem != value) //Collapse previously expanded item
                    {
                        _previouslyExpandedItem.IsExpanded = false;
                        _previouslyExpandedItem.IsDefaultTextVisible = true;
                        _previouslyExpandedItem.BorderBGColor = Color.FromArgb("#538A5E");
                    }

                    value.IsExpanded = !value.IsExpanded;

                    if (value.IsExpanded)
                    {
                        _previouslyExpandedItem = value;
                        value.IsDefaultTextVisible = false;
                        value.BorderBGColor = Color.FromArgb("#D4A373");
                    }
                    else
                    {
                        value.IsDefaultTextVisible = true;
                        value.BorderBGColor = Color.FromArgb("#538A5E");
                    }

                    _selectedTip = value;
                    OnPropertyChanged(nameof(SelectedTip));

                    MainThread.BeginInvokeOnMainThread(() => { //Clear selection to allow reselection
                        _selectedTip = null;
                        OnPropertyChanged(nameof(SelectedTip));
                    });
                }
            }
        }

        private Types_TipsFrontendDTO? _tipType = null;
        private readonly IService<TipsDTO> _tipsService;

        public TipsListViewModel(IService<TipsDTO> tipsService)
        {
            _tipsService = tipsService;
        }
        private async Task LoadTips()
        {
            try
            {
                if (_tipType == null || _tipType?.TypeId <= 0)
                {
                    await Shell.Current.DisplayAlert("Warning", $"Couldn't find tips for the provided type.", "Close");
                    return;
                }
                Title = _tipType?.Type;
                var tips = await _tipsService.SearchAllByArgumentAsync(new TipsDTO { TipsTypeId = _tipType?.TypeId, Content = null }, null, null);
                if (tips != null && string.IsNullOrEmpty(tips.Error) && tips.Result != null)
                {
                    List<ExpandableItem<TipsDTO?>> tempTips = new();
                    foreach (var t in tips.Result.Items)
                    {
                        tempTips.Add(new ExpandableItem<TipsDTO?>
                        {
                            Data = t,
                            IsExpanded = false,
                            IsDefaultTextVisible = true,
                            BorderBGColor = Color.FromArgb("#538A5E")
                        });
                    }
                    Tips = tempTips;
                }
            }
            catch(Exception ex)
            {
                await Shell.Current.DisplayAlert("Warning", $"Loading tips list error: {ex.Message}\n{ex.InnerException}", "Close");
            }
        }
        public async void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            try
            {
                if (query.ContainsKey("tipType"))
                {
                    _tipType = (Types_TipsFrontendDTO?)query["tipType"];
                    if(_tipType != null && _tipType.TypeId > 0)
                        await LoadTips();
                }
            }catch(Exception ex)
            {
                await Shell.Current.DisplayAlert("Warning", $"Loading tips list page error: {ex.Message}\n{ex.InnerException}", "Close");
            }
        }
    }
}
