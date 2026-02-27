using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IllnessesRecordingSystem.DB;
using IllnessesRecordingSystem.Models;
using IllnessesRecordingSystem.Views;
using MsBox.Avalonia;
using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia.Models;

namespace IllnessesRecordingSystem.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty] private ObservableCollection<IllnessRecordViem> _illnessRecords = new();

    [ObservableProperty] private int _currentPageSize;
    [ObservableProperty] private List<int> pageSizes;
    [ObservableProperty] private string _pageInfo;
    
    [ObservableProperty] private IllnessRecordViem _selectedIllnessRecord;

    private int _currentPage = 1;
    private int _totalPages;

    public MainWindowViewModel()
    {
        PageSizes = new List<int>([5, 10, 20]);
        CurrentPageSize = PageSizes.First();
        CalculatePages();
    }

    partial void OnCurrentPageSizeChanged(int value)
    {
        CalculatePages();
    }

    void CalculatePages()
    {
        using var db = new IllnessRecordRepository();
        var rowsCount = db.GetRowsCount();
        _totalPages = (int)Math.Ceiling((double)rowsCount / CurrentPageSize);
        if (_totalPages == 0) _totalPages = 1;
        ShowFirstPage();
    }

    void ShowPage(int pageIndex)
    {
        if (pageIndex < 1 || pageIndex > _totalPages) return;
        
        _currentPage = pageIndex;
        using var db = new IllnessRecordRepository();
        IllnessRecords.Clear();
        var rows = db.GetPage(pageIndex, CurrentPageSize);
        rows.ForEach(i => IllnessRecords.Add(i));
        PageInfo = $"Страница {_currentPage} из {_totalPages}";
    }

    [RelayCommand]
    public void ShowFirstPage()
    {
        ShowPage(1);
    }

    [RelayCommand]
    public void ShowLastPage()
    {
        ShowPage(_totalPages);
    }

    [RelayCommand]
    public void ShowNextPage()
    {
        if (_currentPage < _totalPages)
            ShowPage(_currentPage + 1);
    }

    [RelayCommand]
    public void ShowPreviousPage()
    {
        if (_currentPage > 1)
        {
            ShowPage(_currentPage - 1);
        }
    }

    [RelayCommand]
    public void RefreshData()
    {
        CalculatePages();
    }

    Window GetMain()
    {
        Window main = null;
        if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            main = desktop.MainWindow;
        return main;
    }

    [RelayCommand]
    public async void DeleteSelectedIllnessRecord()
    {
        /*var messageBox = MessageBoxManager
            .GetMessageBoxStandard("Подтверждение", $"Удалить запись о заболевании сотрудника {SelectedIllnessRecord.EmployeeName}?", ButtonEnum.YesNo);*/
        var messageBox = MessageBoxManager.GetMessageBoxCustom(
            new MessageBoxCustomParams
            {
                ContentTitle = "Подтверждение",
                ContentMessage = $"Удалить запись о заболевании сотрудника {SelectedIllnessRecord.EmployeeName}?",
                ButtonDefinitions = new List<ButtonDefinition>
                {
                    new ButtonDefinition {Name = "✅", IsDefault = true},
                    new ButtonDefinition {Name = "❌"}
                }
            });

        var result = await messageBox.ShowWindowDialogAsync(GetMain());

        if (result == "✅")
        {
            using var db = new IllnessRecordRepository();
            db.Delete(SelectedIllnessRecord);
            RefreshData();
        }
    }
    
    [RelayCommand]
    public void OpenAddWindow()
    {
        var window = new AddWindow();
        window.ShowDialog(App.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop ? desktop.MainWindow : null);

        window.Closed += (sender, args) =>
        {
            RefreshData();
        };
    }

}