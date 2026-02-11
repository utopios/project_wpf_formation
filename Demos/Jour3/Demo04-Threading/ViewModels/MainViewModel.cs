using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using ThreadingDemo.Commands;

namespace ThreadingDemo.ViewModels;

/// <summary>
/// ViewModel principal démontrant l'évolution du threading en .NET :
/// Thread → ThreadPool → Synchronisation → Task (TPL)
/// </summary>
public class MainViewModel : ViewModelBase
{
    private readonly Dispatcher _dispatcher;
    private readonly object _lockObject = new();
    private int _unsafeCounter;
    private CancellationTokenSource? _cts;

    public MainViewModel()
    {
        _dispatcher = Application.Current.Dispatcher;

        // Tab 1 : Thread
        StartThreadCommand = new RelayCommand(_ => StartThread(), _ => !IsThreadRunning);
        FreezeUiCommand = new RelayCommand(_ => FreezeUi(), _ => !IsThreadRunning);

        // Tab 2 : ThreadPool
        StartPoolCommand = new RelayCommand(_ => StartPool(), _ => !IsPoolRunning);

        // Tab 3 : Synchronisation
        StartWithoutSyncCommand = new RelayCommand(_ => StartWithoutSync(), _ => !IsSyncRunning);
        StartWithSyncCommand = new RelayCommand(_ => StartWithSync(), _ => !IsSyncRunning);

        // Tab 4 : Task (TPL)
        StartTaskCommand = new RelayCommand(_ => StartTaskAsync(), _ => !IsTaskRunning);
        StartParallelCommand = new RelayCommand(_ => StartParallelAsync(), _ => !IsTaskRunning);
        CancelTaskCommand = new RelayCommand(_ => CancelTask(), _ => IsTaskRunning);
    }

    // ================================================================
    // TAB 1 : Thread
    // ================================================================

    #region Thread

    private string _threadLog = string.Empty;
    public string ThreadLog
    {
        get => _threadLog;
        set => SetProperty(ref _threadLog, value);
    }

    private double _threadProgress;
    public double ThreadProgress
    {
        get => _threadProgress;
        set => SetProperty(ref _threadProgress, value);
    }

    private bool _isThreadRunning;
    public bool IsThreadRunning
    {
        get => _isThreadRunning;
        set => SetProperty(ref _isThreadRunning, value);
    }

    public ICommand StartThreadCommand { get; }
    public ICommand FreezeUiCommand { get; }

    /// <summary>
    /// Démarre un thread d'arrière-plan avec mise à jour du UI via Dispatcher
    /// </summary>
    private void StartThread()
    {
        ThreadLog = string.Empty;
        ThreadProgress = 0;
        IsThreadRunning = true;

        ThreadLog += $"[UI Thread #{Environment.CurrentManagedThreadId}] Démarrage du thread de travail...\n";
        ThreadLog += "L'interface reste réactive pendant le traitement.\n\n";

        var thread = new Thread(() =>
        {
            var threadId = Environment.CurrentManagedThreadId;

            for (int i = 1; i <= 100; i++)
            {
                Thread.Sleep(30);
                int progress = i;
                _dispatcher.Invoke(() =>
                {
                    ThreadProgress = progress;
                    if (progress % 10 == 0)
                        ThreadLog += $"  [Thread #{threadId}] Progression : {progress}%\n";
                });
            }

            _dispatcher.Invoke(() =>
            {
                ThreadLog += $"\n[Thread #{threadId}] Travail terminé !\n";
                ThreadLog += $"[UI Thread #{Environment.CurrentManagedThreadId}] Interface mise à jour.\n";
                IsThreadRunning = false;
            });
        });

        thread.IsBackground = true;
        thread.Name = "WorkerThread";
        thread.Start();
    }

    /// <summary>
    /// Exécute un travail lourd sur le thread UI pour montrer le gel de l'interface
    /// </summary>
    private void FreezeUi()
    {
        ThreadLog = "[UI Thread] Début du travail sur le thread principal...\n";
        ThreadLog += "L'interface va se figer pendant 3 secondes !\n\n";
        ThreadProgress = 0;

        // Force le rendu avant le blocage pour que le message soit visible
        _dispatcher.Invoke(() => { }, DispatcherPriority.Render);

        // Bloque le thread UI pendant 3 secondes
        Thread.Sleep(3000);

        ThreadProgress = 100;
        ThreadLog += "[UI Thread] Travail terminé.\n\n";
        ThreadLog += "PROBLÈME : L'interface était complètement figée !\n";
        ThreadLog += "=> Il faut utiliser des threads secondaires pour les\n";
        ThreadLog += "   traitements longs afin de garder le UI réactif.\n";
    }

