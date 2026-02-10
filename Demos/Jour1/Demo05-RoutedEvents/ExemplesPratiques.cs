using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RoutedEventsDemo;

/// <summary>
/// Exemples pratiques d'utilisation des Routed Events
/// </summary>
public static class ExemplesPratiques
{
    #region Exemple 1 : Validation globale de saisie (Tunneling)

    /// <summary>
    /// Attache un validateur de saisie numérique à une Window
    /// Bloque tous les caractères non-numériques AVANT qu'ils n'atteignent les TextBox
    /// </summary>
    public static void AttacherValidationNumerique(Window window)
    {
        // PreviewTextInput = Tunneling, intercepte AVANT la TextBox
        window.PreviewTextInput += (sender, e) =>
        {
            // Bloquer si ce n'est pas un chiffre
            if (!char.IsDigit(e.Text[0]))
            {
                e.Handled = true; // ❌ Arrête la propagation
                MessageBox.Show($"Saisie bloquée : '{e.Text}' n'est pas un chiffre");
            }
        };
    }

    #endregion

    #region Exemple 2 : Gestion centralisée des clics (Bubbling)

    /// <summary>
    /// Écoute TOUS les clics de boutons dans un Panel parent
    /// Utile pour logger ou gérer les actions de manière centralisée
    /// </summary>
    public static void EcouterClicsBoutons(Panel panel)
    {
        // MouseDown = Bubbling, remonte de l'enfant vers le parent
        panel.MouseDown += (sender, e) =>
        {
            // e.Source = élément qui a déclenché l'événement (le Button)
            // sender = élément qui écoute (le Panel)
            if (e.Source is Button button)
            {
                MessageBox.Show($"[BUBBLING] Clic détecté sur le bouton : {button.Content}");
                // L'événement continue de remonter vers les parents
            }
        };
    }

    #endregion

    #region Exemple 3 : Prévenir le double-clic accidentel (Tunneling)

    private static DateTime _dernierClic = DateTime.MinValue;
    private static readonly TimeSpan DelaiMinimum = TimeSpan.FromMilliseconds(500);

    /// <summary>
    /// Empêche les double-clics rapides sur tous les boutons d'une Window
    /// </summary>
    public static void PrevenirDoubleClicRapide(Window window)
    {
        window.PreviewMouseDown += (sender, e) =>
        {
            var maintenant = DateTime.Now;
            var delai = maintenant - _dernierClic;

            if (delai < DelaiMinimum && e.Source is Button)
            {
                e.Handled = true; // ❌ Bloquer le clic
                MessageBox.Show($"⏱️ Clic trop rapide ! Attendez {DelaiMinimum.TotalMilliseconds}ms");
                return;
            }

            _dernierClic = maintenant;
        };
    }

    #endregion

    #region Exemple 4 : Logger tous les événements (Bubbling)

    /// <summary>
    /// Log tous les événements clavier d'une Window dans la console
    /// </summary>
    public static void LoggerEvenementsClavierGlobal(Window window, ListBox logListBox)
    {
        // KeyDown = Bubbling
        window.KeyDown += (sender, e) =>
        {
            var message = $"[KeyDown BUBBLE] Touche : {e.Key}, Source : {e.Source?.GetType().Name}";
            logListBox.Items.Insert(0, message);

            // NE PAS mettre e.Handled = true pour laisser la touche atteindre le contrôle
        };

        // PreviewKeyDown = Tunneling
        window.PreviewKeyDown += (sender, e) =>
        {
            var message = $"[PreviewKeyDown TUNNEL] Touche : {e.Key}, Source : {e.OriginalSource?.GetType().Name}";
            logListBox.Items.Insert(0, message);
        };
    }

    #endregion

    #region Exemple 5 : Créer un événement routé personnalisé

