using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Demo01_ArbreVisuel.Models
{
    public class MainModel
    {
        public void BuildLogicalTree(DependencyObject parent, TreeViewItem parentItem)
        {
            foreach (var child in LogicalTreeHelper.GetChildren(parent))
            {
                if (child is DependencyObject depObj)
                {
                    var childItem = new TreeViewItem
                    {
                        Header = GetElementDescription(depObj),
                        IsExpanded = true
                    };

                    parentItem.Items.Add(childItem);
                    BuildLogicalTree(depObj, childItem);
                }
            }
        }

        /// <summary>
        /// Construit récursivement l'arbre visuel
        /// </summary>
        public void BuildVisualTree(DependencyObject parent, TreeViewItem parentItem)
        {
            int childCount = VisualTreeHelper.GetChildrenCount(parent);

            for (int i = 0; i < childCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                var childItem = new TreeViewItem()
                {
                    Header = GetElementDescription(child),
                    IsExpanded = true,
                    // Colorer différemment les éléments de template
                    Foreground = IsTemplateElement(child)
                        ? new SolidColorBrush(Colors.Blue)
                        : new SolidColorBrush(Colors.Black)
                };

                parentItem.Items.Add(childItem);
                BuildVisualTree(child, childItem);
            }
        }

        /// <summary>
        /// Génère une description lisible pour un élément
        /// </summary>
        public string GetElementDescription(object element)
        {
            var typeName = element.GetType().Name;

            if (element is FrameworkElement fe && !string.IsNullOrEmpty(fe.Name))
            {
                return $"{typeName} (x:Name=\"{fe.Name}\")";
            }

            if (element is ContentControl cc && cc.Content is string content)
            {
                var truncated = content.Length > 20 ? content.Substring(0, 20) + "..." : content;
                return $"{typeName} [\"{truncated}\"]";
            }

            if (element is TextBlock tb)
            {
                var text = tb.Text;
                var truncated = text.Length > 20 ? text.Substring(0, 20) + "..." : text;
                return $"{typeName} [\"{truncated}\"]";
            }

            return typeName;
        }

        /// <summary>
        /// Détermine si un élément fait partie d'un template (heuristique simple)
        /// </summary>
        public bool IsTemplateElement(DependencyObject element)
        {
            if (element is FrameworkElement fe)
            {
                return fe.TemplatedParent != null;
            }
            return false;
        }
    }
}
