using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using AopDemo.Attributes;

namespace AopDemo.Infrastructure;

/// <summary>
/// Intercepteur AOP transparent.
/// S'attache à n'importe quel INotifyPropertyChanged et applique
/// les aspects déclarés par attributs — SANS que l'objet le sache.
///
/// Equivalent .NET de ce que fait Spring AOP avec ses proxies :
/// le conteneur DI appelle Attach() automatiquement à la résolution.
/// Le ViewModel n'a aucun code AOP, aucune base class, aucun câblage.
/// </summary>
public class AopInterceptor
{
    // Cache statique : type → métadonnées AOP (scanné une seule fois)
    private static readonly Dictionary<Type, Dictionary<string, PropertyAopInfo>> _typeCache = new();

    // État par instance
    private readonly object _target;
    private readonly Dictionary<string, PropertyAopInfo> _properties;
    private readonly Dictionary<string, object?> _originalValues = new();
    private bool _isTracking;
    private bool _isDirty;

    /// <summary>Log AOP observable — bindable directement dans l'UI</summary>
    public ObservableCollection<string> Log { get; } = new();

    /// <summary>IsDirty exposé pour le binding</summary>
    public bool IsDirty
    {
        get => _isDirty;
        private set
        {
            if (_isDirty == value) return;
            _isDirty = value;
            DirtyChanged?.Invoke(value);
        }
    }

    public event Action<bool>? DirtyChanged;

    private AopInterceptor(object target, Dictionary<string, PropertyAopInfo> properties)
    {
        _target = target;
        _properties = properties;
    }

    /// <summary>
    /// Point d'entrée : attache l'intercepteur à un objet.
    /// Appelé automatiquement par le conteneur DI via AddWithAop().
    /// Le ViewModel ne voit RIEN.
    /// </summary>
    public static AopInterceptor Attach(INotifyPropertyChanged target)
    {
        var type = target.GetType();
        var properties = GetOrScanProperties(type);
        var interceptor = new AopInterceptor(target, properties);

        // Branchement transparent sur l'event PropertyChanged
        target.PropertyChanged += interceptor.OnPropertyChanged;

        return interceptor;
    }

    /// <summary>
    /// Démarre le dirty tracking (appelé après un chargement)
    /// </summary>
    public void StartTracking()
    {
        _originalValues.Clear();
        var type = _target.GetType();

        foreach (var (propName, info) in _properties)
        {
            if (info.HasAutoDirty)
                _originalValues[propName] = type.GetProperty(propName)?.GetValue(_target);
        }

        _isTracking = true;
        IsDirty = false;
        AddLog("AutoDirty", "Tracking démarré");
    }