    #endregion

    // ================================================================
    // TAB 2 : ThreadPool
    // ================================================================

    #region ThreadPool

    private string _poolLog = string.Empty;
    public string PoolLog
    {
        get => _poolLog;
        set => SetProperty(ref _poolLog, value);
    }

    private bool _isPoolRunning;
    public bool IsPoolRunning
    {
        get => _isPoolRunning;
        set => SetProperty(ref _isPoolRunning, value);
    }

    public ICommand StartPoolCommand { get; }

    /// <summary>
    /// Lance 5 tâches via ThreadPool.QueueUserWorkItem
    /// </summary>
    private void StartPool()
    {
        PoolLog = string.Empty;
        IsPoolRunning = true;
        int completedCount = 0;
        const int totalTasks = 5;

        ThreadPool.GetMinThreads(out int workerMin, out _);
        ThreadPool.GetMaxThreads(out int workerMax, out _);

        PoolLog += "=== ThreadPool ===\n\n";
        PoolLog += $"Configuration : min={workerMin}, max={workerMax} threads\n";
        PoolLog += "Le pool réutilise les threads automatiquement.\n\n";

        for (int i = 1; i <= totalTasks; i++)
        {
            int taskNum = i;
            ThreadPool.QueueUserWorkItem(_ =>
            {
                var threadId = Environment.CurrentManagedThreadId;
                var duration = Random.Shared.Next(500, 2500);

                _dispatcher.Invoke(() =>
                    PoolLog += $"[Tâche {taskNum}] Démarrée sur Thread #{threadId} (durée : {duration}ms)\n");

                Thread.Sleep(duration);

                _dispatcher.Invoke(() =>
                {
                    PoolLog += $"[Tâche {taskNum}] Terminée (Thread #{threadId})\n";

                    if (Interlocked.Increment(ref completedCount) == totalTasks)
                    {
                        PoolLog += $"\nToutes les {totalTasks} tâches sont terminées !\n\n";
                        PoolLog += "Avantages du ThreadPool vs Thread :\n";
                        PoolLog += "  - Pas de coût de création/destruction de threads\n";
                        PoolLog += "  - Gestion automatique du nombre de threads\n";
                        PoolLog += "  - Réutilisation des threads existants\n";
                        IsPoolRunning = false;
                    }
                });
            });
        }
    }

    #endregion

    // ================================================================
    // TAB 3 : Synchronisation
    // ================================================================

    #region Synchronisation

    private string _syncLog = string.Empty;
    public string SyncLog
    {
        get => _syncLog;
        set => SetProperty(ref _syncLog, value);
    }

    private int _sharedCounter;
    public int SharedCounter
    {
        get => _sharedCounter;
        set => SetProperty(ref _sharedCounter, value);
    }

    private bool _isSyncRunning;
    public bool IsSyncRunning
    {
        get => _isSyncRunning;
        set => SetProperty(ref _isSyncRunning, value);
    }

    public ICommand StartWithoutSyncCommand { get; }
    public ICommand StartWithSyncCommand { get; }

