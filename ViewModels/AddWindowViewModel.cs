using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IllnessesRecordingSystem.DB;
using IllnessesRecordingSystem.Models;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace IllnessesRecordingSystem.ViewModels;

public partial class AddWindowViewModel: ObservableObject
{
    private EmployeeRepository _employeeRepository;
    private IllnessTypeRepository _illnessTypeRepository;
    private IllnessRecordRepository _illnessRecordRepository;
    private DepartmentRepository _departmentRepository;
    
    [ObservableProperty]
    private List<Employee> _employees;
    
    [ObservableProperty]
    private List<IllnessType> _illnessTypes;
    
    [ObservableProperty]
    private Employee? _selectedEmployee;
    
    [ObservableProperty]
    private IllnessType? _selectedIllnessType;
    
    [ObservableProperty]
    private DateTimeOffset _startDate = DateTimeOffset.Now.Date;
    
    [ObservableProperty]
    private DateTimeOffset _endDate = DateTimeOffset.Now.Date.AddDays(7);

    [ObservableProperty]
    private string _diagnosisNote;

    public AddWindowViewModel()
    {
        _illnessRecordRepository = new IllnessRecordRepository();
        _departmentRepository = new DepartmentRepository();
        
        _employeeRepository = new EmployeeRepository();
        Employees = _employeeRepository.GetAll();
        
        _illnessTypeRepository = new IllnessTypeRepository();
        IllnessTypes = _illnessTypeRepository.GetAll();
        
    }

    [RelayCommand]
    public void Close()
    {
        if (App.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            foreach (var window in desktop.Windows)
            {
                if (window.DataContext == this)
                {
                    window.Close();
                    break;
                }
            }
        }
    }
    
    Window GetWindow()
    {
        if (App.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            foreach (var window in desktop.Windows)
            {
                if (window.DataContext == this)
                {
                    return window;
                }
            }
        }

        return null;
    }
    
    [RelayCommand]
    public async void AddRecord()
    {
        if (SelectedEmployee == null || string.IsNullOrWhiteSpace(SelectedEmployee.FullName))
        {
            var errorBox = MessageBoxManager
                .GetMessageBoxStandard("Ошибка", "Укажите ФИО сотрудника", ButtonEnum.Ok);
            await errorBox.ShowWindowDialogAsync(GetWindow());
            return;
        }
        
        if (SelectedIllnessType == null || string.IsNullOrWhiteSpace(SelectedIllnessType.Name))
        {
            var errorBox = MessageBoxManager
                .GetMessageBoxStandard("Ошибка", "Укажите тип болезни", ButtonEnum.Ok);
            await errorBox.ShowWindowDialogAsync(GetWindow());
            return;
        }

        if (EndDate < StartDate)
        {
            var errorBox = MessageBoxManager
                .GetMessageBoxStandard("Ошибка", "Дата окончания не может быть раньше даты начала", ButtonEnum.Ok);
            await errorBox.ShowWindowDialogAsync(GetWindow());
            return;
        }

        var AddRecord = new IllnessRecordViem
        {
            Id = _illnessRecordRepository.GetRowsCount() + 1,
            EmployeeName = SelectedEmployee.FullName,
            DepartmentName = _departmentRepository.GetById(SelectedEmployee.DepartmentId).Name,
            IllnessType = SelectedIllnessType.Name,
            StartDate = StartDate.DateTime,
            EndDate = EndDate.DateTime,
            DurationDays = (EndDate.DateTime - StartDate.DateTime).Days,
            DiagnosisNote = DiagnosisNote,
        };
        
        _illnessRecordRepository.Add(AddRecord);
        
        if (App.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            foreach (var window in desktop.Windows)
            {
                if (window.DataContext == this)
                {
                    window.Close();
                    break;
                }
            }
        }
    }
}