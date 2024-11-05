﻿using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using HKW.HKWMapper;
using HKW.HKWReactiveUI;
using HKW.HKWUtils.Observable;
using VPet.ModMaker.ViewModels;
using VPet_Simulator.Windows.Interface;

namespace VPet.ModMaker.Models;

/// <summary>
/// 低状态文本
/// </summary>
[MapTo(typeof(LowText))]
[MapFrom(typeof(LowText))]
[MapFrom(typeof(LowTextModel))]
public partial class LowTextModel : ViewModelBase
{
    public LowTextModel() { }

    public LowTextModel(LowTextModel lowText)
        : this()
    {
        this.MapFromLowTextModel(lowText);
    }

    public LowTextModel(LowText lowText)
        : this()
    {
        this.MapFromLowText(lowText);
    }

    /// <summary>
    /// 状态类型
    /// </summary>
    public static FrozenSet<LowText.ModeType> ModeTypes => EnumInfo<LowText.ModeType>.Values;

    /// <summary>
    /// 好感度类型
    /// </summary>
    public static FrozenSet<LowText.LikeType> LikeTypes => EnumInfo<LowText.LikeType>.Values;

    /// <summary>
    /// 体力类型
    /// </summary>
    public static FrozenSet<LowText.StrengthType> StrengthTypes =>
        EnumInfo<LowText.StrengthType>.Values;

    /// <summary>
    /// ID
    /// </summary>
    [LowTextModelMapToLowTextProperty(nameof(LowText.Text))]
    [LowTextModelMapFromLowTextProperty(nameof(LowText.Text))]
    [ReactiveProperty]
    public string ID { get; set; } = string.Empty;

    [MapIgnoreProperty]
    [ReactiveProperty]
    public required I18nResource<string, string> I18nResource { get; set; }

    partial void OnI18nResourceChanged(
        I18nResource<string, string> oldValue,
        I18nResource<string, string> newValue
    )
    {
        oldValue?.I18nObjects.Remove(I18nObject);
        newValue?.I18nObjects?.Add(I18nObject);
    }

    [MapIgnoreProperty]
    [NotifyPropertyChangeFrom("")]
    public I18nObject<string, string> I18nObject => new(this);

    [MapIgnoreProperty]
    [ReactiveI18nProperty("I18nResource", nameof(I18nObject), nameof(ID))]
    public string Text
    {
        get => I18nResource.GetCurrentCultureDataOrDefault(ID);
        set => I18nResource.SetCurrentCultureData(ID, value);
    }

    /// <summary>
    /// 状态
    /// </summary>
    [LowTextModelMapToLowTextProperty(nameof(LowText.Mode))]
    [LowTextModelMapFromLowTextProperty(nameof(LowText.Mode))]
    [ReactiveProperty]
    public LowText.ModeType Mode { get; set; }

    /// <summary>
    /// 体力
    /// </summary>
    [LowTextModelMapToLowTextProperty(nameof(LowText.Strength))]
    [LowTextModelMapFromLowTextProperty(nameof(LowText.Strength))]
    [ReactiveProperty]
    public LowText.StrengthType Strength { get; set; }

    /// <summary>
    /// 好感度
    /// </summary>
    [LowTextModelMapToLowTextProperty(nameof(LowText.Like))]
    [LowTextModelMapFromLowTextProperty(nameof(LowText.Like))]
    [ReactiveProperty]
    public LowText.LikeType Like { get; set; }

    public void Close()
    {
        I18nResource.I18nObjects.Remove(I18nObject);
    }
}
