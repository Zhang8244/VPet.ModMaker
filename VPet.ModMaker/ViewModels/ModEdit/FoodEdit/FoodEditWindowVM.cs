﻿using HKW.HKWViewModels.SimpleObservable;
using LinePutScript.Localization.WPF;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using VPet.ModMaker.Models;
using VPet_Simulator.Windows.Interface;

namespace VPet.ModMaker.ViewModels.ModEdit.FoodEdit;

public class FoodEditWindowVM
{
    public I18nHelper I18nData => I18nHelper.Current;
    #region Value
    public FoodModel OldFood { get; set; }
    public ObservableValue<FoodModel> Food { get; } = new(new());
    public ObservableValue<bool> AutoSetReferencePrice { get; } = new(false);
    #endregion

    #region Command
    public ObservableCommand AddImageCommand { get; } = new();
    public ObservableCommand ChangeImageCommand { get; } = new();

    public ObservableCommand<double> SetReferencePriceCommand { get; } = new();
    #endregion

    public FoodEditWindowVM()
    {
        AddImageCommand.ExecuteEvent += AddImage;
        ChangeImageCommand.ExecuteEvent += ChangeImage;
        AutoSetReferencePrice.ValueChanged += AutoSetReferencePrice_ValueChanged;
        SetReferencePriceCommand.ExecuteEvent += SetReferencePriceToPrice;
        Food.Value.ReferencePrice.ValueChanged += ReferencePrice_ValueChanged;
    }

    private void AutoSetReferencePrice_ValueChanged(bool oldValue, bool newValue)
    {
        if (newValue)
        {
            SetReferencePriceToPrice(Food.Value.ReferencePrice.Value);
        }
    }

    private void ReferencePrice_ValueChanged(double oldValue, double newValue)
    {
        if (AutoSetReferencePrice.Value)
        {
            SetReferencePriceToPrice(newValue);
        }
    }

    private void SetReferencePriceToPrice(double value)
    {
        Food.Value.Price.Value = value;
    }

    public void Close()
    {
        Food.Value.Close();
    }

    private void AddImage()
    {
        OpenFileDialog openFileDialog =
            new()
            {
                Title = "选择图片".Translate(),
                Filter = $"图片|*.jpg;*.jpeg;*.png;*.bmp".Translate()
            };
        if (openFileDialog.ShowDialog() is true)
        {
            Food.Value.Image.Value = Utils.LoadImageToMemoryStream(openFileDialog.FileName);
        }
    }

    private void ChangeImage()
    {
        OpenFileDialog openFileDialog =
            new()
            {
                Title = "选择图片".Translate(),
                Filter = $"图片|*.jpg;*.jpeg;*.png;*.bmp".Translate()
            };
        if (openFileDialog.ShowDialog() is true)
        {
            Food.Value.Image.Value?.StreamSource?.Close();
            Food.Value.Image.Value = Utils.LoadImageToMemoryStream(openFileDialog.FileName);
        }
    }
}
