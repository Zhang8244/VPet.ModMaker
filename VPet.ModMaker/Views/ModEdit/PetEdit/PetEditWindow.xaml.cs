﻿using LinePutScript.Localization.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VPet.ModMaker.Models;
using VPet.ModMaker.ViewModels.ModEdit.PetEdit;

namespace VPet.ModMaker.Views.ModEdit.PetEdit;

/// <summary>
/// PetEditWindow.xaml 的交互逻辑
/// </summary>
public partial class PetEditWindow : Window
{
    public PetEditWindowVM ViewModel => (PetEditWindowVM)DataContext;
    public bool IsCancel { get; private set; } = true;

    public PetEditWindow()
    {
        DataContext = new PetEditWindowVM();
        InitializeComponent();
        Closed += (s, e) =>
        {
            if (IsCancel)
                ViewModel.Close();
        };
    }

    private void Button_Cancel_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void Button_Yes_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(ViewModel.Pet.Value.Name.Value))
        {
            MessageBox.Show("Id不可为空".Translate(), "", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        if (string.IsNullOrWhiteSpace(ViewModel.Pet.Value.CurrentI18nData.Value.Name.Value))
        {
            MessageBox.Show("名称不可为空".Translate(), "", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        if (string.IsNullOrWhiteSpace(ViewModel.Pet.Value.CurrentI18nData.Value.PetName.Value))
        {
            MessageBox.Show(
                "宠物名称不可为空".Translate(),
                "",
                MessageBoxButton.OK,
                MessageBoxImage.Warning
            );
            return;
        }
        if (
            ViewModel.OldPet?.Name.Value != ViewModel.Pet.Value.Name.Value
            && ModInfoModel.Current.Pets.Any(i => i.Name == ViewModel.Pet.Value.Name)
        )
        {
            MessageBox.Show("此Id已存在", "", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        IsCancel = false;
        Close();
    }
}
