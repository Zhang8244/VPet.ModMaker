﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData.Binding;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs;
using HKW.HKWReactiveUI;
using HKW.HKWUtils.Collections;
using HKW.HKWUtils.Extensions;
using HKW.HKWUtils.Observable;
using HKW.MVVMDialogs;
using HKW.WPF.MVVMDialogs;
using LinePutScript.Localization.WPF;
using Panuon.WPF.UI;
using ReactiveUI;
using Splat;
using VPet.ModMaker.Models;
using VPet.ModMaker.Models.ModModel;
using VPet.ModMaker.Views.ModEdit;

namespace VPet.ModMaker.ViewModels.ModEdit;

/// <summary>
/// 动画视图模型
/// </summary>
public partial class AnimeVM : ViewModelBase
{
    private static IDialogService DialogService => Locator.Current.GetService<IDialogService>()!;

    /// <inheritdoc/>
    public AnimeVM()
    {
        AllAnimes = new(
            [],
            [],
            (f) =>
            {
                if (f is AnimeTypeModel animeModel)
                {
                    return animeModel.ID.Contains(Search, StringComparison.OrdinalIgnoreCase);
                }
                else if (f is FoodAnimeTypeModel foodAnimeModel)
                {
                    return foodAnimeModel.ID.Contains(Search, StringComparison.OrdinalIgnoreCase);
                }
                else
                    return false;
            }
        );

        this.WhenValueChanged(x => x.Search)
            .Throttle(TimeSpan.FromSeconds(0.5), RxApp.TaskpoolScheduler)
            .DistinctUntilChanged()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(_ => AllAnimes.Refresh());
    }

    #region Property
    /// <summary>
    /// 模组信息
    /// </summary>
    [ReactiveProperty]
    public ModInfoModel ModInfo { get; set; } = null!;

    partial void OnModInfoChanged(ModInfoModel oldValue, ModInfoModel newValue)
    {
        if (oldValue is not null)
        {
            oldValue.PropertyChanged -= ModInfo_PropertyChanged;
        }
        if (newValue is not null)
        {
            newValue
                .WhenValueChanged(x => x.CurrentPet)
                .Throttle(TimeSpan.FromSeconds(0.5), RxApp.TaskpoolScheduler)
                .DistinctUntilChanged()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(x => CurrentPet = x!);
        }
    }

