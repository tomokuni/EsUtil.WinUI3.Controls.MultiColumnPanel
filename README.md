# MultiColumnPanel

## 概要
MultiColumnPanel は、WinUI3 向けの抽象パネルクラスで、ItemsSource と XAML で直接追加された子要素の両方を同時に扱う機能を提供します。ItemsControl の ItemsPanel として 100% 置き換え可能な互換性を有します。垂直方向にアイテムを複数列配置し、高さを自動計算するカスタムパネルです。

## 特徴
- **DualSourceItemsPanel から継承（ItemsControl の ItemsPanel として使用可能）**
- **MultiColumnLayout.Solve() の結果を直接反映**
- **行間・列間スペースを制御可能な DependencyProperty**
- **3 種類のアルゴリズム（DynamicProgramming/BinarySearch/Greedy）をサポート**
- **ベースクラスから継承したデバッグプロパティで実行状態をモニタリング可能**

## ベースクラスのデバッグプロパティ
- **LastChildCount**：最後のレイアウト計算時の子要素数
- **IsManagerInitialized**：ItemsManager が初期化済みかどうか
- **IsItemsSourceConnected**：ItemsSource と ItemTemplate が両方設定済みかどうか
- **DirectChildItemsCount**：直接保存された子要素の数

## 計算フロー
1. **MeasureOverride**：各子要素を測定し、最適レイアウトを計算
2. **ArrangeOverride**：計算結果に基づいて各子要素を配置

## 使用方法
MultiColumnPanel を ItemsControl の ItemsPanel として使用します。プロパティを設定してレイアウトを制御します。

### 例
```xaml
<ItemsControl>
    <ItemsControl.ItemsPanel>
        <local:MultiColumnPanel RowSpace="10" ColumnSpace="10" ColumnLimit="5" Method="BinarySearch" />
    </ItemsControl.ItemsPanel>
</ItemsControl>
```

## パフォーマンス向上施策
- **Layout キャッシュで重複計算を削減し、ArrangeOverride での重複計算を回避**: Layout インスタンスをキャッシュすることで、MeasureOverride と ArrangeOverride 間の重複計算を防ぎ、パフォーマンスを向上させます。これにより、レイアウト変更時の再計算コストを最小限に抑え、UI の応答性を高めます。
- **ChildElements プロパティで LINQ Iterator なしの直接イテレーションを使用し、オブジェクト割り当てを削減**: ChildElements プロパティで LINQ の OfType を使用しつつ、Iterator オブジェクトの割り当てを避けることで、メモリ効率を高めます。これにより、子要素の抽出処理が高速化され、ガベージコレクションの負担を軽減します。
- **ColumnMetricsCache による重複計算回避**: LayoutStrategyBase で ColumnMetricsCache を使用して、列メトリクスの計算をキャッシュし、重複計算を回避します。これにより、アルゴリズム実行時のパフォーマンスが向上し、計算時間を短縮します。
- **ArrayPool によるメモリ効率化**: MultiColumnLayoutEngine で ArrayPool を使用して、配列のメモリ割り当てを効率化します。これにより、頻繁な配列作成によるメモリ消費を抑え、アプリケーションの全体的なパフォーマンスを向上させます。
- **Strategy パターンでアルゴリズム切り替え**: LayoutStrategyFactory で Strategy パターンを採用し、アルゴリズムの切り替えを効率的に行います。これにより、コードの再利用性が高まり、異なるアルゴリズム間の切り替えコストを最小限に抑えます。
- **遅延初期化によるメモリ効率化**: LayoutStrategyFactory で Strategy インスタンスを遅延初期化することで、不要なメモリ使用を避けます。これにより、初期化コストを分散し、アプリケーションの起動時間を短縮します。
- **ColumnMetricsCache の共有で計算コスト削減**: 全 Strategy で同じ ColumnMetricsCache を共有することで、計算結果の再利用を実現します。これにより、複数アルゴリズム実行時のパフォーマンスが向上し、全体的な計算効率を高めます。
- **StackAlloc で小規模配列最適化**: LayoutStrategyBase で STACKALLOC_THRESHOLD 以下の要素数に対して stackalloc を使用し、ヒープ割り当てを避けます。これにより、小規模データの処理が高速化され、メモリ管理の効率が向上します。
- **Template Method パターンでコード再利用**: LayoutStrategyBase で Template Method パターンを採用し、共通処理を基底クラスで定義します。これにより、コードの重複を減らし、メンテナンス性を向上させると同時に、パフォーマンスの安定性を確保します。

## ライセンス
このプロジェクトは MIT ライセンスの下で公開されています。