    /// <summary>
    /// Exemple de contrôle avec un événement routé personnalisé
    /// </summary>
    public class MonBoutonPersonnalise : Button
    {
        // 1. Déclarer l'événement routé (statique)
        public static readonly RoutedEvent DoubleClicRapideEvent =
            EventManager.RegisterRoutedEvent(
                "DoubleClicRapide",
                RoutingStrategy.Bubble, // Choix de la stratégie
                typeof(RoutedEventHandler),
                typeof(MonBoutonPersonnalise));

        // 2. Wrapper CLR pour l'événement
        public event RoutedEventHandler DoubleClicRapide
        {
            add => AddHandler(DoubleClicRapideEvent, value);
            remove => RemoveHandler(DoubleClicRapideEvent, value);
        }

        private DateTime _dernierClicInterne = DateTime.MinValue;

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            var maintenant = DateTime.Now;
            if (maintenant - _dernierClicInterne < TimeSpan.FromMilliseconds(300))
            {
                // 3. Déclencher l'événement routé
                RaiseEvent(new RoutedEventArgs(DoubleClicRapideEvent, this));
            }
            _dernierClicInterne = maintenant;
        }
    }

    #endregion

    #region Exemple 6 : Drag & Drop avec routage (Advanced)

    /// <summary>
    /// Permet de drag & drop n'importe quel élément dans un Panel
    /// Utilise PreviewMouseDown (Tunneling) pour intercepter tôt
    /// </summary>
    public static void ActiverDragAndDrop(Panel panel)
    {
        UIElement? elementEnCoursDeDrag = null;
        Point positionInitiale;

        // Tunneling : intercepte AVANT que l'élément enfant ne consomme l'événement
        panel.PreviewMouseDown += (sender, e) =>
        {
            if (e.Source is UIElement element && element != panel)
            {
                elementEnCoursDeDrag = element;
                positionInitiale = e.GetPosition(panel);
                element.CaptureMouse();
                e.Handled = true; // Empêche le bubbling
            }
        };

        panel.PreviewMouseMove += (sender, e) =>
        {
            if (elementEnCoursDeDrag != null)
            {
                var positionActuelle = e.GetPosition(panel);
                var deltaX = positionActuelle.X - positionInitiale.X;
                var deltaY = positionActuelle.Y - positionInitiale.Y;

                // Déplacer l'élément (si Canvas avec Canvas.Left/Top)
                if (panel is Canvas)
                {
                    Canvas.SetLeft(elementEnCoursDeDrag, Canvas.GetLeft(elementEnCoursDeDrag) + deltaX);
                    Canvas.SetTop(elementEnCoursDeDrag, Canvas.GetTop(elementEnCoursDeDrag) + deltaY);
                }

                positionInitiale = positionActuelle;
            }
        };

        panel.PreviewMouseUp += (sender, e) =>
        {
            if (elementEnCoursDeDrag != null)
            {
                elementEnCoursDeDrag.ReleaseMouseCapture();
                elementEnCoursDeDrag = null;
            }
        };
    }

    #endregion
}

/*
 * 📚 RÉSUMÉ DES CONCEPTS
 *
 * 🟣 TUNNELING (Preview*)
 * - Descend de la racine vers la cible
 * - Permet d'intercepter AVANT
 * - Utile pour : validation, bloquer des actions, logging préventif
 * - e.Handled = true arrête la descente
 *
 * 🔵 BUBBLING (événements normaux)
 * - Remonte de la cible vers la racine
 * - Mode par défaut pour la plupart des événements
 * - Utile pour : gestion centralisée, délégation d'événements
 * - e.Handled = true arrête la remontée
 *
 * 🟢 DIRECT
 * - Reste sur l'élément cible uniquement
 * - Utile pour : événements locaux (MouseEnter, Loaded)
 *
 * 🎯 PROPRIÉTÉS IMPORTANTES
 * - e.Source : Élément qui a déclenché l'événement (reste constant)
 * - sender : Élément qui écoute actuellement (change selon le routage)
 * - e.OriginalSource : Élément le plus bas dans l'arbre (avant routage)
 * - e.Handled : Arrête la propagation (mais n'annule pas l'événement)
 */
