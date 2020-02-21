# WpfLiveShapingBug
LiveShaping bug that occurs when using both sort and group

The bug occurs when we have a CollectionView using LiveShaping, with both a SortDescription and a GroupDescription.
If the item changes both the sort and group property, the resulting list is not sorted.

Initial data:

![Before simulation](https://github.com/Flueworks/WpfLiveShapingBug/blob/master/Images/BeforeSimulation.png)

After changing data

![After simulation](https://github.com/Flueworks/WpfLiveShapingBug/blob/master/Images/AfterSimulation.png)
