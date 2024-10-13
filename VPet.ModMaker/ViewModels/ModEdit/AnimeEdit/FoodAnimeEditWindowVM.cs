﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using HKW.HKWReactiveUI;
using HKW.HKWUtils.Observable;
using LinePutScript.Localization.WPF;
using Microsoft.Win32;
using VPet.ModMaker.Models;
using VPet.ModMaker.Models.ModModel;
using VPet.ModMaker.Resources;
using VPet_Simulator.Core;
using static VPet_Simulator.Core.IGameSave;

namespace VPet.ModMaker.ViewModels.ModEdit.AnimeEdit;

public partial class FoodAnimeEditWindowVM : ViewModelBase
{
    public FoodAnimeEditWindowVM()
    {
        _frontPlayerTask = new(FrontPlay);
        _backPlayerTask = new(BackPlay);
        _foodPlayerTask = new(FoodPlay);
        FoodImage = DefaultFoodImage;
        //PropertyChangedX += FoodAnimeEditWindowVM_PropertyChangedX;

        //PlayCommand.ExecuteAsyncCommand += PlayCommand_AsyncExecuteEvent;
        //StopCommand.ExecuteCommand += StopCommand_ExecuteEvent;
        //ReplaceFoodImageCommand.ExecuteCommand += ChangeFoodImageCommand_ExecuteEvent;
        //ResetFoodImageCommand.ExecuteCommand += ResetFoodImageCommand_ExecuteEvent;

        //AddAnimeCommand.ExecuteCommand += AddAnimeCommand_ExecuteEvent;
        //RemoveAnimeCommand.ExecuteCommand += RemoveAnimeCommand_ExecuteEvent;

        //AddFrontImageCommand.ExecuteCommand += AddFrontImageCommand_ExecuteEvent;
        //RemoveFrontImageCommand.ExecuteCommand += RemoveFrontImageCommand_ExecuteEvent;
        //ClearFrontImageCommand.ExecuteCommand += ClearFrontImageCommand_ExecuteEvent;
        //ChangeFrontImageCommand.ExecuteCommand += ChangeFrontImageCommand_ExecuteEvent;

        //AddBackImageCommand.ExecuteCommand += AddBackImageCommand_ExecuteEvent;
        //RemoveBackImageCommand.ExecuteCommand += RemoveBackImageCommand_ExecuteEvent;
        //ClearBackImageCommand.ExecuteCommand += ClearBackImageCommand_ExecuteEvent;
        //ChangeBackImageCommand.ExecuteCommand += ChangeBackImageCommand_ExecuteEvent;

        //AddFoodLocationCommand.ExecuteCommand += AddeFoodLocationCommand_ExecuteEvent;
        //RemoveFoodLocationCommand.ExecuteCommand += RemoveFoodLocationCommand_ExecuteEvent;
        //ClearFoodLocationCommand.ExecuteCommand += ClearFoodLocationCommand_ExecuteEvent;
    }

    //private void FoodAnimeEditWindowVM_PropertyChangedX(object? sender, PropertyChangedXEventArgs e)
    //{
    //    if (e.PropertyName == nameof(CurrentAnimeModel))
    //    {
    //        var newModel = e.NewValue as FoodAnimeModel;
    //        var oldModel = e.OldValue as FoodAnimeModel;
    //        Stop();
    //        if (oldModel is not null)
    //        {
    //            oldModel.FrontImages.CollectionChanged -= Images_CollectionChanged;
    //            oldModel.BackImages.CollectionChanged -= Images_CollectionChanged;
    //            oldModel.FoodLocations.CollectionChanged -= Images_CollectionChanged;
    //        }
    //        if (newModel is not null)
    //        {
    //            newModel.FrontImages.CollectionChanged += Images_CollectionChanged;
    //            newModel.BackImages.CollectionChanged += Images_CollectionChanged;
    //            newModel.FoodLocations.CollectionChanged += Images_CollectionChanged;
    //        }
    //    }
    //}

    /// <summary>
    /// 当前宠物
    /// </summary>
    public PetModel CurrentPet { get; set; }

    /// <summary>
    /// 默认食物图片
    /// </summary>
    public static BitmapImage DefaultFoodImage { get; } =
        NativeUtils.LoadImageToMemoryStream(NativeResources.GetStream(NativeResources.FoodImage));

