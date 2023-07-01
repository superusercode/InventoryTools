using System.Collections.Generic;
using CriticalCommonLib.Crafting;
using CriticalCommonLib.Models;
using CriticalCommonLib.Sheets;
using InventoryTools.Logic.Filters.Abstract;

namespace InventoryTools.Logic.Filters;

public class CraftRetrieveGroupFilter : ChoiceFilter<RetrieveGroupSetting>
{
    public override RetrieveGroupSetting CurrentValue(FilterConfiguration configuration)
    {
        return configuration.CraftList.RetrieveGroupSetting;
    }

    public override FilterType AvailableIn { get; set; } = FilterType.CraftFilter;
    public override bool? FilterItem(FilterConfiguration configuration, InventoryItem item)
    {
        return null;
    }

    public override bool? FilterItem(FilterConfiguration configuration, ItemEx item)
    {
        return null;
    }

    public override void ResetFilter(FilterConfiguration configuration)
    {
        configuration.CraftList.SetRetrieveGroupSetting(DefaultValue);
        configuration.NotifyConfigurationChange();
    }

    public override void UpdateFilterConfiguration(FilterConfiguration configuration, RetrieveGroupSetting newValue)
    {
        configuration.CraftList.SetRetrieveGroupSetting(newValue);
        configuration.NotifyConfigurationChange();
    }

    public override string Key { get; set; } = "CraftRetrieveGroupFilter";
    public override string Name { get; set; } = "Group Retrieval Items By";

    public override string HelpText { get; set; } =
        "Should the items you need to retrieve be grouped?";

    public override FilterCategory FilterCategory { get; set; } = FilterCategory.Basic;
    public override RetrieveGroupSetting DefaultValue { get; set; } = RetrieveGroupSetting.None;
    public override List<RetrieveGroupSetting> GetChoices(FilterConfiguration configuration)
    {
        return new List<RetrieveGroupSetting>()
        {
            RetrieveGroupSetting.None,
            RetrieveGroupSetting.Together
        };
    }

    public override string GetFormattedChoice(RetrieveGroupSetting choice)
    {
        return choice.ToString();
    }
}