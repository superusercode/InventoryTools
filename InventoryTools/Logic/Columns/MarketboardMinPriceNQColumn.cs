using CriticalCommonLib.Models;
using CriticalCommonLib.Sheets;
using Dalamud.Interface.Colors;
using ImGuiNET;
using InventoryTools.Logic.Columns.Abstract;
using InventoryTools.Ui.Widgets;

namespace InventoryTools.Logic.Columns
{
    public class MarketBoardMinPriceNQColumn : GilColumn
    {
        public override ColumnCategory ColumnCategory => ColumnCategory.Market;

        protected readonly string LoadingString = "loading...";
        protected readonly string UntradableString = "untradable";
        protected readonly int Loading = -1;
        protected readonly int Untradable = -2;

        public override void Draw(FilterConfiguration configuration, InventoryItem item, int rowIndex)
        {
            var result = DoDraw(CurrentValue(item), rowIndex, configuration);
            result?.HandleEvent(configuration,item);
        }
        public override void Draw(FilterConfiguration configuration, SortingResult item, int rowIndex)
        {
            var result = DoDraw(CurrentValue(item), rowIndex, configuration);
            result?.HandleEvent(configuration,item);
        }
        public override void Draw(FilterConfiguration configuration, ItemEx item, int rowIndex)
        {
            var result = DoDraw(CurrentValue((ItemEx)item), rowIndex, configuration);
            result?.HandleEvent(configuration,item);
        }

        public override IColumnEvent? DoDraw(int? currentValue, int rowIndex, FilterConfiguration filterConfiguration)
        {
            if (currentValue.HasValue && currentValue.Value == Loading)
            {
                ImGui.TableNextColumn();
                ImGuiUtil.VerticalAlignTextColored(LoadingString, ImGuiColors.DalamudYellow, filterConfiguration.TableHeight, false);
            }
            else if (currentValue.HasValue && currentValue.Value == Untradable)
            {
                ImGui.TableNextColumn();
                ImGuiUtil.VerticalAlignTextColored(UntradableString, ImGuiColors.DalamudRed, filterConfiguration.TableHeight, false);
            }
            else if(currentValue.HasValue)
            {
                base.DoDraw(currentValue, rowIndex, filterConfiguration);
                ImGui.SameLine();
                if (ImGui.SmallButton("R##" + rowIndex))
                {
                    return new RefreshPricingEvent();
                }
            }
            else
            {
                base.DoDraw(currentValue, rowIndex, filterConfiguration);
            }
            return null;
        }

        public override int? CurrentValue(InventoryItem item)
        {
            if (!item.CanBeTraded)
            {
                return Untradable;
            }

            var marketBoardData = PluginService.MarketCache.GetPricing(item.ItemId, false);
            if (marketBoardData != null)
            {
                var hq = marketBoardData.minPriceNQ;
                return (int)hq;
            }

            return Loading;
        }

        public override int? CurrentValue(ItemEx item)
        {
            if (!item.CanBeTraded)
            {
                return Untradable;
            }

            var marketBoardData = PluginService.MarketCache.GetPricing(item.RowId, false);
            if (marketBoardData != null)
            {
                var hq = marketBoardData.minPriceNQ;
                return (int)hq;
            }

            return Loading;
        }

        public override int? CurrentValue(SortingResult item)
        {
            return CurrentValue(item.InventoryItem);
        }

        public override string Name { get; set; } = "Market Board Minimum Price NQ";
        public override string RenderName => "MB Min. Price NQ";
        public override string HelpText { get; set; } =
            "Shows the minimum price of the NQ form of the item. This data is sourced from universalis.";
        public override float Width { get; set; } = 250.0f;
        public override bool HasFilter { get; set; } = true;
        public override ColumnFilterType FilterType { get; set; } = ColumnFilterType.Text;
    }
}