    /// <summary>
    /// 食物图片
    /// </summary>
    [ReactiveProperty]
    public BitmapImage FoodImage { get; set; }

    /// <summary>
    /// 比例
    /// </summary>
    [ReactiveProperty]
    public double LengthRatio { get; set; } = 0.5;

    /// <summary>
    /// 旧动画
    /// </summary>
    public FoodAnimeTypeModel? OldAnime { get; set; }

    /// <summary>
    /// 动画
    /// </summary>
    [ReactiveProperty]
    public FoodAnimeTypeModel Anime { get; set; } = new();

    /// <summary>
    /// 当前顶层图像模型
    /// </summary>
    [ReactiveProperty]
    public ImageModel CurrentFrontImageModel { get; set; }

    /// <summary>
    /// 当前底层图像模型
    /// </summary>
    [ReactiveProperty]
    public ImageModel CurrentBackImageModel { get; set; }

    /// <summary>
    /// 当前食物定位模型
    /// </summary>
    [ReactiveProperty]
    public FoodAnimeLocationModel CurrentFoodLocationModel { get; set; }

    /// <summary>
    /// 当前动画模型
    /// </summary>
    [ReactiveProperty]
    public FoodAnimeModel CurrentAnimeModel { get; set; }

    partial void OnCurrentAnimeModelChanged(FoodAnimeModel oldValue, FoodAnimeModel newValue)
    {
        Stop();
        if (oldValue is not null)
        {
            oldValue.FrontImages.CollectionChanged -= Images_CollectionChanged;
            oldValue.BackImages.CollectionChanged -= Images_CollectionChanged;
            oldValue.FoodLocations.CollectionChanged -= Images_CollectionChanged;
        }
        if (newValue is not null)
        {
            newValue.FrontImages.CollectionChanged += Images_CollectionChanged;
            newValue.BackImages.CollectionChanged += Images_CollectionChanged;
            newValue.FoodLocations.CollectionChanged += Images_CollectionChanged;
        }
    }

    /// <summary>
    /// 当前模式
    /// </summary>
    public ModeType CurrentMode { get; set; }

    /// <summary>
    /// 循环
    /// </summary>
    [ReactiveProperty]
    public bool Loop { get; set; }

    ///// <summary>
    ///// 含有多个状态
    ///// </summary>
    //[ReactiveProperty]
    //[NotifyPropertyChangeFrom(nameof(Anime))]
    //public bool HasMultiType => AnimeTypeModel.HasMultiTypeAnimes.Contains(Anime.GraphType);

    ///// <summary>
    ///// 含有动画名称
    ///// </summary>
    //[ReactiveProperty]
    //[NotifyPropertyChangeFrom(nameof(Anime))]
    //public bool HasAnimeName => AnimeTypeModel.HasNameAnimes.Contains(Anime.GraphType);

    //#region Command
    ///// <summary>
    ///// 播放命令
    ///// </summary>
    //public ObservableCommand PlayCommand { get; } = new();

    ///// <summary>
    ///// 停止命令
    ///// </summary>
    //public ObservableCommand StopCommand { get; } = new();

    ///// <summary>
    ///// 添加动画命令
    ///// </summary>
    //public ObservableCommand AddAnimeCommand { get; } = new();

    ///// <summary>
    ///// 删除动画命令
    ///// </summary>
    //public ObservableCommand<FoodAnimeModel> RemoveAnimeCommand { get; } = new();

    //#region FrontImage
    ///// <summary>
    ///// 添加顶层图片命令
    ///// </summary>
    //public ObservableCommand<FoodAnimeModel> AddFrontImageCommand { get; } = new();

    ///// <summary>
    ///// 删除顶层图片命令
    ///// </summary>
    //public ObservableCommand<FoodAnimeModel> RemoveFrontImageCommand { get; } = new();

    ///// <summary>
    ///// 清除顶层图片命令
    ///// </summary>
    //public ObservableCommand<FoodAnimeModel> ClearFrontImageCommand { get; } = new();

    ///// <summary>
    ///// 改变顶层图片命令
    ///// </summary>
    //public ObservableCommand<FoodAnimeModel> ChangeFrontImageCommand { get; } = new();
    //#endregion

    //#region BackImage
    ///// <summary>
    ///// 添加底层图片命令
    ///// </summary>
    //public ObservableCommand<FoodAnimeModel> AddBackImageCommand { get; } = new();

