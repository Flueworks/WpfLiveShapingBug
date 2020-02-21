using System.Collections;
using System.ComponentModel;
using System.Windows.Data;

namespace GridSortBug
{
    public static class CollectionViewFactory
    {
        public static ICollectionView Create(IList items, bool group)
        {
            var collectionView = new ListCollectionView(items);
            var liveShaping = (ICollectionViewLiveShaping) collectionView;

            collectionView.SortDescriptions.Add(new SortDescription(nameof(Item.SortOrder), ListSortDirection.Descending));

            if (group)
            {
                var statusGroupingRule = new PropertyGroupDescription(nameof(Item.Group));
                collectionView.GroupDescriptions.Add(statusGroupingRule);
            }

            liveShaping.LiveSortingProperties.Add(nameof(Item.SortOrder));
            liveShaping.IsLiveSorting = true;

            liveShaping.LiveGroupingProperties.Add(nameof(Item.Group));
            liveShaping.IsLiveGrouping = true;

            return collectionView;
        }
    }
}