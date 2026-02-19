using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IlnessesRecordingSystem.DB;
using IlnessesRecordingSystem.Models;

namespace IlnessesRecordingSystem.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private ObservableCollection<IllnessRecordViem> _illnessRecords = new();

    [ObservableProperty] private int _currentPageSize;
    [ObservableProperty] private List<int> pageSizes;
    [ObservableProperty] private string pageInfo;
    
    private int currentPage = 1;
    private int totalPages;

    public MainWindowViewModel()
    {
        PageSizes = new List<int>([5, 10, 20]);
        CurrentPageSize = PageSizes.First();
    }

    partial void OnCurrentPageSizeChanged(int value)
    {
        CalculatePages();
    }

    void CalculatePages()
    {
        using var db = new IllnessRecordRepository();
        var rowsCount = db.GetRowsCount();
        totalPages = (int)Math.Ceiling((double)rowsCount / CurrentPageSize);
        ShowFirstPage();
    }

    void ShowPage(int pageIndex)
    {
        currentPage = pageIndex;
        using var db = new IllnessRecordRepository();
        IllnessRecords.Clear();
        var rows = db.GetPage(pageIndex, CurrentPageSize);
        rows.ForEach(i => IllnessRecords.Add(i));
        pageInfo = $"Страница {currentPage} из {totalPages}";
    }

    [RelayCommand]
    private void ShowFirstPage()
    {
        ShowPage(1);
    }

    [RelayCommand]
    private void ShowLastPage()
    {
        ShowPage(totalPages);
    }

    [RelayCommand]
    private void ShowNextPage()
    {
        if (currentPage < totalPages - 1)
            ShowPage(currentPage + 1);
    }
    
    [RelayCommand]
    private void ShowPreviousPage()
    {
        if (currentPage > 1)
        {
            ShowPage(currentPage - 1);
        }
    }
}