    ///// <summary>
    ///// 删除底层图片命令
    ///// </summary>
    //public ObservableCommand<FoodAnimeModel> RemoveBackImageCommand { get; } = new();

    ///// <summary>
    ///// 清除底层图片命令
    ///// </summary>
    //public ObservableCommand<FoodAnimeModel> ClearBackImageCommand { get; } = new();

    ///// <summary>
    ///// 改变底层图片命令
    ///// </summary>
    //public ObservableCommand<FoodAnimeModel> ChangeBackImageCommand { get; } = new();
    //#endregion
    //#region FoodLocation
    ///// <summary>
    ///// 添加食物定位命令
    ///// </summary>
    //public ObservableCommand<FoodAnimeModel> AddFoodLocationCommand { get; } = new();

    ///// <summary>
    ///// 删除食物定位命令
    ///// </summary>
    //public ObservableCommand<FoodAnimeModel> RemoveFoodLocationCommand { get; } = new();

    ///// <summary>
    ///// 清除食物定位命令
    ///// </summary>
    //public ObservableCommand<FoodAnimeModel> ClearFoodLocationCommand { get; } = new();
    //#endregion
    ///// <summary>
    ///// 改变食物图片
    ///// </summary>
    //public ObservableCommand ReplaceFoodImageCommand { get; } = new();

    ///// <summary>
    ///// 重置食物图片
    ///// </summary>
    //public ObservableCommand ResetFoodImageCommand { get; } = new();

    //#endregion

    /// <summary>
    /// 正在播放
    /// </summary>
    private bool _playing = false;

    /// <summary>
    /// 顶层动画任务
    /// </summary>
    private Task _frontPlayerTask;

    /// <summary>
    /// 底层动画任务
    /// </summary>
    private Task _backPlayerTask;

    /// <summary>
    /// 食物动画任务
    /// </summary>
    private Task _foodPlayerTask;

    #region Command

    [ReactiveCommand]
    private void ResetFoodImage()
    {
        if (FoodImage != DefaultFoodImage)
            FoodImage.CloseStream();
        FoodImage = DefaultFoodImage;
    }

    [ReactiveCommand]
    private void ChangeFoodImage()
    {
        OpenFileDialog openFileDialog =
            new() { Title = "选择食物图片".Translate(), Filter = $"图片|*.png".Translate() };
        if (openFileDialog.ShowDialog() is true)
        {
            if (FoodImage != DefaultFoodImage)
                FoodImage.CloseStream();
            FoodImage = NativeUtils.LoadImageToMemoryStream(openFileDialog.FileName);
        }
    }

    //#region LoadAnime
    //private void Anime_ValueChanged(ObservableValue<FoodAnimeTypeModel> sender, ValueChangedEventArgs<FoodAnimeTypeModel> e)
    //{
    //    CheckGraphType(newValue);
    //}

    //private void CheckGraphType(FoodAnimeTypeModel model)
    //{
    //    //if (FoodAnimeTypeModel.HasMultiTypeAnimes.Contains(model.GraphType.Value))
    //    //    HasMultiType.Value = true;

    //    //if (FoodAnimeTypeModel.HasNameAnimes.Contains(model.GraphType.Value))
    //    //    HasAnimeName.Value = true;
    //}
    //#endregion
    #region AnimeCommand
    [ReactiveCommand]
    private void AddAnime()
    {
        if (CurrentMode is ModeType.Happy)
            Anime.HappyAnimes.Add(new());
        else if (CurrentMode is ModeType.Nomal)
            Anime.NomalAnimes.Add(new());
        else if (CurrentMode is ModeType.PoorCondition)
            Anime.PoorConditionAnimes.Add(new());
        else if (CurrentMode is ModeType.Ill)
            Anime.IllAnimes.Add(new());
    }

    /// <summary>
    /// 删除动画
    /// </summary>
    /// <param name="value">动画模型</param>
    [ReactiveCommand]
    private void RemoveAnime(FoodAnimeModel value)
    {
        if (
            MessageBox.Show("确定删除吗".Translate(), "", MessageBoxButton.YesNo) is MessageBoxResult.Yes
        )
        {
            if (CurrentMode is ModeType.Happy)
                Anime.HappyAnimes.Remove(value);
            else if (CurrentMode is ModeType.Nomal)
                Anime.NomalAnimes.Remove(value);
            else if (CurrentMode is ModeType.PoorCondition)
                Anime.PoorConditionAnimes.Remove(value);
            else if (CurrentMode is ModeType.Ill)
                Anime.IllAnimes.Remove(value);
            value.Close();
        }
    }
    #endregion
    #region ImageCommand

