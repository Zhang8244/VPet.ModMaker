﻿using HKW.HKWViewModels.SimpleObservable;
using LinePutScript.Localization.WPF;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VPet.ModMaker.Models;
using VPet.ModMaker.Models.ModModel;
using VPet_Simulator.Core;

namespace VPet.ModMaker.ViewModels.ModEdit.AnimeEdit;

public class AnimeEditWindowVM
{
    public PetModel CurrentPet { get; set; }
    public AnimeTypeModel OldAnime { get; set; }
    public ObservableValue<AnimeTypeModel> Anime { get; } = new(new());
    public ObservableValue<ImageModel> CurrentImageModel { get; } = new();
    public ObservableValue<AnimeModel> CurrentAnimeModel { get; } = new();
    public GameSave.ModeType CurrentMode { get; set; }
    public ObservableValue<bool> Loop { get; } = new();
    #region Command
    public ObservableCommand PlayCommand { get; } = new();
    public ObservableCommand StopCommand { get; } = new();

    public ObservableCommand<AnimeModel> AddImageCommand { get; } = new();
    public ObservableCommand<AnimeModel> ClearImageCommand { get; } = new();
    public ObservableCommand<AnimeModel> RemoveAnimeCommand { get; } = new();
    public ObservableCommand<AnimeModel> RemoveImageCommand { get; } = new();
    #endregion

    private bool _playing = false;
    private Task _playerTask;

    public AnimeEditWindowVM()
    {
        _playerTask = new(Play);

        CurrentAnimeModel.ValueChanged += CurrentAnimeModel_ValueChanged;
        ;

        PlayCommand.ExecuteEvent += PlayCommand_ExecuteEvent;
        StopCommand.ExecuteEvent += StopCommand_ExecuteEvent;
        AddImageCommand.ExecuteEvent += AddImageCommand_ExecuteEvent;
        ClearImageCommand.ExecuteEvent += ClearImageCommand_ExecuteEvent;
        RemoveAnimeCommand.ExecuteEvent += RemoveAnimeCommand_ExecuteEvent;
        RemoveImageCommand.ExecuteEvent += RemoveImageCommand_ExecuteEvent;
    }

    private void CurrentAnimeModel_ValueChanged(AnimeModel oldValue, AnimeModel newValue)
    {
        StopCommand_ExecuteEvent();
        oldValue.Images.CollectionChanged -= Images_CollectionChanged;
        newValue.Images.CollectionChanged += Images_CollectionChanged;
    }

    private void Images_CollectionChanged(
        object sender,
        System.Collections.Specialized.NotifyCollectionChangedEventArgs e
    )
    {
        StopCommand_ExecuteEvent();
    }

    private void RemoveImageCommand_ExecuteEvent(AnimeModel value)
    {
        CurrentImageModel.Value.Close();
        value.Images.Remove(CurrentImageModel.Value);
    }

    private void RemoveAnimeCommand_ExecuteEvent(AnimeModel value)
    {
        if (
            MessageBox.Show("确定删除吗".Translate(), "", MessageBoxButton.YesNo) is MessageBoxResult.Yes
        )
        {
            if (CurrentMode is GameSave.ModeType.Happy)
                Anime.Value.HappyAnimes.Remove(value);
            else if (CurrentMode is GameSave.ModeType.Nomal)
                Anime.Value.NomalAnimes.Remove(value);
            else if (CurrentMode is GameSave.ModeType.PoorCondition)
                Anime.Value.PoorConditionAnimes.Remove(value);
            else if (CurrentMode is GameSave.ModeType.Ill)
                Anime.Value.IllAnimes.Remove(value);
            value.Close();
        }
    }

    private void ClearImageCommand_ExecuteEvent(AnimeModel value)
    {
        if (
            MessageBox.Show("确定清空吗".Translate(), "", MessageBoxButton.YesNo) is MessageBoxResult.Yes
        )
        {
            value.Close();
            value.Images.Clear();
        }
    }

    private void AddImageCommand_ExecuteEvent(AnimeModel value)
    {
        OpenFileDialog openFileDialog =
            new()
            {
                Title = "选择图片".Translate(),
                Filter = $"图片|*.jpg;*.jpeg;*.png;*.bmp".Translate()
            };
        if (openFileDialog.ShowDialog() is true)
        {
            value.Images.Add(new(Utils.LoadImageToStream(openFileDialog.FileName)));
        }
    }

    private void StopCommand_ExecuteEvent()
    {
        if (_playing is false)
            return;
        Reset();
    }

    private void PlayCommand_ExecuteEvent()
    {
        if (_playing)
        {
            MessageBox.Show("正在播放".Translate());
            return;
        }
        _playing = true;
        _playerTask.Start();
    }

    private void Play()
    {
        do
        {
            foreach (var model in CurrentAnimeModel.Value.Images)
            {
                CurrentImageModel.Value = model;
                Task.Delay(model.Duration.Value).Wait();
                if (_playing is false)
                    return;
            }
        } while (Loop.Value);
        Reset();
    }

    private void Reset()
    {
        _playing = false;
        _playerTask = new(Play);
    }
}
