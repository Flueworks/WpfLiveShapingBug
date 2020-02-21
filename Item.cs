using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GridSortBug
{
    public class Item : INotifyPropertyChanged
    {
        #region Private members

        private int _group;
        private int _id;
        private int _sortOrder;
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
