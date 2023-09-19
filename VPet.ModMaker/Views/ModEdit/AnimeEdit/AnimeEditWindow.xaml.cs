﻿using System;
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
using VPet.ModMaker.Models.ModModel;
using VPet.ModMaker.ViewModels.ModEdit.AnimeEdit;
using VPet_Simulator.Core;

namespace VPet.ModMaker.Views.ModEdit.AnimeEdit;

/// <summary>
/// AnimeEditWindow.xaml 的交互逻辑
/// </summary>
public partial class AnimeEditWindow : Window
{
    public AnimeEditWindow()
    {
        DataContext = new AnimeEditWindowVM();
        InitializeComponent();
        Closed += (s, e) =>
        {
            try
            {
                DataContext = null;
            }
            catch { }
        };
    }

    public AnimeEditWindowVM ViewModel => (AnimeEditWindowVM)DataContext;

    public bool IsCancel { get; private set; } = true;

    private void Button_Cancel_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void Button_Yes_Click(object sender, RoutedEventArgs e)
    {
        //if (string.IsNullOrEmpty(ViewModel.Work.Value.Id.Value))
        //{
        //    MessageBox.Show("Id不可为空".Translate(), "", MessageBoxButton.OK, MessageBoxImage.Warning);
        //    return;
        //}
        //if (string.IsNullOrEmpty(ViewModel.Work.Value.Graph.Value))
        //{
        //    MessageBox.Show(
        //        "指定动画Id不可为空".Translate(),
        //        "",
        //        MessageBoxButton.OK,
        //        MessageBoxImage.Warning
        //    );
        //    return;
        //}
        //if (
        //    ViewModel.OldWork?.Id.Value != ViewModel.Work.Value.Id.Value
        //    && ViewModel.CurrentPet.Works.Any(i => i.Id.Value == ViewModel.Work.Value.Id.Value)
        //)
        //{
        //    MessageBox.Show("此Id已存在".Translate(), "", MessageBoxButton.OK, MessageBoxImage.Warning);
        //    return;
        //}
        IsCancel = false;
        Close();
    }

    //private void ListBox_Drop(object sender, DragEventArgs e)
    //{
    //    var fileName = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
    //}

    private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (
            sender is not TabControl tabControl
            || tabControl.SelectedItem is not TabItem item
            || item.Tag is not string str
        )
            return;
        if (Enum.TryParse<GameSave.ModeType>(str, true, out var mode))
            ViewModel.CurrentMode = mode;
    }

    private void ListBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
        {
            RoutedEvent = MouseWheelEvent,
            Source = sender
        };
        var parent = ((Control)sender).Parent as UIElement;
        parent.RaiseEvent(eventArg);
        e.Handled = true;
    }

    private object _dropSender;

    private void ListBox_PreviewMouseMove(object sender, MouseEventArgs e)
    {
        if (sender is not ListBox listBox)
            return;
        if (e.LeftButton != MouseButtonState.Pressed)
            return;
        var pos = e.GetPosition(listBox);
        HitTestResult result = VisualTreeHelper.HitTest(listBox, pos);
        if (result is null)
            return;
        var listBoxItem = FindVisualParent<ListBoxItem>(result.VisualHit);
        if (listBoxItem == null || listBoxItem.Content != listBox.SelectedItem)
            return;
        var dataObj = new DataObject(listBoxItem.Content);
        DragDrop.DoDragDrop(listBox, dataObj, DragDropEffects.Move);
        _dropSender = sender;
    }

    private void ListBox_Drop(object sender, DragEventArgs e)
    {
        if (sender.Equals(_dropSender) is false)
        {
            MessageBox.Show("无法移动不同动画的图片");
            return;
        }
        if (sender is not ListBox listBox)
            return;
        var pos = e.GetPosition(listBox);
        var result = VisualTreeHelper.HitTest(listBox, pos);
        if (result == null)
            return;
        //查找元数据
        if (e.Data.GetData(typeof(ImageModel)) is not ImageModel sourcePerson)
            return;
        //查找目标数据
        var listBoxItem = FindVisualParent<ListBoxItem>(result.VisualHit);
        if (listBoxItem == null)
            return;
        var targetPerson = listBoxItem.Content as ImageModel;
        if (ReferenceEquals(targetPerson, sourcePerson))
            return;
        if (listBox.ItemsSource is not IList<ImageModel> list)
            return;
        var sourceIndex = list.IndexOf(sourcePerson);
        var targetIndex = list.IndexOf(targetPerson);
        var temp = list[sourceIndex];
        list[sourceIndex] = list[targetIndex];
        list[targetIndex] = temp;
    }

    public static T? FindVisualChild<T>(DependencyObject obj)
        where T : DependencyObject
    {
        if (obj is null)
            return null;
        var count = VisualTreeHelper.GetChildrenCount(obj);
        for (int i = 0; i < count; i++)
        {
            var child = VisualTreeHelper.GetChild(obj, i);
            if (child is T t)
                return t;
            if (FindVisualChild<T>(child) is T childItem)
                return childItem;
        }
        return null;
    }

    public static T FindVisualParent<T>(DependencyObject obj)
        where T : class
    {
        while (obj != null)
        {
            if (obj is T)
                return obj as T;
            obj = VisualTreeHelper.GetParent(obj);
        }
        return null;
    }

    private void Button_AddAnime_Click(object sender, RoutedEventArgs e)
    {
        if (ViewModel.CurrentMode is GameSave.ModeType.Happy)
            ViewModel.Anime.Value.HappyAnimes.Add(new());
        else if (ViewModel.CurrentMode is GameSave.ModeType.Nomal)
            ViewModel.Anime.Value.NomalAnimes.Add(new());
        else if (ViewModel.CurrentMode is GameSave.ModeType.PoorCondition)
            ViewModel.Anime.Value.PoorConditionAnimes.Add(new());
        else if (ViewModel.CurrentMode is GameSave.ModeType.Ill)
            ViewModel.Anime.Value.IllAnimes.Add(new());
    }

    private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is not ListBox listBox)
            return;
        if (listBox.DataContext is AnimeModel model)
            ViewModel.CurrentAnimeModel.Value = model;
    }
}
