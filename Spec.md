# MultiColumnPanel プロジェクトの詳細仕様書

## 概要
MultiColumnPanel は、WinUI3 向けの抽象パネルクラスで、ItemsSource と XAML で直接追加された子要素の両方を同時に扱う機能を提供します。ItemsControl の ItemsPanel として 100% 置き換え可能な互換性を有します。垂直方向にアイテムを複数列配置し、高さを自動計算するカスタムパネルです。

## クラス構造

### MultiColumnPanel (partial class)
- **継承**: DualSourceItemsPanel.DualSourceItemsPanel
- **名前空間**: EsUtil.WinUI3.Controls.MultiColumnPanel
- **目的**: 垂直方向にアイテムを複数列配置し、高さを自動計算するカスタムパネル

#### プロパティ
- **Layout**: MultiColumnLayoutEngine? 型の protected プロパティ。最後に計算したレイアウト結果を保持（デバッグやモニタリング用）。
- **RowSpace**: double 型の public プロパティ。行間スペース（ピクセル、デフォルト: 8.0）。
- **ColumnSpace**: double 型の public プロパティ。列間スペース（ピクセル、デフォルト: 8.0）。
- **ColumnLimit**: int 型の public プロパティ。最大列数（デフォルト: 10）。
- **Method**: VMCLMethod 型の public プロパティ。使用するアルゴリズム（デフォルト: BinarySearch）。
- **LayoutUsedWidth**: double? 型の public プロパティ。使用された幅（ピクセル、Layout.Solve() の結果から自動更新、未計算時は null）。
- **LayoutMinHeight**: double? 型の public プロパティ。最小高さ（ピクセル、Layout.Solve() の結果から自動更新、未計算時は null）。
- **LayoutColumnCount**: int? 型の public プロパティ。計算されたレイアウトの列数（Layout.GetLastColumnSegments() の長さから自動更新、未計算時は null）。
- **LayoutColumnSegments**: string? 型の public プロパティ。計算されたレイアウトの列セグメント情報（Layout.GetLastColumnSegments() の結果から自動更新、未計算時は null）。

#### 依存関係プロパティ
- **RowSpaceProperty**: RowSpace の依存関係プロパティ。
- **ColumnSpaceProperty**: ColumnSpace の依存関係プロパティ。
- **ColumnLimitProperty**: ColumnLimit の依存関係プロパティ。
- **MethodProperty**: Method の依存関係プロパティ。
- **LayoutUsedWidthProperty**: LayoutUsedWidth の依存関係プロパティ。
- **LayoutMinHeightProperty**: LayoutMinHeight の依存関係プロパティ。
- **LayoutColumnCountProperty**: LayoutColumnCount の依存関係プロパティ。
- **LayoutColumnSegmentsProperty**: LayoutColumnSegments の依存関係プロパティ。

#### メソッド
- **コンストラクター**: MultiColumnPanel()。なし。
- **OnLayoutPropertyChanged (private static)**: DependencyProperty 変更時のコールバック。レイアウト再計算が必要な場合に InvalidateMeasure() を呼び出す。
- **UpdateLayoutResultProperties (private)**: Layout の結果を DependencyProperty に反映。
- **OnDependencyPropertyChangedInternal (protected override)**: DependencyProperty が変更されたときに呼び出される。
- **OnItemsSourceChangedInternal (protected override)**: ItemsSource が変更されたときに呼び出される。
- **OnItemTemplateChangedInternal (protected override)**: ItemTemplate が変更されたときに呼び出される。
- **OnItemsSourceFirstChangedInternal (protected override)**: ItemsSourceFirst が変更されたときに呼び出される。
- **MeasureOverride (protected override)**: 測定をオーバーライド。
- **ArrangeOverride (protected override)**: 配置をオーバーライド。
- **ChildElements (private)**: 子要素を抽出（LINQ Iterator なし）。

### LayoutStrategyBase (abstract class)
- **名前空間**: EsUtil.Algorithm.MultiColumnLayoutEngine
- **目的**: マルチカラムレイアウト最適化アルゴリズムの基底クラス

#### フィールド
- **STACKALLOC_THRESHOLD**: int 型の internal const フィールド。スタックアロケーションの閾値（この要素以下は stackalloc を使用）。
- **_items**: Size[] 型の internal readonly フィールド。各アイテムの (幅, 高さ) タプル配列。
- **_space**: Space 型の internal readonly フィールド。行間スペースと列間スペース。
- **_columnLimit**: int 型の internal readonly フィールド。使用可能な列数の上限。
- **_itemsMaxWidthCached**: double 型の internal フィールド。最大アイテム幅のキャッシュ。
- **_lastSolveResult**: StrategyResult 型の internal フィールド。最後の Solve() 実行時の 最大列高さ と 最小幅 と 実行時の各列のアイテム範囲。

