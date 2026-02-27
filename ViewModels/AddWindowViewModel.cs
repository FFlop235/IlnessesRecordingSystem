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
    
    [RelayCommand]
    public void AddRecord()
    {
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