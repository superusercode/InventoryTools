using System.Collections.Generic;
using CriticalCommonLib.Models;
using CriticalCommonLib.Sheets;
using InventoryTools.Logic.Filters.RuleSystem;

namespace InventoryTools.Logic.Filters
{
    public interface IFilter
    {
        public int LabelSize { get; set; }
        public int InputSize { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }
        public string HelpText { get; set; }
        public bool ShowReset { get; set; }
        
        public FilterCategory FilterCategory { get; set; }
        
        public int Order { get; set; }

        public bool HasValueSet(FilterConfiguration configuration);
        
        public FilterType AvailableIn { get; set; }
        public bool? FilterItem(FilterConfiguration configuration, InventoryItem item);
        public bool? FilterItem(FilterConfiguration configuration, ItemEx item);
        public bool? FilterItem(FilterConfiguration configuration, InventoryChange item);
        public bool? FilterItem(FilterRule configuration, InventoryItem item);
        public bool? FilterItem(FilterRule configuration, ItemEx item);
        public bool? FilterItem(FilterRule configuration, InventoryChange item);
        public void Draw(FilterConfiguration configuration);
        public void Draw(FilterRule filterRule);

        public void ResetFilter(FilterConfiguration configuration);
        public void ResetFilter(FilterConfiguration fromConfiguration, FilterConfiguration toConfiguration);

        public void ResetFilter(FilterRule configuration);
        public void ResetFilter(FilterRule fromConfiguration, FilterRule toConfiguration);

        public static readonly List<FilterCategory> FilterCategoryOrder = new() {FilterCategory.Basic, FilterCategory.Columns,FilterCategory.CraftColumns, FilterCategory.IngredientSourcing,FilterCategory.ZonePreference, FilterCategory.Inventories, FilterCategory.Display, FilterCategory.Acquisition, FilterCategory.Searching, FilterCategory.Market, FilterCategory.Searching, FilterCategory.Crafting, FilterCategory.Gathering, FilterCategory.Advanced};
    }
}