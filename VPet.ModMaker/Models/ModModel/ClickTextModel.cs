﻿using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HKW.HKWReactiveUI;
using HKW.HKWUtils;
using HKW.HKWUtils.Observable;
using LinePutScript.Converter;
using Mapster;
using VPet.ModMaker.ViewModels;
using VPet_Simulator.Windows.Interface;

namespace VPet.ModMaker.Models;

/// <summary>
/// 点击文本模型
/// </summary>
public partial class ClickTextModel : ViewModelBase
{
    public ClickTextModel() { }

    public ClickTextModel(ClickTextModel clickText)
        : this()
    {
        ID = clickText.ID;
        Mode.Value = clickText.Mode.Value;
        Working = clickText.Working;
        WorkingState = clickText.WorkingState;
        DayTime.Value = clickText.DayTime.Value;
        Like = clickText.Like.Clone();
        Health = clickText.Health.Clone();
        Level = clickText.Level.Clone();
        Money = clickText.Money.Clone();
        Food = clickText.Food.Clone();
        Drink = clickText.Drink.Clone();
        Feel = clickText.Feel.Clone();
        Strength = clickText.Strength.Clone();
    }

    public ClickTextModel(ClickText clickText)
        : this()
    {
        ID = clickText.Text;
        Mode.Value = clickText.Mode;
        Working = clickText.Working;
        WorkingState = clickText.State;
        DayTime.Value = clickText.DaiTime;
        Like = new(clickText.LikeMin, clickText.LikeMax);
        Health = new(clickText.HealthMin, clickText.HealthMax);
        Level = new(clickText.LevelMin, clickText.LevelMax);
        Money = new(clickText.MoneyMin, clickText.MoneyMax);
        Food = new(clickText.FoodMin, clickText.FoodMax);
        Drink = new(clickText.DrinkMin, clickText.DrinkMax);
        Feel = new(clickText.FeelMin, clickText.FeelMax);
        Strength = new(clickText.StrengthMin, clickText.StrengthMax);
    }

    public ClickText ToClickText()
    {
        return new()
        {
            Text = ID,
            Mode = Mode.Value,
            Working = Working,
            State = WorkingState,
            DaiTime = DayTime.Value,
            LikeMax = Like.Max,
            LikeMin = Like.Min,
            HealthMin = Health.Min,
            HealthMax = Health.Max,
            LevelMin = Level.Min,
            LevelMax = Level.Max,
            MoneyMin = Money.Min,
            MoneyMax = Money.Max,
            FoodMin = Food.Min,
            FoodMax = Food.Max,
            DrinkMin = Drink.Min,
            DrinkMax = Drink.Max,
            FeelMin = Feel.Min,
            FeelMax = Feel.Max,
            StrengthMin = Strength.Min,
            StrengthMax = Strength.Max,
        };
    }

    /// <summary>
    /// 模式类型
    /// </summary>
    public static FrozenSet<ClickText.ModeType> ModeTypes { get; } =
        Enum.GetValues<ClickText.ModeType>().ToFrozenSet();

    /// <summary>
    /// 日期区间
    /// </summary>
    public static FrozenSet<ClickText.DayTime> DayTimes { get; } =
        Enum.GetValues<ClickText.DayTime>().ToFrozenSet();

    /// <summary>
    /// 工作状态
    /// </summary>
    public static FrozenSet<VPet_Simulator.Core.Main.WorkingState> WorkingStates { get; } =
        Enum.GetValues<VPet_Simulator.Core.Main.WorkingState>().ToFrozenSet();

    /// <summary>
    /// ID
    /// </summary>
    [AdaptMember(nameof(ClickText.Text))]
    [ReactiveProperty]
    public string ID { get; set; } = string.Empty;

    #region I18nData
    [AdaptIgnore]
    private I18nResource<string, string> _i18nResource = null!;

    [AdaptIgnore]
    public required I18nResource<string, string> I18nResource
    {
        get => _i18nResource;
        set
        {
            //TODO:
            //if (_i18nResource is not null)
            //    I18nResource.I18nObjectInfos.Remove(this);
            //_i18nResource = value;
            InitializeI18nResource();
        }
    }

    public void InitializeI18nResource()
    {
        //TODO:
        //I18nResource?.I18nObjectInfos.Add(
        //    this,
        //    new I18nObjectInfo<string, string>(this, OnPropertyChanged).AddPropertyInfo(
        //        nameof(ID),
        //        ID,
        //        nameof(Text),
        //        true
        //    )
        //);
    }

    [AdaptIgnore]
    public string Text
    {
        get => I18nResource.GetCurrentCultureDataOrDefault(ID);
        set => I18nResource.SetCurrentCultureData(ID, value);
    }
    #endregion


    /// <summary>
    /// 指定工作
    /// </summary>
    [AdaptMember(nameof(ClickText.Working))]
    [ReactiveProperty]
    public string Working { get; set; } = string.Empty;

    /// <summary>
    /// 宠物状态
    /// </summary>
    public ObservableEnum<ClickText.ModeType> Mode { get; } =
        new(
            ClickText.ModeType.Happy
                | ClickText.ModeType.Nomal
                | ClickText.ModeType.PoorCondition
                | ClickText.ModeType.Ill,
            (v, f) => v |= f,
            (v, f) => v &= f
        );

    /// <summary>
    /// 行动状态
    /// </summary>
    [AdaptMember(nameof(ClickText.State))]
    [ReactiveProperty]
    public VPet_Simulator.Core.Main.WorkingState WorkingState { get; set; }

    /// <summary>
    /// 日期区间
    /// </summary>

    public ObservableEnum<ClickText.DayTime> DayTime { get; } =
        new(
            ClickText.DayTime.Morning
                | ClickText.DayTime.Afternoon
                | ClickText.DayTime.Night
                | ClickText.DayTime.Midnight,
            (v, f) => v |= f,
            (v, f) => v &= f
        );

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

    public void Close()
    {
        //TODO:
        //I18nResource.I18nObjectInfos.Remove(this);
    }
}