    /// <summary>
    /// Démontre une race condition : plusieurs threads modifient un compteur sans synchronisation
    /// </summary>
    private void StartWithoutSync()
    {
        const int threadsCount = 5;
        const int incrementsPerThread = 100_000;
        int expected = threadsCount * incrementsPerThread;

        SyncLog = "=== SANS synchronisation (race condition) ===\n\n";
        SyncLog += $"{threadsCount} threads x {incrementsPerThread:N0} incréments = {expected:N0} attendu\n\n";
        _unsafeCounter = 0;
        SharedCounter = 0;
        IsSyncRunning = true;

        int completedThreads = 0;

        for (int i = 0; i < threadsCount; i++)
        {
            int threadNum = i + 1;
            var thread = new Thread(() =>
            {
                var threadId = Environment.CurrentManagedThreadId;

                for (int j = 0; j < incrementsPerThread; j++)
                {
                    _unsafeCounter++; // Race condition !
                }

                _dispatcher.Invoke(() =>
                    SyncLog += $"  Thread #{threadId} (n°{threadNum}) a terminé ses incréments\n");

                if (Interlocked.Increment(ref completedThreads) == threadsCount)
                {
                    _dispatcher.Invoke(() =>
                    {
                        SharedCounter = _unsafeCounter;
                        SyncLog += $"\nRésultat obtenu  : {SharedCounter:N0}\n";
                        SyncLog += $"Résultat attendu : {expected:N0}\n";
                        SyncLog += $"Incréments perdus : {expected - SharedCounter:N0}\n\n";
                        SyncLog += "ERREUR : Des incréments ont été perdus !\n";
                        SyncLog += "Le ++ n'est pas atomique : read-modify-write\n";
                        SyncLog += "peut être interrompu par un autre thread.\n";
                        IsSyncRunning = false;
                    });
                }
            });
            thread.IsBackground = true;
            thread.Start();
        }
    }

    /// <summary>
    /// Même opération mais avec un lock pour synchroniser l'accès au compteur
    /// </summary>
    private void StartWithSync()
    {
        const int threadsCount = 5;
        const int incrementsPerThread = 100_000;
        int expected = threadsCount * incrementsPerThread;

        SyncLog = "=== AVEC synchronisation (lock) ===\n\n";
        SyncLog += $"{threadsCount} threads x {incrementsPerThread:N0} incréments = {expected:N0} attendu\n\n";
        _unsafeCounter = 0;
        SharedCounter = 0;
        IsSyncRunning = true;

        var sw = Stopwatch.StartNew();
        int completedThreads = 0;

        for (int i = 0; i < threadsCount; i++)
        {
            int threadNum = i + 1;
            var thread = new Thread(() =>
            {
                var threadId = Environment.CurrentManagedThreadId;

                for (int j = 0; j < incrementsPerThread; j++)
                {
                    lock (_lockObject)
                    {
                        _unsafeCounter++;
                    }
                }

                _dispatcher.Invoke(() =>
                    SyncLog += $"  Thread #{threadId} (n°{threadNum}) a terminé ses incréments\n");

                if (Interlocked.Increment(ref completedThreads) == threadsCount)
                {
                    sw.Stop();
                    _dispatcher.Invoke(() =>
                    {
                        SharedCounter = _unsafeCounter;
                        SyncLog += $"\nRésultat obtenu  : {SharedCounter:N0}\n";
                        SyncLog += $"Résultat attendu : {expected:N0}\n";
                        SyncLog += $"Durée : {sw.ElapsedMilliseconds}ms\n\n";
                        SyncLog += "SUCCÈS : Le lock garantit l'accès exclusif\n";
                        SyncLog += "au compteur partagé entre les threads.\n\n";
                        SyncLog += "Alternatives au lock :\n";
                        SyncLog += "  - Monitor.Enter/Exit (lock est du sucre syntaxique)\n";
                        SyncLog += "  - Mutex (inter-processus)\n";
                        SyncLog += "  - SemaphoreSlim (limiter l'accès concurrent)\n";
                        SyncLog += "  - Interlocked (opérations atomiques simples)\n";
                        IsSyncRunning = false;
                    });
                }
            });
            thread.IsBackground = true;
            thread.Start();
        }
    }

    #endregion

    // ================================================================
    // TAB 4 : Task (TPL - Task Parallel Library)
    // ================================================================

    #region Task

    private string _taskLog = string.Empty;
    public string TaskLog
    {
        get => _taskLog;
        set => SetProperty(ref _taskLog, value);
    }

    private double _taskProgress;
    public double TaskProgress
    {
        get => _taskProgress;
        set => SetProperty(ref _taskProgress, value);
    }

    private bool _isTaskRunning;
    public bool IsTaskRunning
    {
        get => _isTaskRunning;
        set => SetProperty(ref _isTaskRunning, value);
    }

    public ICommand StartTaskCommand { get; }
    public ICommand StartParallelCommand { get; }
    public ICommand CancelTaskCommand { get; }

