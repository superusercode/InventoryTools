using CriticalCommonLib.Models;
using CriticalCommonLib.Sheets;
using InventoryTools.Logic.Filters.RuleSystem;

namespace InventoryTools.Logic.Filters.Abstract
{
    public abstract class Filter<T> : IFilter
    {
        public virtual int LabelSize { get; set; } = 220;
        public virtual int InputSize { get; set; } = 250;
        public abstract T CurrentValue(FilterConfiguration configuration);
        public abstract void Draw(FilterConfiguration configuration);
        public abstract void ResetFilter(FilterConfiguration configuration);
        public void ResetFilter(FilterConfiguration fromConfiguration, FilterConfiguration toConfiguration)
        {
            var currentValue = CurrentValue(fromConfiguration);
            UpdateFilterConfiguration(toConfiguration, currentValue);
        }
        public abstract T CurrentValue(FilterRule filterRule);
        public abstract void Draw(FilterRule filterRule);
        public abstract void ResetFilter(FilterRule filterRule);
        public void ResetFilter(FilterRule fromFilterRule, FilterRule toFilterRule)
        {
            var currentValue = CurrentValue(fromFilterRule);
            UpdateFilterConfiguration(toFilterRule, currentValue);
        }

        public abstract void UpdateFilterConfiguration(FilterConfiguration configuration, T newValue);

        public abstract void UpdateFilterConfiguration(FilterRule filterRule, T newValue);

        public abstract string Key { get; set; }
        public abstract string Name { get; set; }
        public abstract string HelpText { get; set; }
        
        public abstract FilterCategory FilterCategory { get; set; }

        public virtual int Order { get; set; } = 0;
        public virtual bool ShowReset { get; set; } = true;
        public abstract T DefaultValue { get; set; }

        public abstract bool HasValueSet(FilterConfiguration configuration);
        public abstract bool HasValueSet(FilterRule filterRule);
        public abstract FilterType AvailableIn { get; set; }

        public virtual bool? FilterItem(FilterConfiguration configuration, InventoryItem item)
        {
            return FilterItem(CurrentValue(configuration), item);
        }

        public virtual bool? FilterItem(FilterConfiguration configuration, ItemEx item)
        {
            return FilterItem(CurrentValue(configuration), item);
        }

        public virtual bool? FilterItem(FilterConfiguration configuration, InventoryChange item)
        {
            return FilterItem(configuration,item.InventoryItem);
        }

        public virtual bool? FilterItem(FilterRule filterRule, InventoryItem item)
        {
            return FilterItem(CurrentValue(filterRule), item);
        }

        public virtual bool? FilterItem(FilterRule filterRule, ItemEx item)
        {
            return FilterItem(CurrentValue(filterRule), item);
        }

        public virtual bool? FilterItem(FilterRule filterRule, InventoryChange item)
        {
            return FilterItem(filterRule,item.InventoryItem);
        }
        
        
        public abstract bool? FilterItem(T? filterValue,InventoryItem item);
        public abstract bool? FilterItem(T? filterValue, ItemEx item);
        public abstract bool? FilterItem(T? filterValue, InventoryChange item);

    }
}