#### プロパティ
- **_metricsCache**: ColumnMetricsCache 型の internal readonly フィールド。列メトリクスのキャッシュ（複数アルゴリズム実行間で共用）。

#### メソッド
- **コンストラクター**: LayoutStrategyBase(IReadOnlyList<(double Width, double Height)> items, (double Row, double Column) space, int columnLimit = 10)。パラメータを受け取ってフィールド変数に格納。
- **Solve (public)**: 最適なマルチカラムレイアウトを計算。
- **SolveCore (internal abstract)**: 具体的なレイアウト計算ロジックを実装。
- **IsValidWidth (public)**: 最大アイテム幅が widthLimit を超えるかどうかを判定。
- **VerifyLayoutResult (public)**: レイアウト計算結果の正当性を検証。
- **SolveSingleColumnLayout (public)**: 単列レイアウト結果を計算して返却。
- **GetLastResult (public)**: 最後の Solve() 実行時の結果を取得。
- **GetLastColumnSegments (public)**: 最後の Solve() 実行時の各列のアイテム範囲セグメントを取得。
- **GetLastItemLayouts (public)**: レイアウト計算結果に基づいて、各アイテムの描画座標とサイズを計算。
- **ClearCache (public)**: キャッシュをクリア。
- **GetColumnMetrics (internal)**: 列メトリクスを取得（キャッシュ付き遅延計算）。

### MultiColumnLayoutEngine (sealed class)
- **継承**: LayoutStrategyBase
- **名前空間**: EsUtil.Algorithm.MultiColumnLayoutEngine
- **目的**: 垂直マルチカラムレイアウト最適化クラス

#### フィールド
- **_strategyFactory**: LayoutStrategyFactory? 型の internal フィールド。Strategy Factory（遅延初期化）。

#### プロパティ
- **StrategyFactory**: LayoutStrategyFactory 型の internal プロパティ。Strategy Factory を遅延初期化して返却。
- **BinarySearchOptions**: BinarySearchOptions 型の public プロパティ。BinarySearch 用のオプションを取得または設定。
- **CurrentMethod**: Method 型の public プロパティ。使用するレイアウト計算アルゴリズムを取得または設定。

#### メソッド
- **コンストラクター**: MultiColumnLayoutEngine(IReadOnlyList<(double Width, double Height)> items, (double Row, double Column) space, int columnLimit = 10)。パラメータを受け取ってフィールド変数に格納。
- **SolveCore (internal override)**: 抽象メソッド：具体的なレイアウト計算ロジックを実装。
- **GetCurrentStrategyName (public)**: 現在アクティブな戦略の名前を取得。
- **GetBinarySearchIterationCount (public)**: 最後の BinarySearch Solve() 実行時の反復回数を取得。

### LayoutStrategyFactory (internal sealed class)
- **名前空間**: EsUtil.Algorithm.MultiColumnLayoutEngine
- **目的**: Layout Strategy インスタンスを生成する Factory クラス

#### フィールド
- **_dpStrategy**: DPLayoutStrategy? 型の internal フィールド。
- **_greedyStrategy**: GreedyLayoutStrategy? 型の internal フィールド。
- **_binarySearchStrategy**: BinarySearchLayoutStrategy? 型の internal フィールド。
- **_bSearchOptions**: BinarySearchOptions? 型の internal フィールド。

#### プロパティ
- **CurrentMethod**: Method 型の public プロパティ。使用するレイアウト計算アルゴリズムを取得または設定。
- **BinarySearchOptions**: BinarySearchOptions 型の public プロパティ。BinarySearch 用のオプションを取得または設定（遅延初期化）。

#### メソッド
- **コンストラクター**: LayoutStrategyFactory(Size[] items, Space space, int columnLimit, ColumnMetricsCache metricsCache)。パラメータを受け取ってフィールド変数に格納。
- **GetStrategy (public)**: 指定のメソッドに対応した Strategy インスタンスを取得。
- **GetOrCreateDPStrategy (private)**: DP Strategy インスタンスを取得または作成（遅延初期化）。
- **GetOrCreateGreedyStrategy (private)**: Greedy Strategy インスタンスを取得または作成（遅延初期化）。
- **GetOrCreateBinarySearchStrategy (private)**: BinarySearch Strategy インスタンスを取得または作成（遅延初期化）。
- **ClearStrategyCache (public)**: Factory の遅延初期化キャッシュをクリア。
- **ClearMetricsCache (public)**: メトリクスキャッシュを直接クリア。

### StrategyResult (internal readonly record struct)
- **名前空間**: EsUtil.Algorithm.MultiColumnLayoutEngine
- **目的**: レイアウト計算の結果を格納するレコード構造体

#### フィールド
- **UsedWidth**: double 型のフィールド。
- **MinHeight**: double 型のフィールド。
- **ColumnSegments**: ColumnSegment[] 型のフィールド。

