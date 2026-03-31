using System.Collections.ObjectModel;
using Calmska.Application.DTO;
using Calmska.Helper;
using Calmska.Services.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Calmska.ViewModels
{
    internal partial class TipsListViewModel : ObservableObject, IQueryAttributable
    {
        [ObservableProperty]
        private ObservableCollection<TipsExpandableItem> _tips;
        [ObservableProperty]
        private string _title = string.Empty;
        private TipsExpandableItem _previouslyExpandedItem;
        private TipsExpandableItem _selectedTip = new();
        public TipsExpandableItem SelectedTip
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
        private CancellationTokenSource _cts;

        public TipsListViewModel(IService<TipsDTO> tipsService)
        {
            _tipsService = tipsService;
        }
        
        [RelayCommand]
        public void OnDisappearing()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
        }
        
        private async Task LoadTips()
        {
            try
            {
                if (_tipType == null || _tipType?.TypeId <= 0)
                {
                    await Shell.Current.DisplayAlertAsync("Warning", $"Couldn't find tips for the provided type.", "Close");
                    return;
                }
                Title = _tipType?.Type ?? "";
                var tips = await _tipsService.SearchAllByArgumentAsync(new TipsDTO { TipsTypeId = _tipType?.TypeId, Content = null }, null, null, _cts.Token);
                if (tips != null && string.IsNullOrEmpty(tips.Error) && tips.Result != null)
                {
                    ObservableCollection<TipsExpandableItem> tempTips = new();
                    foreach (var t in tips.Result.Items)
                    {
                        tempTips.Add(new TipsExpandableItem
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
                await Shell.Current.DisplayAlertAsync("Warning", $"Loading tips list error: {ex.Message}\n{ex.InnerException}", "Close");
            }
        }
        public async void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = new CancellationTokenSource();
            
            try
            {
                if (query.TryGetValue("tipType", out var value))
                {
                    _tipType = (Types_TipsFrontendDTO?)value;
                    if(_tipType != null && _tipType.TypeId > 0)
                        await LoadTips();
                }
            }catch(Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Warning", $"Loading tips list page error: {ex.Message}\n{ex.InnerException}", "Close");
            }
        }
    }
}
