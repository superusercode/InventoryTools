using CriticalCommonLib.Models;
using CriticalCommonLib.Sheets;
using InventoryTools.Extensions;
using InventoryTools.Logic.Columns.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Columns;

public class DesynthesisClassColumn : TextColumn
{
    public DesynthesisClassColumn(ILogger<DesynthesisClassColumn> logger, ImGuiService imGuiService) : base(logger, imGuiService)
    {
    }
    public override ColumnCategory ColumnCategory => ColumnCategory.Basic;
    public override string? CurrentValue(ColumnConfiguration columnConfiguration, InventoryItem item)
    {
        return CurrentValue(columnConfiguration, item.Item);
    }

    public override string? CurrentValue(ColumnConfiguration columnConfiguration, ItemEx item)
    {
        if (!item.CanBeDesynthed || item.ClassJobRepair.Row == 0)
        {
            return null;
        }

        return item.ClassJobRepair.Value?.Name.ToString().ToTitleCase() ?? "Unknown";
    }

    public override string? CurrentValue(ColumnConfiguration columnConfiguration, SortingResult item)
    {
        return CurrentValue(columnConfiguration, item.InventoryItem);
    }

    public override string Name { get; set; } = "Desynthesis Class";
    public override string RenderName  => "Desynth Class";
    public override float Width { get; set; } = 100;
    public override string HelpText { get; set; } = "What class is related to de-synthesising this item?";
    public override bool HasFilter { get; set; } = true;
    public override ColumnFilterType FilterType { get; set; } = ColumnFilterType.Text;
}