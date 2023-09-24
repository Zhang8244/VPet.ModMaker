﻿using HKW.HKWViewModels.SimpleObservable;
using LinePutScript.Localization.WPF;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VPet.ModMaker.Models;
using VPet.ModMaker.Views.ModEdit.WorkEdit;

namespace VPet.ModMaker.ViewModels.ModEdit.WorkEdit;

public class WorkPageVM
{
    #region Value
    public ObservableValue<ObservableCollection<WorkModel>> ShowWorks { get; } = new();
    public ObservableCollection<WorkModel> Works => CurrentPet.Value.Works;

    public ObservableCollection<PetModel> Pets => ModInfoModel.Current.Pets;
    public ObservableValue<PetModel> CurrentPet { get; } = new(new());
    public ObservableValue<string> Search { get; } = new();
    #endregion
    #region Command
    public ObservableCommand AddCommand { get; } = new();
    public ObservableCommand<WorkModel> EditCommand { get; } = new();
    public ObservableCommand<WorkModel> RemoveCommand { get; } = new();
    #endregion
    public WorkPageVM()
    {
        ShowWorks.Value = Works;
        CurrentPet.ValueChanged += CurrentPet_ValueChanged;
        Search.ValueChanged += Search_ValueChanged;

        AddCommand.ExecuteEvent += Add;
        EditCommand.ExecuteEvent += Edit;
        RemoveCommand.ExecuteEvent += Remove;
    }

    private void CurrentPet_ValueChanged(PetModel oldValue, PetModel newValue)
    {
        ShowWorks.Value = newValue.Works;
    }

    private void Search_ValueChanged(string oldValue, string newValue)
    {
        if (string.IsNullOrWhiteSpace(newValue))
        {
            ShowWorks.Value = Works;
        }
        else
        {
            ShowWorks.Value = new(
                Works.Where(m => m.Id.Value.Contains(newValue, StringComparison.OrdinalIgnoreCase))
            );
        }
    }

    public void Close() { }

    private void Add()
    {
        var window = new WorkEditWindow();
        var vm = window.ViewModel;
        vm.CurrentPet = CurrentPet.Value;
        window.ShowDialog();
        if (window.IsCancel)
            return;
        Works.Add(vm.Work.Value);
    }

    public void Edit(WorkModel model)
    {
        var window = new WorkEditWindow();
        var vm = window.ViewModel;
        vm.CurrentPet = CurrentPet.Value;
        vm.OldWork = model;
        var newWork = vm.Work.Value = new(model);
        window.ShowDialog();
        if (window.IsCancel)
            return;
        if (ShowWorks.Value.Count == Works.Count)
        {
            Works[Works.IndexOf(model)] = newWork;
        }
        else
        {
            Works[Works.IndexOf(model)] = newWork;
            ShowWorks.Value[ShowWorks.Value.IndexOf(model)] = newWork;
        }
    }

    private void Remove(WorkModel food)
    {
        if (MessageBox.Show("确定删除吗".Translate(), "", MessageBoxButton.YesNo) is MessageBoxResult.No)
            return;
        if (ShowWorks.Value.Count == Works.Count)
        {
            Works.Remove(food);
        }
        else
        {
            ShowWorks.Value.Remove(food);
            Works.Remove(food);
        }
    }
}
