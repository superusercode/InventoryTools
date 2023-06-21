using System.Collections.Generic;
using System.Numerics;
using CriticalCommonLib.Crafting;
using CriticalCommonLib.Models;
using CriticalCommonLib.Sheets;
using ImGuiNET;

namespace InventoryTools.Logic.Columns.Abstract
{
    public abstract class TextIconColumn : Column<(string,ushort,bool)?>
    {
        public override string CsvExport(InventoryItem item)
        {
            return "";
        }
        

        public override string CsvExport(ItemEx item)
        {
            return "";
        }

        public override string CsvExport(SortingResult item)
        {
            return "";
        }
        public override (string,ushort,bool)? CurrentValue(CraftItem currentValue)
        {
            return CurrentValue(currentValue.Item);
        }
        
        public override (string,ushort,bool)? CurrentValue(InventoryChange currentValue)
        {
            return CurrentValue(currentValue.InventoryItem);
        }
        
        public override IEnumerable<CraftItem> Filter(IEnumerable<CraftItem> items)
        {
            return items;
        }

        public override IEnumerable<CraftItem> Sort(ImGuiSortDirection direction, IEnumerable<CraftItem> items)
        {
            return items;
        }
        public virtual string EmptyText
        {
            get
            {
                return "N/A";
            }
        }
        public override void Draw(FilterConfiguration configuration, InventoryItem item, int rowIndex)
        {
            var result = DoDraw(CurrentValue(item), rowIndex, configuration);
            result?.HandleEvent(configuration, item);
        }
        public override void Draw(FilterConfiguration configuration, SortingResult item, int rowIndex)
        {
            var result = DoDraw(CurrentValue(item), rowIndex, configuration);
            result?.HandleEvent(configuration, item);
        }
        public override void Draw(FilterConfiguration configuration, ItemEx item, int rowIndex)
        {
            var result = DoDraw(CurrentValue((ItemEx)item), rowIndex, configuration);
            result?.HandleEvent(configuration, item);
        }
        public override void Draw(FilterConfiguration configuration, CraftItem item, int rowIndex)
        {
            var result = DoDraw(CurrentValue(item), rowIndex, configuration);
            result?.HandleEvent(configuration, item);
        }
        public override void Draw(FilterConfiguration configuration, InventoryChange item, int rowIndex)
        {
            DoDraw(CurrentValue(item), rowIndex, configuration);
        }
        public override IEnumerable<ItemEx> Filter(IEnumerable<ItemEx> items)
        {
            return items;
        }

        public override IEnumerable<InventoryItem> Filter(IEnumerable<InventoryItem> items)
        {
            return items;
        }

        public override IEnumerable<SortingResult> Filter(IEnumerable<SortingResult> items)
        {
            return items;
        }

        public override IEnumerable<InventoryChange> Filter(IEnumerable<InventoryChange> items)
        {
            return items;
        }

        public override IEnumerable<InventoryItem> Sort(ImGuiSortDirection direction, IEnumerable<InventoryItem> items)
        {
            return items;
        }

        public override IEnumerable<ItemEx> Sort(ImGuiSortDirection direction, IEnumerable<ItemEx> items)
        {
            return items;
        }

        public override IEnumerable<SortingResult> Sort(ImGuiSortDirection direction, IEnumerable<SortingResult> items)
        {
            return items;
        }

        public override IEnumerable<InventoryChange> Sort(ImGuiSortDirection direction, IEnumerable<InventoryChange> items)
        {
            return items;
        }

        public override IColumnEvent? DoDraw((string, ushort, bool)? currentValue, int rowIndex,
            FilterConfiguration filterConfiguration)
        {
            ImGui.TableNextColumn();
            if (currentValue != null)
            {
                PluginService.PluginLogic.DrawIcon(currentValue.Value.Item2, new Vector2(filterConfiguration.TableHeight, filterConfiguration.TableHeight) * ImGui.GetIO().FontGlobalScale, currentValue.Value.Item3);
                ImGui.SameLine();
                if (filterConfiguration.FilterType == Logic.FilterType.CraftFilter)
                {
                    ImGui.TextWrapped(currentValue.Value.Item1);
                }
                else
                {
                    ImGui.Text(currentValue.Value.Item1);
                }
            }
            return null;
        }

        public override void Setup(int columnIndex)
        {
            ImGui.TableSetupColumn(RenderName ?? Name, ImGuiTableColumnFlags.WidthFixed, Width, (uint)columnIndex);
        }
    }
}