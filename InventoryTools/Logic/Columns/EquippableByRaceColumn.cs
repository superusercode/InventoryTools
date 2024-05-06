using System.Collections.Generic;
using CriticalCommonLib.Enums;
using CriticalCommonLib.Extensions;
using CriticalCommonLib.Models;
using CriticalCommonLib.Sheets;
using InventoryTools.Logic.Columns.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Columns
{
    public class EquippableByRaceColumn : TextColumn
    {
        public EquippableByRaceColumn(ILogger<EquippableByRaceColumn> logger, ImGuiService imGuiService) : base(logger, imGuiService)
        {
        }
        public override ColumnCategory ColumnCategory => ColumnCategory.Basic;

        public override string? CurrentValue(ColumnConfiguration columnConfiguration, InventoryItem item)
        {
            return CurrentValue(columnConfiguration, item.Item);
        }

        public override string? CurrentValue(ColumnConfiguration columnConfiguration, ItemEx item)
        {
            return item.EquipRace.FormattedName();
        }

        public override string? CurrentValue(ColumnConfiguration columnConfiguration, SortingResult item)
        {
            return CurrentValue(columnConfiguration, item.InventoryItem);
        }

        public override string Name { get; set; } = "Equipped By (Race)";
        public override float Width { get; set; } = 200;
        public override string HelpText { get; set; } = "Shows if an item can be equipped by a specific race.";
        public override bool HasFilter { get; set; } = true;
        public override ColumnFilterType FilterType { get; set; } = ColumnFilterType.Choice;

        public override List<string>? FilterChoices { get; set; } = new List<string>()
        {
            CharacterRace.Hyur.FormattedName(),
            CharacterRace.Elezen.FormattedName(),
            CharacterRace.Lalafell.FormattedName(),
            CharacterRace.Miqote.FormattedName(),
            CharacterRace.Roegadyn.FormattedName(),
            CharacterRace.Viera.FormattedName(),
            CharacterRace.AuRa.FormattedName(),
            CharacterRace.None.FormattedName(),
        };
    }
}