    /// <summary>
    /// Démontre Task.Run avec async/await, IProgress et CancellationToken
    /// </summary>
    private async void StartTaskAsync()
    {
        TaskLog = string.Empty;
        TaskProgress = 0;
        IsTaskRunning = true;
        _cts = new CancellationTokenSource();

        TaskLog += "=== Task.Run + async/await ===\n\n";
        TaskLog += $"[UI Thread #{Environment.CurrentManagedThreadId}] Lancement de la tâche...\n";
        TaskLog += "Annulation possible avec CancellationToken.\n";
        TaskLog += "Progression via IProgress<T>.\n\n";

        var progress = new Progress<(int percent, string message)>(data =>
        {
            TaskProgress = data.percent;
            TaskLog += data.message;
        });

        try
        {
            var sw = Stopwatch.StartNew();
            await Task.Run(() => DoWork(progress, _cts.Token), _cts.Token);
            sw.Stop();

            TaskLog += $"\nTâche terminée avec succès en {sw.ElapsedMilliseconds}ms !\n\n";
            TaskLog += "Avantages de Task vs Thread :\n";
            TaskLog += "  - async/await : code lisible et séquentiel\n";
            TaskLog += "  - CancellationToken : annulation coopérative\n";
            TaskLog += "  - IProgress<T> : progression thread-safe\n";
            TaskLog += "  - Utilise le ThreadPool en interne\n";
        }
        catch (OperationCanceledException)
        {
            TaskLog += "\nTâche annulée par l'utilisateur.\n";
        }
        finally
        {
            IsTaskRunning = false;
            _cts.Dispose();
            _cts = null;
        }
    }

    private static void DoWork(IProgress<(int percent, string message)> progress, CancellationToken token)
    {
        var threadId = Environment.CurrentManagedThreadId;

        for (int i = 1; i <= 100; i++)
        {
            if(!token.IsCancellationRequested)
            {
                Thread.Sleep(30);

                if (i % 10 == 0)
                    progress.Report((i, $"  [Worker Thread #{threadId}] Progression : {i}%\n"));
            }
           
        }
    }

    /// <summary>
    /// Démontre Task.WhenAll pour l'exécution parallèle de plusieurs tâches
    /// </summary>
    private async void StartParallelAsync()
    {
        TaskLog = string.Empty;
        TaskProgress = 0;
        IsTaskRunning = true;
        _cts = new CancellationTokenSource();

        TaskLog += "=== Task.WhenAll (exécution parallèle) ===\n\n";
        TaskLog += $"[UI Thread #{Environment.CurrentManagedThreadId}] Lancement de 5 tâches en parallèle...\n\n";

        try
        {
            var sw = Stopwatch.StartNew();

            var tasks = Enumerable.Range(1, 5).Select(i => Task.Run(async () =>
            {
                var threadId = Environment.CurrentManagedThreadId;
                var duration = Random.Shared.Next(500, 2500);

                _dispatcher.Invoke(() =>
                    TaskLog += $"  [Tâche {i}] Démarrée sur Thread #{threadId} (durée : {duration}ms)\n");

                await Task.Delay(duration, _cts!.Token);

                _dispatcher.Invoke(() =>
                    TaskLog += $"  [Tâche {i}] Terminée (Thread #{Environment.CurrentManagedThreadId})\n");
            }, _cts.Token)).ToArray();

            await Task.WhenAll(tasks);

            sw.Stop();
            TaskProgress = 100;
            TaskLog += $"\nToutes les tâches terminées en {sw.ElapsedMilliseconds}ms !\n";
            TaskLog += "(Temps total < somme des durées individuelles)\n\n";
            TaskLog += "Task.WhenAll vs Task.WhenAny :\n";
            TaskLog += "  - WhenAll : attend que TOUTES les tâches finissent\n";
            TaskLog += "  - WhenAny : attend que la PREMIÈRE tâche finisse\n";
        }
        catch (OperationCanceledException)
        {
            TaskLog += "\nTâches annulées par l'utilisateur.\n";
        }
        finally
        {
            IsTaskRunning = false;
            _cts?.Dispose();
            _cts = null;
        }
    }

    private void CancelTask()
    {
        TaskLog += "\n--- Demande d'annulation envoyée ---\n\n";
        _cts?.Cancel();
    }

    #endregion
}
