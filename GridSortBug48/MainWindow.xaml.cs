using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GridSortBug
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ICommand _startCommand;
        private ICommand _stopCommand;
        private readonly List<ItemDto> _dtos;
        private CancellationTokenSource _cancellationTokenSource;

        private readonly List<int> _priorities = new List<int> {1, 2, 3, 4, 5};
        private readonly List<int> _states = new List<int> {1, 2, 3};
        private BlockingCollection<ItemDto> _blockingCollection;

        public MainWindow()
        {
            InitializeComponent();

            Items1 = new ObservableCollection<Item>();
            CollectionView1 = CollectionViewFactory.Create(Items1, true);
            Items2 = new ObservableCollection<Item>();
            CollectionView2 = CollectionViewFactory.Create(Items2, false);

            _dtos = new List<ItemDto>();
            for (var i = 0; i < 40; i++)
            {
                var itemDto = new ItemDto
                {
                    Id = i + 1,
                    Group = _states.GetRandom(),
                    SortOrder = _priorities.GetRandom()
                };
                _dtos.Add(itemDto);
                Items1.Add(new Item(itemDto));
                Items2.Add(new Item(itemDto));
            }

            DataContext = this;
        }

        private void Update(ItemDto dto)
        {
            var item1 = Items1.FirstOrDefault(m => m.Id == dto.Id);
            item1?.Update(dto);

            var item2 = Items2.FirstOrDefault(m => m.Id == dto.Id);
            item2?.Update(dto);
        }

        public static readonly DependencyProperty IsRunningProperty = DependencyProperty.Register("IsRunning",
            typeof(bool), typeof(MainWindow), new FrameworkPropertyMetadata(false));

        private RelayCommand _refreshCommand;

        public bool IsRunning
        {
            get => (bool) GetValue(IsRunningProperty);
            set => SetValue(IsRunningProperty, value);
        }

        public ICollectionView CollectionView1 { get; set; }
        public ObservableCollection<Item> Items1 { get; }

        public ICollectionView CollectionView2 { get; set; }
        public ObservableCollection<Item> Items2 { get; }

        public ICommand StartCommand => _startCommand = _startCommand ?? new RelayCommand(param => OnStartCommand());
        public ICommand RefreshCommand => _refreshCommand = _refreshCommand ?? new RelayCommand(param => OnRefreshCommand());

        private void OnRefreshCommand()
        {
            CollectionView1.Refresh();
            CollectionView2.Refresh();
        }

        private void OnStartCommand()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _blockingCollection = new BlockingCollection<ItemDto>();
            _cancellationTokenSource.Token.Register(() => _blockingCollection.CompleteAdding());
            IsRunning = true;
            Task.Factory.StartNew(() => ProcessMessagesLoop(_cancellationTokenSource.Token),
                _cancellationTokenSource.Token);
            Task.Factory.StartNew(() => RunSimulation(_cancellationTokenSource.Token), _cancellationTokenSource.Token);
        }

        private void ProcessMessagesLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                if (_blockingCollection.IsAddingCompleted)
                {
                    return;
                }

                try
                {
                    var dto = _blockingCollection.Take();
                    dto.Thread = Thread.CurrentThread.ManagedThreadId.ToString();
                    Update(dto);
                }
                catch (Exception)
                {
                    // _blockingCollection.Take() throws on stop
                }
            }
        }

        private async Task RunSimulation(CancellationToken token)
        {
            foreach (var missionListDto in _dtos)
            {
                if (!_blockingCollection.IsAddingCompleted)
                {
                    _blockingCollection.Add(missionListDto);
                }
            }

            try
            {
                while (!token.IsCancellationRequested)
                {
                    // Simulate server sending updated data to client
                    foreach (var item in _dtos.Randomize().Take(_dtos.Count / 2))
                    {
                        var dto = new ItemDto()
                        {
                            Id = item.Id,
                            Group = _states.GetRandom(),
                            SortOrder = _priorities.GetRandom(),
                        };
                        _blockingCollection.Add(dto);
                    }

                    await Task.Delay(TimeSpan.FromSeconds(2), token);
                }
            }
            catch
            {

            }
        }

        public ICommand StopCommand => _stopCommand = _stopCommand ?? new RelayCommand(_ => OnStopCommand());

        private void OnStopCommand()
        {
            _cancellationTokenSource?.Cancel();
            IsRunning = false;
        }
    }

    internal static class RandomExtensions
    {
        private static readonly Random Random = new Random();

        public static T GetRandom<T>(this List<T> list)
        {
            return list[Random.Next(list.Count)];
        }

        public static IEnumerable<T> Randomize<T>(this IEnumerable<T> target)
        {
            return target.OrderBy(_ => Guid.NewGuid());
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;

        public RelayCommand(Action<object> execute) => _execute = execute;

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter) => _execute(parameter);

        public event EventHandler CanExecuteChanged;
    }
}
