﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using HKW.HKWReactiveUI;
using HKW.HKWUtils;
using HKW.HKWUtils.Observable;
using HKW.WPF;
using HKW.WPF.Extensions;
using VPet.ModMaker.Native;
using VPet.ModMaker.ViewModels;

namespace VPet.ModMaker.Models.ModModel;

/// <summary>
/// 图像模型
/// </summary>
public partial class ImageModel : ViewModelBase, ICloneable<ImageModel>
{
    public ImageModel(string imageFile, int duration = 100)
    {
        ImageFile = imageFile;
        Duration = duration;
    }

    public ImageModel(BitmapImage image, int duration = 100)
    {
        Image = image;
        Duration = duration;
    }

    /// <summary>
    /// 图片路径
    /// </summary>
    public string ImageFile { get; set; } = string.Empty;

    /// <summary>
    /// 图像
    /// </summary>
    [ReactiveProperty]
    public BitmapImage Image { get; set; } = null!;

    /// <summary>
    /// 持续时间
    /// </summary>
    [ReactiveProperty]
    public int Duration { get; set; } = 100;

    public void LoadImage()
    {
        Image = HKWImageUtils.LoadImageToMemory(ImageFile, this)!;
    }

    public ImageModel Clone()
    {
        var model = new ImageModel(
            Image?.CloneStream() ?? HKWImageUtils.LoadImageToMemory(ImageFile, this)!,
            Duration
        );
        return model;
    }

    object ICloneable.Clone() => Clone();

    public void Close()
    {
        Image?.CloseStream();
    }
}
