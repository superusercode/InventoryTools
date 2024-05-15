﻿using CriticalCommonLib.MarketBoard;
using CriticalCommonLib.Models;
using CriticalCommonLib.Services;
using CriticalCommonLib.Sheets;
using InventoryTools.Logic.Columns.Abstract;
using InventoryTools.Logic.Columns.ColumnSettings;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Columns
{
    public class MarketBoardMinPriceColumn : MarketBoardPriceColumn
    {
        private readonly ICharacterMonitor _characterMonitor;
        private readonly IMarketCache _marketCache;

        public MarketBoardMinPriceColumn(ILogger<MarketBoardMinPriceColumn> logger, ImGuiService imGuiService, MarketboardWorldSetting marketboardWorldSetting, ICharacterMonitor characterMonitor, IMarketCache marketCache, ExcelCache excelCache) : base(logger, imGuiService, marketboardWorldSetting, characterMonitor, marketCache, excelCache)
        {
            _characterMonitor = characterMonitor;
            _marketCache = marketCache;
        }
        public override ColumnCategory ColumnCategory => ColumnCategory.Market;

        public override string HelpText { get; set; } =
            "Shows the minimum price of both the NQ and HQ form of the item. If no world is selected, your home world is used. This data is sourced from universalis.";
        
        public override (int, int)? CurrentValue(ColumnConfiguration columnConfiguration, InventoryItem item)
        {
            if (!item.CanBeTraded)
            {
                return (Untradable, Untradable);
            }
            var activeCharacter = _characterMonitor.ActiveCharacter;
            if (activeCharacter != null)
            {
                var selectedWorldId = MarketboardWorldSetting.SelectedWorldId(columnConfiguration, activeCharacter);

                var marketBoardData = _marketCache.GetPricing(item.ItemId, selectedWorldId, false);
                if (marketBoardData != null)
                {
                    var nq = marketBoardData.MinPriceNq;
                    var hq = marketBoardData.MinPriceHq;
                    return ((int)nq, (int)hq);
                }
            }

            return (Loading, Loading);
        }

        public override (int, int)? CurrentValue(ColumnConfiguration columnConfiguration, ItemEx item)
        {
            if (!item.CanBeTraded)
            {
                return (Untradable, Untradable);
            }
            var activeCharacter = _characterMonitor.ActiveCharacter;
            if (activeCharacter != null)
            {
                var selectedWorldId = MarketboardWorldSetting.SelectedWorldId(columnConfiguration, activeCharacter);

                var marketBoardData = _marketCache.GetPricing(item.RowId, selectedWorldId, false);
                if (marketBoardData != null)
                {
                    var nq = marketBoardData.MinPriceNq;
                    var hq = marketBoardData.MinPriceHq;
                    return ((int)nq, (int)hq);
                }
            }

            return (Loading, Loading);
        }

        public override (int, int)? CurrentValue(ColumnConfiguration columnConfiguration, SortingResult item)
        {
            return CurrentValue(columnConfiguration, item.InventoryItem);
        }

        public override string Name { get; set; } = "Market Board Minimum Price NQ/HQ";
        public override string RenderName => "MB Min. Price NQ/HQ";
        
        public override FilterType DefaultIn => Logic.FilterType.CraftFilter;
    }
}