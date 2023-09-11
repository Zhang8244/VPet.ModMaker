﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWViewModels.SimpleObservable;

/// <summary>
/// 可观察值
/// </summary>
/// <typeparam name="T"></typeparam>
[DebuggerDisplay("{EnumValue}")]
public class ObservableValue<T> : INotifyPropertyChanging, INotifyPropertyChanged
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private T _value = default!;

    /// <summary>
    /// 当前值
    /// </summary>
    public T Value
    {
        get => _value;
        set
        {
            if (_value?.Equals(value) is true)
                return;
            NotifyPropertyChanging(_value, value);
            _value = value;
            NotifyPropertyChanged(_value, value);
        }
    }

    #region Ctor
    /// <inheritdoc/>
    public ObservableValue() { }

    /// <inheritdoc/>
    /// <param name="value">初始值</param>
    public ObservableValue(T value)
    {
        _value = value;
    }
    #endregion

    #region NotifyProperty
    /// <summary>
    /// 通知属性改变前
    /// </summary>
    /// <param name="oldValue">旧值</param>
    /// <param name="newValue">新值</param>
    private void NotifyPropertyChanging(T oldValue, T newValue)
    {
        PropertyChanging?.Invoke(this, new(nameof(Value)));
        ValueChanging?.Invoke(oldValue, newValue);
    }

    /// <summary>
    /// 通知属性改变后
    /// </summary>
    /// <param name="oldValue">旧值</param>
    /// <param name="newValue">新值</param>
    private void NotifyPropertyChanged(T oldValue, T newValue)
    {
        PropertyChanged?.Invoke(this, new(nameof(Value)));
        ValueChanged?.Invoke(oldValue, newValue);
    }
    #endregion

    #region Overwrite
    /// <inheritdoc/>
    public override string ToString()
    {
        return Value?.ToString()!;
    }
    #endregion

    #region NotifyReceiver
    /// <summary>
    /// 添加通知属性改变后接收器
    /// <para>
    /// 添加的接口触发后会执行 <see cref="NotifyReceived"/>
    /// </para>
    /// <para>示例:
    /// <code><![CDATA[
    /// ObservableValue<string> value1 = new();
    /// ObservableValue<string> value2 = new();
    /// value2.AddNotifyReceiver(value1);
    /// value2.NotifyReceived += (ref string v) =>
    /// {
    ///     v = "B"; // trigger this
    /// };
    /// value1.EnumValue = "A"; // execute this
    /// // result: value1.EnumValue == "A" , value2.EnumValue == "B"
    /// ]]>
    /// </code></para>
    /// </summary>
    /// <param name="notifies">通知属性改变后接口</param>
    public void AddNotifyReceiver(params INotifyPropertyChanged[] notifies)
    {
        foreach (var notify in notifies)
            notify.PropertyChanged += Notify_PropertyChanged;
    }

    /// <summary>
    /// 删除通知属性改变后接收器
    /// </summary>
    /// <param name="notifies">通知属性改变后接口</param>
    public void RemoveNotifyReceiver(params INotifyPropertyChanged[] notifies)
    {
        foreach (var notify in notifies)
            notify.PropertyChanged -= Notify_PropertyChanged;
    }

    private void Notify_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        var temp = Value;
        NotifyReceived?.Invoke(ref temp);
        Value = temp;
    }
    #endregion

    #region Event
    /// <summary>
    /// 属性改变前事件
    /// </summary>
    public event PropertyChangingEventHandler? PropertyChanging;

    /// <summary>
    /// 属性改变后事件
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// 值改变前事件
    /// </summary>
    public event ValueChangeEventHandler? ValueChanging;

    /// <summary>
    /// 值改变后事件
    /// </summary>
    public event ValueChangeEventHandler? ValueChanged;

    /// <summary>
    /// 通知接收器事件
    /// </summary>
    public event NotifyReceivedHandler? NotifyReceived;
    #endregion

    #region Delegate
    /// <summary>
    /// 值改变事件
    /// </summary>
    /// <param name="oldValue">旧值</param>
    /// <param name="newValue">新值</param>
    public delegate void ValueChangeEventHandler(T oldValue, T newValue);

    /// <summary>
    /// 通知接收器
    /// </summary>
    /// <param name="value">引用值</param>
    public delegate void NotifyReceivedHandler(ref T value);
    #endregion
}