#### プロパティ
- **Empty**: StrategyResult 型の static readonly フィールド。空のレイアウト結果。

### Size (internal readonly record struct)
- **名前空間**: EsUtil.Algorithm.MultiColumnLayoutEngine
- **目的**: 幅と高さを表すサイズ情報を格納するレコード構造体

#### メソッド
- **ToSizeArray (public static, AggressiveInlining)**: IReadOnlyList から 配列に変換。
- **ToSizeArray (public static, AggressiveInlining)**: ReadOnlySpan から 配列に変換。

### BinarySearchOptions (public readonly record struct)
- **名前空間**: EsUtil.Algorithm.MultiColumnLayoutEngine
- **目的**: バイナリサーチアルゴリズムの動作制御オプション

#### フィールド
- **Epsilon**: double 型のフィールド。二分探索の収束判定の許容誤差（デフォルト: 1e-3）。
- **MaxIterations**: int 型のフィールド。二分探索の最大反復回数（デフォルト: 100）。
- **LowerBoundRatio**: double 型のフィールド。二分探索の下限比率（デフォルト: 0.95）。

#### プロパティ
- **Default**: BinarySearchOptions 型の static readonly フィールド。デフォルトのオプション値。

#### メソッド
- **コンストラクター**: BinarySearchOptions(double Epsilon = 1e-3, int MaxIterations = 100, double LowerBoundRatio = 0.95)。コンストラクタ（デフォルト値）。
- **コンストラクター**: BinarySearchOptions()。コンストラクタ（デフォルト値）。
- **IsValid (public)**: オプション設定の妥当性を検証。

### Method (public enum)
- **名前空間**: EsUtil.Algorithm.MultiColumnLayoutEngine
- **目的**: 使用するレイアウトアルゴリズムを指定する列挙型

#### 値
- **DynamicProgramming**: 動的計画法：最適解を保証（計算量 O(n² × m)）。
- **Greedy**: Greedy 近似法：高速計算（計算量 O(n × m)、品質 90%+）。
- **BinarySearch**: バイナリサーチ法：バランス型（計算量 O(n × log(h))、品質 99%+）。

## 設計原則
- DualSourceItemsPanel から継承（ItemsControl の ItemsPanel として使用可能）。
- MultiColumnLayout.Solve() の結果を直接反映。
- 行間・列間スペースを制御可能な DependencyProperty。
- 3 種類のアルゴリズム（DynamicProgramming/BinarySearch/Greedy）をサポート。
- ベースクラスから継承したデバッグプロパティで実行状態をモニタリング可能。

## パフォーマンス向上施策
- **Layout キャッシュで重複計算を削減し、ArrangeOverride での重複計算を回避**: Layout インスタンスをキャッシュすることで、MeasureOverride と ArrangeOverride 間の重複計算を防ぎ、パフォーマンスを向上させます。これにより、レイアウト変更時の再計算コストを最小限に抑えます。
- **ChildElements プロパティで LINQ Iterator なしの直接イテレーションを使用し、オブジェクト割り当てを削減**: ChildElements プロパティで LINQ の OfType を使用しつつ、Iterator オブジェクトの割り当てを避けることで、メモリ効率を高めます。これにより、子要素の抽出処理が高速化されます。
- **ColumnMetricsCache による重複計算回避**: LayoutStrategyBase で ColumnMetricsCache を使用して、列メトリクスの計算をキャッシュし、重複計算を回避します。これにより、アルゴリズム実行時のパフォーマンスが向上します。
- **ArrayPool によるメモリ効率化**: MultiColumnLayoutEngine で ArrayPool を使用して、配列のメモリ割り当てを効率化します。これにより、ガベージコレクションの負担を軽減します。
- **Strategy パターンでアルゴリズム切り替え**: LayoutStrategyFactory で Strategy パターンを採用し、アルゴリズムの切り替えを効率的に行います。これにより、コードの再利用性とパフォーマンスを向上させます。
- **遅延初期化によるメモリ効率化**: LayoutStrategyFactory で Strategy インスタンスを遅延初期化することで、不要なメモリ使用を避けます。これにより、初期化コストを分散します。
- **ColumnMetricsCache の共有で計算コスト削減**: 全 Strategy で同じ ColumnMetricsCache を共有することで、計算結果の再利用を実現します。これにより、複数アルゴリズム実行時のパフォーマンスが向上します。
- **StackAlloc で小規模配列最適化**: LayoutStrategyBase で STACKALLOC_THRESHOLD 以下の要素数に対して stackalloc を使用し、ヒープ割り当てを避けます。これにより、小規模データの処理が高速化されます。
- **Template Method パターンでコード再利用**: LayoutStrategyBase で Template Method パターンを採用し、共通処理を基底クラスで定義します。これにより、コードの重複を減らし、メンテナンス性を向上させます。
