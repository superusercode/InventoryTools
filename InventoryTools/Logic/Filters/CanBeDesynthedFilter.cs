using CriticalCommonLib.Models;
using CriticalCommonLib.Sheets;
using InventoryTools.Logic.Filters.Abstract;

namespace InventoryTools.Logic.Filters
{
    public class CanBeDesynthedFilter : BooleanFilter
    {
        public override string Key { get; set; } = "desynth";
        public override string Name { get; set; } = "Can be Desynthed?";
        public override string HelpText { get; set; } = "Can this item be desynthesised?";
        public override FilterCategory FilterCategory { get; set; } = FilterCategory.Basic;

        public override FilterType AvailableIn { get; set; } =
            FilterType.SearchFilter | FilterType.SortingFilter | FilterType.GameItemFilter | FilterType.HistoryFilter;
        
        public override bool? FilterItem(bool? filterValue, InventoryItem item)
        {
            return FilterItem(filterValue, item.Item);
        }

        public override bool? FilterItem(bool? filterValue, ItemEx item)
        {
            if (filterValue == null)
            {
                return null;
            }
            return filterValue.Value && item.CanBeDesynthed || !filterValue.Value && !item.CanBeDesynthed;
        }

        public override bool? FilterItem(bool? filterValue, InventoryChange item)
        {
            return FilterItem(filterValue, item.InventoryItem);
        }
    }
}