using CriticalCommonLib.Models;
using CriticalCommonLib.Sheets;
using InventoryTools.Logic.Filters.Abstract;

namespace InventoryTools.Logic.Filters;

public class IsMobDropFilter : BooleanFilter
{
    public override string Key { get; set; } = "IsMobDrop";
    public override string Name { get; set; } = "Is Dropped by Mobs?";
    public override string HelpText { get; set; } = "Is this item dropped by mobs?";
    public override FilterCategory FilterCategory { get; set; } = FilterCategory.Acquisition;
    public override FilterType AvailableIn { get; set; } =
        FilterType.SearchFilter | FilterType.SortingFilter | FilterType.GameItemFilter | FilterType.HistoryFilter;
    public override bool? FilterItem(FilterConfiguration configuration, InventoryItem item)
    {
        return FilterItem(configuration, item.Item);
    }

    public override bool? FilterItem(FilterConfiguration configuration, ItemEx item)
    {
        var currentValue = CurrentValue(configuration);
        if (currentValue == null)
        {
            return null;
        }

        switch (currentValue.Value)
        {
            case false:
                return !item.HasMobDrops();
            case true:
                return item.HasMobDrops();
        }
    }
}