    /// <summary>
    /// Interception de chaque changement de propriété.
    /// C'est ici que les aspects s'exécutent.
    /// </summary>
    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is null || sender is null) return;
        if (!_properties.TryGetValue(e.PropertyName, out var info)) return;

        var type = sender.GetType();
        var prop = type.GetProperty(e.PropertyName);
        if (prop is null) return;

        var currentValue = prop.GetValue(sender);

        // ── Aspect [Clamp] ──
        if (info.Clamp is { } clamp)
        {
            var clamped = ClampValue(currentValue, clamp.Minimum, clamp.Maximum);
            if (!Equals(clamped, currentValue))
            {
                AddLog("Clamp", $"{e.PropertyName} : {currentValue} → {clamped} [{clamp.Minimum}..{clamp.Maximum}]");
                prop.SetValue(sender, clamped);
                return; // Re-déclenche PropertyChanged avec la valeur corrigée
            }
        }

        // ── Aspect [LogOnChange] ──
        if (info.HasLog)
        {
            AddLog("Log", $"{e.PropertyName} = {FormatValue(currentValue)}");
        }

        // ── Aspect [AutoDirty] ──
        if (info.HasAutoDirty && _isTracking)
        {
            if (_originalValues.TryGetValue(e.PropertyName, out var original))
            {
                if (!Equals(original, currentValue) && !IsDirty)
                {
                    IsDirty = true;
                    AddLog("AutoDirty", $"'{e.PropertyName}' modifié — dirty");
                }
                else if (Equals(original, currentValue))
                {
                    var stillDirty = _originalValues.Any(kvp =>
                        !Equals(type.GetProperty(kvp.Key)?.GetValue(sender), kvp.Value));

                    if (!stillDirty && IsDirty)
                    {
                        IsDirty = false;
                        AddLog("AutoDirty", "Valeurs restaurées — clean");
                    }
                }
            }
        }

        // ── Aspect [DependsOn] ──
        foreach (var dependent in info.DependentProperties)
        {
            AddLog("DependsOn", $"{e.PropertyName} → notifie {dependent}");
            // Déclencher PropertyChanged pour la propriété dépendante
            if (sender is CommunityToolkit.Mvvm.ComponentModel.ObservableObject obs)
            {
                // Utiliser réflexion pour appeler OnPropertyChanged protégé
                var method = typeof(CommunityToolkit.Mvvm.ComponentModel.ObservableObject)
                    .GetMethod("OnPropertyChanged", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
                        null, [typeof(string)], null);
                method?.Invoke(sender, [dependent]);
            }
        }
    }

    #region Scanning des attributs

    /// <summary>
    /// Scanne un type pour trouver tous les attributs AOP.
    /// Cherche sur les propriétés ET sur les fields (pour [ObservableProperty] du Toolkit).
    /// Résultat mis en cache — exécuté une seule fois par type.
    /// </summary>
    private static Dictionary<string, PropertyAopInfo> GetOrScanProperties(Type type)
    {
        if (_typeCache.TryGetValue(type, out var cached))
            return cached;

        var result = new Dictionary<string, PropertyAopInfo>();

        // Scanner les propriétés publiques
        foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            var info = ScanMember(prop);
            if (info is not null)
                result[prop.Name] = info;
        }

        // Scanner les fields privés (pour les [ObservableProperty] du Toolkit)
        // Le Toolkit génère la propriété PascalCase depuis le field _camelCase
        foreach (var field in type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
        {
            var info = ScanMember(field);
            if (info is null) continue;

            // Convertir _fieldName → PropertyName
            var propName = FieldToPropertyName(field.Name);
            if (propName is not null && !result.ContainsKey(propName))
                result[propName] = info;
        }

        _typeCache[type] = result;
        return result;
    }

    private static PropertyAopInfo? ScanMember(MemberInfo member)
    {
        var log = member.GetCustomAttribute<LogOnChangeAttribute>();
        var clamp = member.GetCustomAttribute<ClampAttribute>();
        var dirty = member.GetCustomAttribute<AutoDirtyAttribute>();
        var depends = member.GetCustomAttributes<DependsOnAttribute>()
            .SelectMany(a => a.PropertyNames).ToList();

        if (log is null && clamp is null && dirty is null && depends.Count == 0)
            return null;

        return new PropertyAopInfo
        {
            HasLog = log is not null,
            Clamp = clamp,
            HasAutoDirty = dirty is not null,
            DependentProperties = depends
        };
    }

    /// <summary>
    /// Convertit un nom de field Toolkit en nom de propriété :
    /// _firstName → FirstName, _price → Price
    /// </summary>
    private static string? FieldToPropertyName(string fieldName)
    {
        if (!fieldName.StartsWith('_') || fieldName.Length < 2)
            return null;

        return char.ToUpper(fieldName[1]) + fieldName[2..];
    }

    #endregion

    #region Helpers

    private void AddLog(string aspect, string message)
    {
        var timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
        Log.Add($"[{timestamp}] [{aspect}] {message}");
    }

    private static object? ClampValue(object? value, double min, double max) => value switch
    {
        int i => Math.Clamp(i, (int)min, (int)max),
        double d => Math.Clamp(d, min, max),
        float f => Math.Clamp(f, (float)min, (float)max),
        _ => value
    };

    private static string FormatValue(object? value) => value switch
    {
        null => "(null)",
        string s when s.Length == 0 => "(vide)",
        string s => $"\"{s}\"",
        _ => value.ToString() ?? "(null)"
    };

    #endregion
}

internal class PropertyAopInfo
{
    public bool HasLog { get; init; }
    public ClampAttribute? Clamp { get; init; }
    public bool HasAutoDirty { get; init; }
    public List<string> DependentProperties { get; init; } = [];
}
