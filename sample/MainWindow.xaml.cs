using Microsoft.UI.Xaml;

namespace EsUtil.WinUI3.Controls.MultiColumnPanelSample;

/// <summary>
/// Represents the main window of the application, providing the primary user interface and hosting application
/// content.
/// </summary>
/// <remarks>This class is typically instantiated by the application framework when the application
/// starts. It serves as the entry point for user interaction and navigation within the application. MainWindow is
/// sealed and cannot be inherited.</remarks>
public sealed partial class MainWindow : Window
{
    /// <summary>
    /// Initializes a new instance of the MainWindow class and navigates the content frame to the SamplePage.
    /// </summary>
    /// <remarks>
    /// This constructor sets up the main window and displays the initial page. Navigation to
    /// SamplePage occurs automatically upon window creation.
    /// </remarks>
    public MainWindow()
    {
        InitializeComponent();
        ContentFrame.Navigate(typeof(SamplePage));
    }
}
