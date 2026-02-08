using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.Foundation;

namespace EsUtil.WinUI3.Controls.MultiColumnPanelSample;


public readonly record struct SampleItem(string Label, double Height = double.NaN);


public partial class SamplePage : Page, INotifyPropertyChanged
{
    public IReadOnlyList<SampleItem> ItemsSource { get; } = s_defs;
    private static readonly SampleItem[] s_defs =
    [
        new("丸括弧", 30),
        new("角括弧", 35),
        new("波括弧", 40),
        new("ダブルクォート", 45),
        new("シングルクォート", 50),
        new("カンマ", 55),
        new("ピリオド", 60),
        new("コロン", 65),
        new("セミコロン", 70),
        new("不等号", 30),
        new("イコール", 35),
        new("不等号", 40),
        new("プラス", 45),
        new("マイナス", 50),
        new("はてな", 55),
        new("びっくり", 60),
        new("シャープ", 65),
        new("ダラー", 70),
        new("パーセント", 30),
        new("アンパサンド", 35),
        new("アスタリスク", 40),
        new("スラッシュ", 45),
        new("アットマーク", 50),
        new("キャレット", 55),
        new("アンダースコア", 60),
        new("バッククォート", 65),
        new("縦棒", 70),
        new("チルダ", 30)
    ];

    public SamplePage()
    {
        InitializeComponent();
    }

    public bool IsEnabledExpanderItems
    {
        get;
        set { SetProperty(ref field, value); }
    } = true;


    #region PropertyChanged, OnPropertyChanged, SetProperty

    /// <summary>
    /// プロパティ変更イベント。
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// プロパティ変更を通知します。
    /// </summary>
    /// <param name="propertyName">変更されたプロパティ名。</param>
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    /// <summary>
    /// プロパティの値を設定し、変更を通知します。
    /// </summary>
    /// <typeparam name="T">プロパティの型。</typeparam>
    /// <param name="backingField">バッキングフィールド。</param>
    /// <param name="value">新しい値。</param>
    /// <param name="propertyName">プロパティ名。</param>
    /// <returns>値が変更された場合は true。</returns>
    protected bool SetProperty<T>(ref T backingField, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(backingField, value))
            return false;
        backingField = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    #endregion PropertyChanged, OnPropertyChanged, SetProperty
}


public static class Helper
{
    /// <summary>
    /// 親コンテナに UIElement を追加するヘルパーメソッド
    /// 最初に見つかった Panel に要素を追加します。
    /// </summary>
    /// <param name="element">基準となる要素</param>
    /// <param name="elementToAdd">追加する UIElement</param>
    /// <param name="zIndex">Z-Index（デフォルト: 1000）</param>
    public static void AttachElementToAncestor(
        DependencyObject element,
        UIElement? elementToAdd,
        int zIndex = 1000)
    {
        if (elementToAdd == null)
            return;

        // Panel 型の祖先を自動検出（型指定なし）
        var parentPanel = FindAncestor<Panel>(element);

        // 見つけた親 Panel に要素を追加
        if (parentPanel != null && !parentPanel.Children.Contains(elementToAdd))
        {
            parentPanel.Children.Add(elementToAdd);
            Canvas.SetZIndex(elementToAdd, zIndex);
        }
    }

    /// <summary>
    /// 指定された型の祖先要素を探索します（ジェネリック版）
    /// </summary>
    /// <typeparam name="T">探索する祖先の型。DependencyObject を継承している必要があります。</typeparam>
    /// <param name="current">探索を開始する要素。null の場合は null を返します。</param>
    /// <returns>
    /// 見つかった最初の祖先要素。見つからない場合は null。<br/>
    /// ビジュアルツリーを上向きに辿り、最初に型 T にマッチした要素を返します。
    /// </returns>
    /// <remarks>
    /// 【探索フロー】<br/>
    /// current → GetParent → GetParent → ... → T型が見つかるか Root に到達<br/>
    /// <br/>
    /// 【使用例】
    /// <code>
    /// // Grid の祖先を探索
    /// var parentGrid = FindAncestor&lt;Grid&gt;(this);
    /// 
    /// // Panel の祖先を探索（任意の Panel に対応）
    /// var parentPanel = FindAncestor&lt;Panel&gt;(this);
    /// </code>
    /// </remarks>
    public static T? FindAncestor<T>(DependencyObject? current) where T : DependencyObject
    {
        while (current != null)
        {
            current = VisualTreeHelper.GetParent(current);
            if (current is T t)
                return t;
        }
        return null;
    }

}


/// <summary>
/// FrameworkElement に対するレイアウト関連の拡張メソッド群。
/// 可読性・簡潔さ・モダンなC#記法を優先し、幅計算の汎用性を重視。
/// </summary>
public static class FrameworkElementExtensions
{
    /// <summary>
    /// 要素の内側サイズ（ボーダーとパディングを除いたサイズ）を取得します。<br/>
    /// ActualWidth と ActualHeight からボーダー厚さとパディングを差し引いたサイズを計算します。
    /// </summary>
    /// <param name="element">対象の FrameworkElement。</param>
    /// <returns>内側サイズ。無効な場合はデフォルトサイズ。</returns>
    public static Size GetInnerSize(this FrameworkElement element)
    {
        ArgumentNullException.ThrowIfNull(element);

        var padding = GetPadding(element);
        var border = GetBorderThickness(element);

        return new Size(
            element.ActualWidth - (padding.Left + padding.Right + border.Left + border.Right),
            element.ActualHeight - (padding.Top + padding.Bottom + border.Top + border.Bottom)
        );
    }

    /// <summary>
    /// 要素の BorderThickness によるサイズ消費量を取得します。
    /// </summary>
    /// <param name="e">調査対象の FrameworkElement</param>
    /// <returns>BorderThickness (Thickness)</returns>
    public static Thickness GetBorderThickness(this FrameworkElement e) => e switch
    {
        Border b => b.BorderThickness,
        _ => default,
    };

    /// <summary>
    /// 要素の Padding によるサイズ消費量を取得します。
    /// </summary>
    /// <param name="e">調査対象の FrameworkElement</param>
    /// <returns>Padding (Thickness)</returns>
    public static Thickness GetPadding(this FrameworkElement e) => e switch
    {
        Control c => c.Padding,
        Border b => b.Padding,
        _ => default
    };

    /// <summary>
    /// Thickness をフォーマットします。
    /// </summary>
    public static string Format(this Thickness thickness) =>
        thickness is { Left: var l, Top: var t, Right: var r, Bottom: var b } &&
        l == t && r == b && l == r
            ? $"{l:F1}"
            : l == r && t == b
                ? $"{l:F1}, {t:F1}"
                : $"L:{l:F1}, T:{t:F1}, R:{r:F1}, B:{b:F1}";

    /// <summary>
    /// Size をフォーマットします。
    /// </summary>
    public static string Format(this Size size, string separator = ", ") =>
        $"{(double.IsNaN(size.Width) ? "Auto" : $"{size.Width:F1}")}{separator}{(double.IsNaN(size.Height) ? "Auto" : $"{size.Height:F1}")}";

}
