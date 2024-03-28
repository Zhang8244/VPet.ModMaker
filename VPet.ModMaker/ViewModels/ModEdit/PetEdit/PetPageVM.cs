﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using HKW.HKWUtils.Observable;
using LinePutScript.Localization.WPF;
using VPet.ModMaker.Models;
using VPet.ModMaker.Views.ModEdit.I18nEdit;
using VPet.ModMaker.Views.ModEdit.PetEdit;

namespace VPet.ModMaker.ViewModels.ModEdit.PetEdit;

public class PetPageVM : ObservableObjectX<PetPageVM>
{
    public PetPageVM()
    {
        Pets = new(ModInfoModel.Current.Pets)
        {
            Filter = f => f.ID.Contains(Search, StringComparison.OrdinalIgnoreCase),
            FilteredList = new()
        };

        AddCommand.ExecuteCommand += Add;
        EditCommand.ExecuteCommand += Edit;
        RemoveCommand.ExecuteCommand += Remove;
    }

    public static ModInfoModel ModInfo => ModInfoModel.Current;

    #region Property
    #region ShowPets
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private ObservableFilterList<PetModel, ObservableList<PetModel>> _pets = null!;

    public ObservableFilterList<PetModel, ObservableList<PetModel>> Pets
    {
        get => _pets;
        set => SetProperty(ref _pets, value);
    }
    #endregion

    #region Search
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string _search = string.Empty;

    public string Search
    {
        get => _search;
        set
        {
            SetProperty(ref _search, value);
            Pets.Refresh();
        }
    }
    #endregion
    #endregion
    #region Command
    public ObservableCommand AddCommand { get; } = new();
    public ObservableCommand<PetModel> EditCommand { get; } = new();
    public ObservableCommand<PetModel> RemoveCommand { get; } = new();
    #endregion

    public void Close() { }

    private void Add()
    {
        var window = new PetEditWindow();
        var vm = window.ViewModel;
        window.ShowDialog();
        if (window.IsCancel)
            return;
        Pets.Add(vm.Pet);
    }

    public void Edit(PetModel model)
    {
        if (model.FromMain)
        {
            if (
                MessageBox.Show("这是本体自带的宠物, 确定要编辑吗".Translate(), "", MessageBoxButton.YesNo)
                is not MessageBoxResult.Yes
            )
                return;
        }
        var window = new PetEditWindow();
        var vm = window.ViewModel;
        vm.OldPet = model;
        var newPet = vm.Pet = new(model);
        window.ShowDialog();
        if (window.IsCancel)
            return;
        if (model.FromMain)
        {
            var index = Pets.IndexOf(model);
            Pets.Remove(model);
            Pets.Insert(index, newPet);
        }
        else
        {
            Pets[Pets.IndexOf(model)] = newPet;
        }
        model.Close();
    }

    private void Remove(PetModel model)
    {
        if (model.FromMain)
        {
            MessageBox.Show("这是本体自带的宠物, 无法删除".Translate());
            return;
        }
        if (MessageBox.Show("确定删除吗".Translate(), "", MessageBoxButton.YesNo) is MessageBoxResult.No)
            return;
        Pets.Remove(model);
    }
}
