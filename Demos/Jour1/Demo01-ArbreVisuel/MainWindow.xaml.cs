using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Demo01_ArbreVisuel.Models;

namespace Demo01_ArbreVisuel;

public partial class MainWindow : Window
{
    private Action<int, int> attribut_pour_stocker_une_methode;
    private Func<int, int> attribut_pour_une_methode;
    private MainModel model;
    public MainWindow()
    {
        InitializeComponent();
        string text = "update";
        
        SampleButton.Content = text;
        model = new MainModel();
    }

    public MainWindow(string chaine) : this()
    {
        //Instruction
        // Appeler le constructeur par défault
    }

    private void ShowLogicalTree_Click(object sender, RoutedEventArgs e)
    {
        Window1 w = new();
        w.Show();
        Close();
        LogicalTreeView.Items.Clear();
      
        var rootItem = new TreeViewItem
        {
            Header = model.GetElementDescription(SourcePanel),
            IsExpanded = true
        };

        model.BuildLogicalTree(SourcePanel, rootItem);
        LogicalTreeView.Items.Add(rootItem);
    }

    private void ShowVisualTree_Click(object sender, RoutedEventArgs e)
    {
        VisualTreeView.Items.Clear();

        var rootItem = new TreeViewItem
        {
            Header = model.GetElementDescription(SourcePanel),
            IsExpanded = true
        };

        model.BuildVisualTree(SourcePanel, rootItem);
        VisualTreeView.Items.Add(rootItem);
    }

    /// <summary>
    /// Construit récursivement l'arbre logique
    /// </summary>
    
}