    #region FrontImageCommand
    /// <summary>
    /// 添加顶层图片
    /// </summary>
    /// <param name="value">动画模型</param>
    [ReactiveCommand]
    private void AddFrontImage(FoodAnimeModel value)
    {
        OpenFileDialog openFileDialog =
            new()
            {
                Title = "选择图片".Translate(),
                Filter = $"图片|*.png".Translate(),
                Multiselect = true
            };
        if (openFileDialog.ShowDialog() is true)
        {
            AddImages(value.FrontImages, openFileDialog.FileNames);
        }
    }

    /// <summary>
    /// 删除顶层图片
    /// </summary>
    /// <param name="value">动画模型</param>
    [ReactiveCommand]
    private void RemoveFrontImage(FoodAnimeModel value)
    {
        CurrentFrontImageModel.Close();
        value.FrontImages.Remove(CurrentFrontImageModel);
    }

    /// <summary>
    /// 清空顶层图片
    /// </summary>
    /// <param name="value">动画模型</param>
    [ReactiveCommand]
    private void ClearFrontImage(FoodAnimeModel value)
    {
        if (
            MessageBox.Show("确定清空吗".Translate(), "", MessageBoxButton.YesNo) is MessageBoxResult.Yes
        )
        {
            foreach (var image in value.FrontImages)
                image.Close();
            value.FrontImages.Clear();
        }
    }

    /// <summary>
    /// 替换顶层图片
    /// </summary>
    /// <param name="value">动画模型</param>
    [ReactiveCommand]
    private void ChangeFrontImage(FoodAnimeModel value)
    {
        OpenFileDialog openFileDialog =
            new() { Title = "选择图片".Translate(), Filter = $"图片|*.png".Translate() };
        if (openFileDialog.ShowDialog() is not true)
            return;
        BitmapImage newImage;
        try
        {
            newImage = NativeUtils.LoadImageToMemoryStream(openFileDialog.FileName);
        }
        catch (Exception ex)
        {
            MessageBox.Show("替换失败失败 \n{0}".Translate(ex));
            return;
        }
        if (newImage is null)
            return;
        CurrentFrontImageModel.Close();
        CurrentFrontImageModel.Image = newImage;
    }
    #endregion

    #region BackImageCommand
    /// <summary>
    /// 添加顶层图片
    /// </summary>
    /// <param name="value">动画模型</param>
    [ReactiveCommand]
    private void AddBackImage(FoodAnimeModel value)
    {
        OpenFileDialog openFileDialog =
            new()
            {
                Title = "选择图片".Translate(),
                Filter = $"图片|*.png".Translate(),
                Multiselect = true
            };
        if (openFileDialog.ShowDialog() is true)
        {
            AddImages(value.BackImages, openFileDialog.FileNames);
        }
    }

    /// <summary>
    /// 删除顶层图片
    /// </summary>
    /// <param name="value">动画模型</param>
    [ReactiveCommand]
    private void RemoveBackImage(FoodAnimeModel value)
    {
        CurrentBackImageModel.Close();
        value.BackImages.Remove(CurrentBackImageModel);
    }

    /// <summary>
    /// 清空顶层图片
    /// </summary>
    /// <param name="value">动画模型</param>
    [ReactiveCommand]
    private void ClearBackImage(FoodAnimeModel value)
    {
        if (
            MessageBox.Show("确定清空吗".Translate(), "", MessageBoxButton.YesNo) is MessageBoxResult.Yes
        )
        {
            foreach (var image in value.BackImages)
                image.Close();
            value.BackImages.Clear();
        }
    }

