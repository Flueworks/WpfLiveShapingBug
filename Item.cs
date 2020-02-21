using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace GridSortBug
{
    internal static class BrushConverter
    {
        private static Dictionary<int, Brush> _brushMapping = new Dictionary<int, Brush>()
        {
            {0, Brushes.Black},
            {1, Brushes.DarkRed},
            {2, Brushes.DarkGreen},
            {3, Brushes.DarkBlue}
        };
        static BrushConverter()
        {
            foreach (var item in _brushMapping.Values)
            {
                item.Freeze();
            }
        }
        public static Brush GetBackgroundForGroup(int @group)
        {
            if(_brushMapping.TryGetValue(@group, out var brush))
            {
                return brush;
            }

            return Brushes.Transparent;
        }
    }
    public class Item : INotifyPropertyChanged
    {
        #region Private members

        private int _group;
        private int _id;
        private int _sortOrder;
        private Brush _background;
        private int _width;

        #endregion

        public Item(ItemDto source)
        {
            Id = source.Id;
            Update(source);
        }
        #region Public properties

        public int Id
        {
            get => _id;
            set
            {
                if (_id == value)
                    return;
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        public int SortOrder
        {
            get => _sortOrder;
            set
            {
                if (_sortOrder == value)
                    return;
                _sortOrder = value;
                OnPropertyChanged(nameof(SortOrder));
            }
        }

        public int Group
        {
            get => _group;
            set
            {
                if (_group == value)
                    return;
                _group = value;
                OnPropertyChanged(nameof(Group));
            }
        }

        #endregion
        public void Update(ItemDto dto)
        {
            SortOrder = dto.SortOrder;
            Group = dto.Group;
            Thread = dto.Thread;
            Background = BrushConverter.GetBackgroundForGroup(Group);
            Width = SortOrder * 10;
        }

        public int Width
        {
            get => _width;
            set
            {
                if (_width != value)
                {
                    _width = value;
                    OnPropertyChanged(nameof(Width));
                }
            }
        }

        public Brush Background
        {
            get => _background;
            set
            {
                if(_background != value)
                {
                    _background = value;
                    OnPropertyChanged(nameof(Background));
                }
            }
        }

        public string Thread { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class ItemDto
    {
        public int Id { get; set; }
        public int SortOrder { get; set; }
        public int Group { get; set; }
        public string Thread { get; set; }
    }
}
