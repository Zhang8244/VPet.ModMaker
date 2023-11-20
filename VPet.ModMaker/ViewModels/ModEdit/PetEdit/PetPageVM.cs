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
using VPet.ModMaker.Views.ModEdit.PetEdit;

namespace VPet.ModMaker.ViewModels.ModEdit.PetEdit;

public class PetPageVM
{
    #region Value
    public ObservableValue<ObservableCollection<PetModel>> ShowPets { get; } = new();
    public ObservableCollection<PetModel> Pets => ModInfoModel.Current.Pets;
    public ObservableValue<string> Search { get; } = new();
    #endregion
    #region Command
    public ObservableCommand AddCommand { get; } = new();
    public ObservableCommand<PetModel> EditCommand { get; } = new();
    public ObservableCommand<PetModel> RemoveCommand { get; } = new();
    #endregion
    public PetPageVM()
    {
        ShowPets.Value = Pets;
        Search.ValueChanged += Search_ValueChanged;

        AddCommand.ExecuteEvent += Add;
        EditCommand.ExecuteEvent += Edit;
        RemoveCommand.ExecuteEvent += Remove;
    }

    private void Search_ValueChanged(
        ObservableValue<string> sender,
        ValueChangedEventArgs<string> e
    )
    {
        if (string.IsNullOrWhiteSpace(e.NewValue))
        {
            ShowPets.Value = Pets;
        }
        else
        {
            ShowPets.Value = new(
                Pets.Where(m => m.Id.Value.Contains(e.NewValue, StringComparison.OrdinalIgnoreCase))
            );
        }
    }

    public void Close() { }

    private void Add()
    {
        var window = new PetEditWindow();
        var vm = window.ViewModel;
        window.ShowDialog();
        if (window.IsCancel)
            return;
        Pets.Add(vm.Pet.Value);
    }

    public void Edit(PetModel model)
    {
        if (model.IsSimplePetModel)
        {
            MessageBox.Show("这是本体自带的宠物, 无法编辑".Translate());
            return;
        }
        var window = new PetEditWindow();
        var vm = window.ViewModel;
        vm.OldPet = model;
        var newPet = vm.Pet.Value = new(model);
        window.ShowDialog();
        if (window.IsCancel)
            return;
        Pets[Pets.IndexOf(model)] = newPet;
        if (ShowPets.Value.Count != Pets.Count)
            ShowPets.Value[ShowPets.Value.IndexOf(model)] = newPet;
        model.Close();
    }

    private void Remove(PetModel model)
    {
        if (model.IsSimplePetModel)
        {
            MessageBox.Show("这是本体自带的宠物, 无法删除".Translate());
            return;
        }
        if (MessageBox.Show("确定删除吗".Translate(), "", MessageBoxButton.YesNo) is MessageBoxResult.No)
            return;
        if (ShowPets.Value.Count == Pets.Count)
        {
            Pets.Remove(model);
        }
        else
        {
            ShowPets.Value.Remove(model);
            Pets.Remove(model);
        }
    }
}