    /// <summary>
    /// 替换底层图片
    /// </summary>
    /// <param name="value"></param>
    /// <exception cref="NotImplementedException"></exception>
    [ReactiveCommand]
    private void ChangeBackImage(FoodAnimeModel value)
    {
        OpenFileDialog openFileDialog =
            new() { Title = "选择图片".Translate(), Filter = $"图片|*.png".Translate() };
        if (openFileDialog.ShowDialog() is not true)
            return;
        BitmapImage newImage;
        try
        {
            newImage = NativeUtils.LoadImageToMemoryStream(openFileDialog.FileName);
        }
        catch (Exception ex)
        {
            MessageBox.Show("替换失败失败 \n{0}".Translate(ex));
            return;
        }
        if (newImage is null)
            return;
        CurrentBackImageModel.Close();
        CurrentBackImageModel.Image = newImage;
    }
    #endregion

    /// <summary>
    /// 添加图片
    /// </summary>
    /// <param name="images">动画</param>
    /// <param name="paths">路径</param>
    public void AddImages(ObservableList<ImageModel> images, IEnumerable<string> paths)
    {
        try
        {
            var newImages = new List<ImageModel>();
            foreach (string path in paths)
            {
                if (File.Exists(path))
                {
                    newImages.Add(new(NativeUtils.LoadImageToMemoryStream(path)));
                }
                else if (Directory.Exists(path))
                {
                    foreach (var file in Directory.EnumerateFiles(path, "*.png"))
                    {
                        newImages.Add(new(NativeUtils.LoadImageToMemoryStream(path)));
                    }
                }
            }
            foreach (var image in newImages)
                images.Add(image);
        }
        catch (Exception ex)
        {
            MessageBox.Show("添加失败 \n{0}".Translate(ex));
        }
    }
    #endregion
    #region FoodLocationCommand
    [ReactiveCommand]
    private void AddeFoodLocation(FoodAnimeModel value)
    {
        value.FoodLocations.Add(new());
    }

    [ReactiveCommand]
    private void RemoveFoodLocation(FoodAnimeModel value)
    {
        value.FoodLocations.Remove(CurrentFoodLocationModel);
        CurrentFoodLocationModel = null!;
    }

    [ReactiveCommand]
    private void ClearFoodLocation(FoodAnimeModel value)
    {
        if (
            MessageBox.Show("确定清空吗".Translate(), "", MessageBoxButton.YesNo) is MessageBoxResult.Yes
        )
        {
            value.FoodLocations.Clear();
        }
    }
    #endregion
    #region FrontPlayer

    private void Images_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        Stop();
    }

    /// <summary>
    /// 停止播放
    /// </summary>
    [ReactiveCommand]
    private void Stop()
    {
        _playing = false;
    }

    /// <summary>
    /// 开始播放
    /// </summary>
    [ReactiveCommand]
    private async Task Play()
    {
        if (CurrentAnimeModel is null)
        {
            MessageBox.Show("未选中动画".Translate());
            return;
        }
        _playing = true;
        do
        {
            _frontPlayerTask.Start();
            _backPlayerTask.Start();
            _foodPlayerTask.Start();
            await Task.WhenAll(_frontPlayerTask, _backPlayerTask, _foodPlayerTask);
            _frontPlayerTask = new(FrontPlay);
            _backPlayerTask = new(BackPlay);
            _foodPlayerTask = new(FoodPlay);
        } while (Loop && _playing);
    }

    /// <summary>
    /// 顶层播放
    /// </summary>
    private void FrontPlay()
    {
        foreach (var model in CurrentAnimeModel.FrontImages)
        {
            CurrentFrontImageModel = model;
            Task.Delay(model.Duration).Wait();
            if (_playing is false)
                return;
        }
    }

    /// <summary>
    /// 底层
    /// </summary>
    private void BackPlay()
    {
        foreach (var model in CurrentAnimeModel.BackImages)
        {
            CurrentBackImageModel = model;
            Task.Delay(model.Duration).Wait();
            if (_playing is false)
                return;
        }
    }

    /// <summary>
    /// 食物
    /// </summary>
    private void FoodPlay()
    {
        foreach (var model in CurrentAnimeModel.FoodLocations)
        {
            CurrentFoodLocationModel = model;
            Task.Delay(model.Duration).Wait();
            if (_playing is false)
                return;
        }
    }

    /// <summary>
    /// 重置
    /// </summary>
    private void Reset()
    {
        _playing = false;
        _frontPlayerTask = new(FrontPlay);
        _backPlayerTask = new(BackPlay);
        _foodPlayerTask = new(FoodPlay);
    }
    #endregion
    #endregion
}
