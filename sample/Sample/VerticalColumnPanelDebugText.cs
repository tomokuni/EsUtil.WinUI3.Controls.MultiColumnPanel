using EsUtil.WinUI3.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Windows.Foundation;

namespace EsUtil.WinUI3.Controls.MultiColumnPanelSample;


/// <summary>
/// MultiColumnPanel を継承したコントロール。
/// デバッグ情報表示機能を備えています。
/// </summary>
[ContentProperty(Name = nameof(Children))]
public partial class MultiColumnPanelDebugText : MultiColumnPanel, INotifyPropertyChanged
{
    /// <summary>
    /// デバッグ情報表示エリア
    /// </summary>
    public Border DebugInfoArea { get; } = new Border
    {
        BorderThickness = new Thickness(1),
        BorderBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(70, 255, 0, 0)),
        CornerRadius = new CornerRadius(8),
        Padding = new Thickness(8, 4, 8, 4),
        Margin = new Thickness(8),
        Background = new SolidColorBrush(Windows.UI.Color.FromArgb(200, 255, 255, 255)),
        Visibility = Visibility.Collapsed,
        VerticalAlignment = VerticalAlignment.Top,
        HorizontalAlignment = HorizontalAlignment.Stretch,
        Child = new TextBlock
        {
            TextWrapping = TextWrapping.Wrap,
            FontSize = 12,
            Text = "待機中...",
        },
    };

    public MultiColumnPanelDebugText()
    {
        this.Loaded += OnLoaded;
        this.SizeChanged += OnSizeChanged;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        // デバッグ情報 のテキスト作成
        UpdateDebugInfo();
        // デバッグ情報表示エリア を直近の祖先に追加
        Helper.AttachElementToAncestor(this, DebugInfoArea);
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        // デバッグ情報 のテキスト作成
        UpdateDebugInfo();
    }

    public void UpdateDebugInfo()
    {
        // DebugInfoArea のテキストを更新
        (DebugInfoArea.Child as TextBlock)?.Text = GetDebugText();
    }

    public string GetDebugText()
    {
        // コントロール自身の情報を取得
        var text = $"【このコントロール  {GetType().Name}】";
        text += $"    描画可能な内側サイズ（概算）: ({this.GetInnerSize().Format()})\n";
        text += $"    Margin: ({this.Margin.Format()})";
        text += $"   |   Size: ({new Size(this.Width, this.Height).Format()})";
        text += $"   |   ActualSize: ({new Size(this.ActualWidth, this.ActualHeight).Format()})";
        text += $"   |   Padding: ({this.GetPadding().Format()})";

        // レイアウト結果情報を追加
        if (Layout is not null)
        {
            text += $"\n\n【レイアウト結果】\n";
            text += $"    MinHeight: {LayoutMinHeight:F25}";
            text += $"   |   UsedWidth: {LayoutUsedWidth:F1}";
            text += $"   |   列数: {LayoutColumnCount}";
            text += $"   |   ColumnSegments(EndIdx): {LayoutColumnSegments}";
        }

        return text;
    }


    #region DependencyProperty(DebugTextVisibility, ShowDebugText)

    /// <summary>
    /// DebugTextVisibility 依存関係プロパティ。
    /// デバッグテキストボーダーの表示/非表示を制御します。
    /// </summary>
    public static readonly DependencyProperty DebugTextVisibilityProperty =
        DependencyProperty.Register(
            nameof(DebugTextVisibility),
            typeof(Visibility),
            typeof(MultiColumnPanelDebugText),
            new PropertyMetadata(Visibility.Collapsed, OnDebugTextVisibilityChanged));

    private static void OnDebugTextVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        // DebugInfoArea の Visibility を更新
        var panel = (MultiColumnPanelDebugText)d;
        panel.DebugInfoArea?.Visibility = (Visibility)e.NewValue;
    }

    /// <summary>
    /// DebugTextVisibility プロパティ。
    /// デバッグテキストボーダーの表示状態を取得または設定します。
    /// _debugBorder.Visibility と同期しています。
    /// </summary>
    public Visibility DebugTextVisibility
    {
        get => DebugInfoArea?.Visibility ?? (Visibility)GetValue(DebugTextVisibilityProperty);
        set => SetValue(DebugTextVisibilityProperty, value);
    }

    /// <summary>
    /// ShowDebugText プロパティ。
    /// デバッグ情報を表示するかどうかを取得または設定します。
    /// 内部的に DebugTextVisibility を Visibility.Visible/Collapsed に変換します。
    /// </summary>
    public bool ShowDebugText
    {
        get => DebugTextVisibility == Visibility.Visible;
        set => DebugTextVisibility = value ? Visibility.Visible : Visibility.Collapsed;
    }

    #endregion DependencyProperty(DebugTextVisibility, ShowDebugText)

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
