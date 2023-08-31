﻿using HKW.HKWViewModels.SimpleObservable;
using LinePutScript.Converter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VPet_Simulator.Windows.Interface;

namespace VPet.ModMaker.Models;

public class ClickTextModel : I18nModel<I18nClickTextModel>
{
    public ObservableValue<string> Name { get; } = new();
    public ObservableValue<ClickText.ModeType> Mode { get; } = new();

    public ObservableValue<string> Working { get; } = new();
    public ObservableValue<double> LikeMin { get; } = new();
    public ObservableValue<double> LikeMax { get; } = new();
    public ObservableValue<VPet_Simulator.Core.Main.WorkingState> WorkingState { get; } = new();
    public ObservableValue<ClickText.DayTime> DayTime { get; } = new();

    public ClickTextModel() { }

    public ClickTextModel(ClickTextModel clickText)
        : this()
    {
        Name.Value = clickText.Name.Value;
        Mode.Value = clickText.Mode.Value;
        Working.Value = clickText.Working.Value;
        WorkingState.Value = clickText.WorkingState.Value;
        LikeMax.Value = clickText.LikeMax.Value;
        LikeMin.Value = clickText.LikeMin.Value;
        DayTime.Value = clickText.DayTime.Value;
        foreach (var item in clickText.I18nDatas)
        {
            I18nDatas[item.Key] = new();
            I18nDatas[item.Key].Text.Value = clickText.I18nDatas[item.Key].Text.Value;
        }
        CurrentI18nData.Value = I18nDatas[I18nHelper.Current.CultureName.Value];
    }

    public ClickTextModel(ClickText clickText)
        : this()
    {
        Name.Value = clickText.Text;
        Mode.Value = clickText.Mode;
        Working.Value = clickText.Working;
        WorkingState.Value = clickText.State;
        DayTime.Value = clickText.DaiTime;
        LikeMax.Value = clickText.LikeMax;
        LikeMin.Value = clickText.LikeMin;
    }

    public ClickText ToClickText()
    {
        return new()
        {
            Text = Name.Value,
            Mode = Mode.Value,
            Working = Working.Value,
            State = WorkingState.Value,
            LikeMax = LikeMax.Value,
            LikeMin = LikeMin.Value,
            DaiTime = DayTime.Value,
        };
    }
}

public class I18nClickTextModel
{
    public ObservableValue<string> Text { get; } = new();
}
