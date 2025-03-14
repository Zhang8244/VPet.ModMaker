﻿using System.IO;
using System.Windows.Media.Imaging;
using HKW.HKWReactiveUI;
using HKW.WPF;
using HKW.WPF.Extensions;
using LinePutScript.Converter;
using VPet.ModMaker.ViewModels;

namespace VPet.ModMaker.Models;

/// <summary>
/// 模组制作历史
/// </summary>
public class ModMakeHistory : IEquatable<ModMakeHistory>, IEnableLogger<ViewModelBase>
{
    /// <inheritdoc/>
    public ModMakeHistory() { }

    /// <summary>
    /// 图片
    /// </summary>
    public BitmapImage? Image { get; set; }

    /// <summary>
    /// Id
    /// </summary>
    [Line(ignoreCase: true)]
    public string ID { get; set; } = string.Empty;

    /// <summary>
    /// 路径
    /// </summary>
    private string _sourcePath = string.Empty;

    /// <summary>
    /// 资源路径
    /// </summary>
    [Line(ignoreCase: true)]
    public string SourcePath
    {
        get => _sourcePath;
        set
        {
            if (string.IsNullOrWhiteSpace(_sourcePath) is false)
                Image?.CloseStreamWhenNoReference();
            _sourcePath = value;
            var imagePath = Path.Combine(_sourcePath, "icon.png");

            if (File.Exists(imagePath) is false)
                this.LogX().Warn("目标文件不存在, 路径: {path}", imagePath);
            else
                Image = HKWImageUtils.LoadImageToMemory(imagePath);
        }
    }

    /// <summary>
    /// 模组信息文件
    /// </summary>
    public string InfoFile => Path.Combine(SourcePath, "info.lps");

    /// <summary>
    /// 最后编辑时间
    /// </summary>
    [Line(ignoreCase: true)]
    public DateTime LastTime { get; set; } = DateTime.Now;

    #region IEquatable
    /// <inheritdoc/>
    public bool Equals(ModMakeHistory? other)
    {
        return SourcePath.Equals(other?.SourcePath);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return Equals(obj as ModMakeHistory);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return SourcePath.GetHashCode();
    }
    #endregion
}