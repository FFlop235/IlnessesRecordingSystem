using System;
using CommunityToolkit.Mvvm.ComponentModel;
using IllnessesRecordingSystem.Models;

namespace IllnessesRecordingSystem.ViewModels;

public partial class EditRecordViewModel: ViewModelBase
{
    [ObservableProperty]
    IllnessRecordViem _selectedIllnessRecord;

    private Action<object?> close;

    public EditRecordViewModel(IllnessRecordViem selectedIllnessRecord, Action<object?> close)
    {
        SelectedIllnessRecord = selectedIllnessRecord;
        this.close = close;
    }
}