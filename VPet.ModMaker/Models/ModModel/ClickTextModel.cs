﻿using HKW.HKWViewModels.SimpleObservable;
using LinePutScript.Converter;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VPet_Simulator.Windows.Interface;

namespace VPet.ModMaker.Models;

/// <summary>
/// 点击文本模型
/// </summary>
public class ClickTextModel : I18nModel<I18nClickTextModel>
{
    /// <summary>
    /// 模式类型
    /// </summary>
    public static ObservableCollection<ClickText.ModeType> ModeTypes { get; } =
        new(Enum.GetValues(typeof(ClickText.ModeType)).Cast<ClickText.ModeType>());

    /// <summary>
    /// 日期区间
    /// </summary>
    public static ObservableCollection<ClickText.DayTime> DayTimes { get; } =
        new(Enum.GetValues(typeof(ClickText.DayTime)).Cast<ClickText.DayTime>());

    /// <summary>
    /// 工作状态
    /// </summary>
    public static ObservableCollection<VPet_Simulator.Core.Main.WorkingState> WorkingStates { get; } =
        new(
            Enum.GetValues(typeof(VPet_Simulator.Core.Main.WorkingState))
                .Cast<VPet_Simulator.Core.Main.WorkingState>()
        );

    /// <summary>
    /// Id
    /// </summary>
    public ObservableValue<string> Id { get; } = new();

    /// <summary>
    /// 指定工作
    /// </summary>
    public ObservableValue<string> Working { get; } = new();

    /// <summary>
    /// 模式
    /// </summary>
    public ObservableEnumFlags<ClickText.ModeType> Mode { get; } = new();

    /// <summary>
    /// 工作状态
    /// </summary>
    public ObservableValue<VPet_Simulator.Core.Main.WorkingState> WorkingState { get; } = new();

    /// <summary>
    /// 日期区间
    /// </summary>
    public ObservableEnumFlags<ClickText.DayTime> DayTime { get; } = new();

    /// <summary>
    /// 好感度
    /// </summary>
    public ObservableRange<double> Like { get; } = new(0, int.MaxValue);

    /// <summary>
    /// 健康值
    /// </summary>
    public ObservableRange<double> Health { get; } = new(0, int.MaxValue);

    /// <summary>
    /// 等级
    /// </summary>
    public ObservableRange<double> Level { get; } = new(0, int.MaxValue);

    /// <summary>
    /// 金钱
    /// </summary>
    public ObservableRange<double> Money { get; } = new(int.MinValue, int.MaxValue);

    /// <summary>
    /// 饱食度
    /// </summary>
    public ObservableRange<double> Food { get; } = new(0, int.MaxValue);

    /// <summary>
    /// 口渴度
    /// </summary>
    public ObservableRange<double> Drink { get; } = new(0, int.MaxValue);

    /// <summary>
    /// 心情
    /// </summary>
    public ObservableRange<double> Feel { get; } = new(0, int.MaxValue);

    /// <summary>
    /// 体力
    /// </summary>
    public ObservableRange<double> Strength { get; } = new(0, int.MaxValue);

    public ClickTextModel() { }

    public ClickTextModel(ClickTextModel clickText)
        : this()
    {
        Id.Value = clickText.Id.Value;
        Mode.EnumValue.Value = clickText.Mode.EnumValue.Value;
        Working.Value = clickText.Working.Value;
        WorkingState.Value = clickText.WorkingState.Value;
        DayTime.EnumValue.Value = clickText.DayTime.EnumValue.Value;
        Like = clickText.Like.Copy();
        Health = clickText.Health.Copy();
        Level = clickText.Level.Copy();
        Money = clickText.Money.Copy();
        Food = clickText.Food.Copy();
        Drink = clickText.Drink.Copy();
        Feel = clickText.Feel.Copy();
        Strength = clickText.Strength.Copy();
        foreach (var item in clickText.I18nDatas)
            I18nDatas[item.Key] = item.Value.Copy();
        CurrentI18nData.Value = I18nDatas[I18nHelper.Current.CultureName.Value];
    }

    public ClickTextModel(ClickText clickText)
        : this()
    {
        Id.Value = clickText.Text;
        Mode.EnumValue.Value = clickText.Mode;
        Working.Value = clickText.Working;
        WorkingState.Value = clickText.State;
        DayTime.EnumValue.Value = clickText.DaiTime;
        Like.SetValue(clickText.LikeMin, clickText.LikeMax);
        Health.SetValue(clickText.HealthMin, clickText.HealthMax);
        Level.SetValue(clickText.LevelMin, clickText.LevelMax);
        Money.SetValue(clickText.MoneyMin, clickText.MoneyMax);
        Food.SetValue(clickText.FoodMin, clickText.FoodMax);
        Drink.SetValue(clickText.DrinkMin, clickText.DrinkMax);
        Feel.SetValue(clickText.FeelMin, clickText.FeelMax);
        Strength.SetValue(clickText.StrengthMin, clickText.StrengthMax);
    }

    public ClickText ToClickText()
    {
        return new()
        {
            Text = Id.Value,
            Mode = Mode.EnumValue.Value,
            Working = Working.Value,
            State = WorkingState.Value,
            DaiTime = DayTime.EnumValue.Value,
            LikeMax = Like.Max.Value,
            LikeMin = Like.Min.Value,
            HealthMin = Health.Min.Value,
            HealthMax = Health.Max.Value,
            LevelMin = Level.Min.Value,
            LevelMax = Level.Max.Value,
            MoneyMin = Money.Min.Value,
            MoneyMax = Money.Max.Value,
            FoodMin = Food.Min.Value,
            FoodMax = Food.Max.Value,
            DrinkMin = Drink.Min.Value,
            DrinkMax = Drink.Max.Value,
            FeelMin = Feel.Min.Value,
            FeelMax = Feel.Max.Value,
            StrengthMin = Strength.Min.Value,
            StrengthMax = Strength.Max.Value,
        };
    }
}

public class I18nClickTextModel
{
    public ObservableValue<string> Text { get; } = new();

    public I18nClickTextModel Copy()
    {
        var result = new I18nClickTextModel();
        result.Text.Value = Text.Value;
        return result;
    }
}
