using System.Collections.Generic;
using CriticalCommonLib.Crafting;
using CriticalCommonLib.Extensions;
using CriticalCommonLib.Models;
using CriticalCommonLib.Sheets;
using InventoryTools.Logic.Filters.Abstract;

namespace InventoryTools.Logic.Filters;

public class CraftPrecraftGroupFilter : ChoiceFilter<PrecraftGroupSetting>
{
    public override PrecraftGroupSetting CurrentValue(FilterConfiguration configuration)
    {
        return configuration.CraftList.PrecraftGroupSetting;
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
        configuration.CraftList.SetPrecraftGroupSetting(DefaultValue);
        configuration.NotifyConfigurationChange();
    }

    public override void UpdateFilterConfiguration(FilterConfiguration configuration, PrecraftGroupSetting newValue)
    {
        configuration.CraftList.SetPrecraftGroupSetting(newValue);
        configuration.NotifyConfigurationChange();
    }

    public override string Key { get; set; } = "CraftPrecraftGroupFilter";
    public override string Name { get; set; } = "Group Precrafts By";

    public override string HelpText { get; set; } =
        "How should precrafts be grouped together?";

    public override FilterCategory FilterCategory { get; set; } = FilterCategory.Basic;
    public override PrecraftGroupSetting DefaultValue { get; set; } = PrecraftGroupSetting.ByClass;
    public override List<PrecraftGroupSetting> GetChoices(FilterConfiguration configuration)
    {
        return new List<PrecraftGroupSetting>()
        {
            PrecraftGroupSetting.ByDepth,
            PrecraftGroupSetting.ByClass,
            PrecraftGroupSetting.Together,
        };
    }

    public override string GetFormattedChoice(PrecraftGroupSetting choice)
    {
        return choice.FormattedName();
    }
}