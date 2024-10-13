﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using HKW.HKWReactiveUI;
using HKW.HKWUtils.Collections;
using HKW.HKWUtils.Extensions;
using HKW.HKWUtils.Observable;
using LinePutScript.Localization.WPF;
using VPet.ModMaker.Models;
using VPet.ModMaker.Views.ModEdit.I18nEdit;
using VPet.ModMaker.Views.ModEdit.PetEdit;

namespace VPet.ModMaker.ViewModels.ModEdit.PetEdit;

public partial class PetPageVM : ViewModelBase
{
    public PetPageVM()
    {
        Pets = new(
            ModInfoModel.Current.Pets,
            [],
            (f) =>
            {
                if (ShowMainPet is false && f.FromMain)
                    return false;
                return f.ID.Contains(Search, StringComparison.OrdinalIgnoreCase);
            }
        );
        //TODO:
        //Pets.BindingList(ModInfoModel.Current.Pets);

        //AddCommand.ExecuteCommand += Add;
        //EditCommand.ExecuteCommand += Edit;
        //RemoveCommand.ExecuteCommand += Remove;
    }

    public static ModInfoModel ModInfo => ModInfoModel.Current;

    #region Property
    public FilterListWrapper<
        PetModel,
        ObservableList<PetModel>,
        ObservableList<PetModel>
    > Pets { get; set; }

    [ReactiveProperty]
    public string Search { get; set; } = string.Empty;

    partial void OnSearchChanged(string oldValue, string newValue)
    {
        Pets.Refresh();
    }

    [ReactiveProperty]
    public bool ShowMainPet { get; set; }

    partial void OnShowMainPetChanged(bool oldValue, bool newValue)
    {
        ModInfo.ShowMainPet = newValue;
        Pets.Refresh();
    }
    #endregion

    //#region Command
    //public ObservableCommand AddCommand { get; } = new();
    //public ObservableCommand<PetModel> EditCommand { get; } = new();
    //public ObservableCommand<PetModel> RemoveCommand { get; } = new();
    //#endregion

    public void Close() { }

    [ReactiveCommand]
    private void Add()
    {
        var window = new PetEditWindow();
        var vm = window.ViewModel;
        window.ShowDialog();
        if (window.IsCancel)
            return;
        Pets.Add(vm.Pet);
    }

    [ReactiveCommand]
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
        var newModel = vm.Pet = new(model) { I18nResource = ModInfoModel.Current.TempI18nResource };
        model.I18nResource.CopyDataTo(
            newModel.I18nResource,
            [model.ID, model.PetNameID, model.DescriptionID],
            true
        );
        window.ShowDialog();
        if (window.IsCancel)
        {
            newModel.I18nResource.ClearCultureData();
            newModel.CloseI18nResource();
            return;
        }
        newModel.I18nResource.CopyDataTo(ModInfoModel.Current.I18nResource, true);
        newModel.I18nResource = ModInfoModel.Current.I18nResource;
        if (model.FromMain)
        {
            var index = Pets.IndexOf(model);
            Pets.Remove(model);
            Pets.Insert(index, newModel);
        }
        else
        {
            Pets[Pets.IndexOf(model)] = newModel;
        }
        model.CloseI18nResource();
    }

    [ReactiveCommand]
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
        model.CloseI18nResource();
    }
}