    private void ModInfo_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ModInfoModel.ShowMainPet))
        {
            if (CurrentPet?.FromMain is false)
                CurrentPet = null!;
        }
    }

    /// <summary>
    /// 所有动画
    /// </summary>
    public FilterListWrapper<object, List<object>, ObservableList<object>> AllAnimes { get; }

    /// <summary>
    /// 动画
    /// </summary>
    public ObservableList<AnimeTypeModel> Animes => CurrentPet.Animes;

    /// <summary>
    /// 食物动画
    /// </summary>
    public ObservableList<FoodAnimeTypeModel> FoodAnimes => CurrentPet.FoodAnimes;

    /// <summary>
    /// 当前宠物
    /// </summary>
    [ReactiveProperty]
    public PetModel CurrentPet { get; set; } = null!;

    partial void OnCurrentPetChanged(PetModel oldValue, PetModel newValue)
    {
        if (oldValue is not null)
        {
            Animes.CollectionChanged -= Animes_CollectionChanged;
            FoodAnimes.CollectionChanged -= Animes_CollectionChanged;
        }
        AllAnimes.AutoFilter = false;
        AllAnimes.Clear();
        if (newValue is not null)
        {
            AllAnimes.AddRange(newValue.Animes);
            AllAnimes.AddRange(newValue.FoodAnimes);
            Search = string.Empty;
        }
        AllAnimes.Refresh();
        AllAnimes.AutoFilter = true;

        Animes.CollectionChanged -= Animes_CollectionChanged;
        Animes.CollectionChanged += Animes_CollectionChanged;
        FoodAnimes.CollectionChanged -= Animes_CollectionChanged;
        FoodAnimes.CollectionChanged += Animes_CollectionChanged;
    }

    /// <summary>
    /// 搜索
    /// </summary>
    [ReactiveProperty]
    public string Search { get; set; } = string.Empty;
    #endregion

    private void Animes_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action is NotifyCollectionChangedAction.Add)
            AllAnimes.Add(e.NewItems![0]!);
        else if (e.Action is NotifyCollectionChangedAction.Remove)
            AllAnimes.Remove(e.OldItems![0]!);
        else if (e.Action is NotifyCollectionChangedAction.Replace)
            AllAnimes[AllAnimes.IndexOf(e.OldItems![0]!)] = e.NewItems![0]!;
    }

    /// <summary>
    /// 添加
    /// </summary>
    [ReactiveCommand]
    private async void Add()
    {
        var vm = await DialogService.ShowDialogAsyncX(
            this,
            new SelectGraphTypeVM() { CurrentPet = CurrentPet, }
        );
        if (vm.DialogResult is not true)
            return;
        var graphType = vm.GraphType;
        var animeName = vm.AnimeName;
        if (
            graphType is VPet_Simulator.Core.GraphInfo.GraphType.Common
            && FoodAnimeTypeModel.FoodAnimeNames.Contains(animeName)
        )
        {
            var animeVM = await DialogService.ShowDialogAsyncX(
                this,
                new FoodAnimeEditVM()
                {
                    CurrentPet = CurrentPet,
                    Anime = new() { Name = animeName }
                }
            );
            if (animeVM.DialogResult is not true)
                return;
            FoodAnimes.Add(animeVM.Anime);
        }
        else
        {
            var animeVM = await DialogService.ShowDialogAsyncX(
                this,
                new AnimeEditVM()
                {
                    CurrentPet = CurrentPet,
                    Anime = new()
                    {
                        GraphType = graphType,
                        Name = string.IsNullOrWhiteSpace(animeName)
                            ? graphType.ToString()
                            : animeName
                    }
                }
            );
            if (animeVM.DialogResult is not true)
                return;
            Animes.Add(animeVM.Anime);
        }
    }

    /// <summary>
    /// 编辑
    /// </summary>
    /// <param name="model">模型</param>
    [ReactiveCommand]
    public async void Edit(object model)
    {
        //var pendingHandler = PendingBox.Show("载入中".Translate());
        if (model is AnimeTypeModel animeTypeModel)
        {
            var animeVM = await DialogService.ShowDialogAsyncX(
                this,
                new AnimeEditVM()
                {
                    CurrentPet = CurrentPet,
                    OldAnime = animeTypeModel,
                    Anime = new(animeTypeModel)
                }
            );
            if (animeVM.DialogResult is not true)
            {
                animeVM.Anime?.Close();
            }
            else
            {
                animeVM.OldAnime?.Close();
                Animes[Animes.IndexOf(animeTypeModel)] = animeVM.Anime;
            }
        }
        else if (model is FoodAnimeTypeModel foodAnimeTypeModel)
        {
            var animeVM = await DialogService.ShowDialogAsyncX(
                this,
                new FoodAnimeEditVM()
                {
                    CurrentPet = CurrentPet,
                    OldAnime = foodAnimeTypeModel,
                    Anime = new(foodAnimeTypeModel)
                }
            );
            if (animeVM.DialogResult is not true)
            {
                animeVM.Anime?.Close();
            }
            else
            {
                animeVM.OldAnime?.Close();
                FoodAnimes[FoodAnimes.IndexOf(foodAnimeTypeModel)] = animeVM.Anime;
            }
        }
    }

    /// <summary>
    /// 删除
    /// </summary>
    /// <param name="model">模型</param>
    [ReactiveCommand]
    private void Remove(object model)
    {
        if (
            DialogService.ShowMessageBoxX(
                "确定删除动画 {} 吗".Translate(),
                "删除动画".Translate(),
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            )
            is not true
        )
            return;
        AllAnimes.Remove(model);
        if (model is AnimeTypeModel animeTypeModel)
        {
            Animes.Remove(animeTypeModel);
            animeTypeModel.Close();
        }
        else if (model is FoodAnimeTypeModel foodAnimeTypeModel)
        {
            FoodAnimes.Remove(foodAnimeTypeModel);
            foodAnimeTypeModel.Close();
        }
    }
}
