﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Numerics;
using CriticalCommonLib.Models;
using Dalamud.Configuration;
using Dalamud.Game.ClientState.Keys;
using Dalamud.Interface.Colors;
using InventoryTools.Extensions;
using InventoryTools.Logic;
using InventoryTools.Logic.Settings;
using Newtonsoft.Json;
using OtterGui.Classes;

namespace InventoryTools
{
    [Serializable]
    public class InventoryToolsConfiguration : IPluginConfiguration
    {
        public delegate void ConfigurationChangedDelegate();

        private bool _automaticallyDownloadMarketPrices = false;
        private bool _colorRetainerList = true;

        private bool _displayCrossCharacter = true;
        private bool _displayTooltip = true;

        private Vector4 _highlightColor = new (0.007f, 0.008f,
            0.007f, 0.212f);

        private Vector4 _destinationHighlightColor = new Vector4(0.321f, 0.239f, 0.03f, 1f);

        private Vector4 _retainerListColor = ImGuiColors.HealerGreen;

        private string _highlightWhen = "Always";
        private bool _invertHighlighting = true;
        private bool _invertDestinationHighlighting = false;
        private bool _invertTabHighlighting = false;
        private bool _highlightDestination = false;
        private bool _highlightDestinationEmpty = false;
        private bool _addMoreInformationContextMenu = false;

        private bool _isVisible;
        private int _marketRefreshTimeHours = 24;
        private int _marketSaleHistoryLimit = 7;
        private bool _showItemNumberRetainerList = true;

        private Vector4 _tabHighlightColor = new (0.007f, 0.008f,
            0.007f, 1.0f);

        public List<FilterConfiguration> FilterConfigurations = new();

        public Dictionary<ulong, Character> SavedCharacters = new();

        private Dictionary<ulong, HashSet<uint>> _acquiredItems = new();
        public bool InventoriesMigrated { get; set; } = false;
        public bool InventoriesMigratedToCsv { get; set; } = false;

        private HashSet<string>? _openWindows = new();
        private Dictionary<string, Vector2>? _savedWindowPositions = new();

        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                _isVisible = value;
                PluginService.FrameworkService.RunOnFrameworkThread(() => { ConfigurationChanged?.Invoke(); });
            }
        }

        public bool AddMoreInformationContextMenu
        {
            get => _addMoreInformationContextMenu;
            set
            {
                _addMoreInformationContextMenu = value;
                PluginService.FrameworkService.RunOnFrameworkThread(() => { ConfigurationChanged?.Invoke(); });
            }
        }

        public int SelectedConfigurationPage { get; set; }
        public bool ShowFilterTab { get; set; } = true;
        public bool SwitchFiltersAutomatically { get; set; } = true;
        private bool _tooltipCurrentCharacter = false;
        private bool _tooltipDisplayAmountOwned = true;
        private bool _tooltipDisplayMarketAveragePrice = true;
        private bool _tooltipDisplayMarketLowestPrice = false;
        private bool _tooltipAddCharacterNameOwned = false;
        private bool _tooltipDisplayRetrieveAmount = false;
        private int _tooltipLocationLimit = 10;
        private bool _tooltipDisplayHeader = false;
        private int _tooltipHeaderLines = 0;
        private int _tooltipFooterLines = 0;
        private TooltipLocationDisplayMode _tooltipLocationDisplayMode = TooltipLocationDisplayMode.CharacterCategoryQuantityQuality;
        private WindowLayout _craftWindowLayout =  WindowLayout.Tabs;
        private WindowLayout _filtersLayout = WindowLayout.Tabs;
        private uint? _tooltipColor = null;
        public Vector4 HighlightColor
        {
            get => _highlightColor;
            set
            {
                _highlightColor = value;
                PluginService.FrameworkService.RunOnFrameworkThread(() => { ConfigurationChanged?.Invoke(); });
            }
        }
        public Vector4 DestinationHighlightColor
        {
            get => _destinationHighlightColor;
            set
            {
                _destinationHighlightColor = value;
                PluginService.FrameworkService.RunOnFrameworkThread(() => { ConfigurationChanged?.Invoke(); });
            }
        }

        public Vector4 RetainerListColor
        {
            get => _retainerListColor;
            set
            {
                _retainerListColor = value;
                PluginService.FrameworkService.RunOnFrameworkThread(() => { ConfigurationChanged?.Invoke(); });
            }
        }

        public Vector4 TabHighlightColor
        {
            get => _tabHighlightColor;
            set
            {
                _tabHighlightColor = value;
                PluginService.FrameworkService.RunOnFrameworkThread(() => { ConfigurationChanged?.Invoke(); });
            }
        }

        public bool DisplayCrossCharacter
        {
            get => _displayCrossCharacter;
            set
            {
                _displayCrossCharacter = value;
                PluginService.FrameworkService.RunOnFrameworkThread(() => { ConfigurationChanged?.Invoke(); });
            }
        }

        public bool DisplayTooltip
        {
            get => _displayTooltip;
            set
            {
                _displayTooltip = value;
                PluginService.FrameworkService.RunOnFrameworkThread(() => { ConfigurationChanged?.Invoke(); });
            }
        }

        public bool TooltipCurrentCharacter
        {
            get => _tooltipCurrentCharacter;
            set
            {
                _tooltipCurrentCharacter = value;
                PluginService.FrameworkService.RunOnFrameworkThread(() => { ConfigurationChanged?.Invoke(); });
            }
        }
        
        public bool TooltipDisplayAmountOwned
        {
            get => _tooltipDisplayAmountOwned;
            set
            {
                _tooltipDisplayAmountOwned = value;
                PluginService.FrameworkService.RunOnFrameworkThread(() => { ConfigurationChanged?.Invoke(); });
            }
        }
        
        public bool TooltipAddCharacterNameOwned
        {
            get => _tooltipAddCharacterNameOwned;
            set
            {
                _tooltipAddCharacterNameOwned = value;
                PluginService.FrameworkService.RunOnFrameworkThread(() => { ConfigurationChanged?.Invoke(); });
            }
        }

        public bool TooltipDisplayMarketAveragePrice
        {
            get => _tooltipDisplayMarketAveragePrice;
            set
            {
                _tooltipDisplayMarketAveragePrice = value;
                PluginService.FrameworkService.RunOnFrameworkThread(() => { ConfigurationChanged?.Invoke(); });
            }
        }

        public bool TooltipDisplayMarketLowestPrice
        {
            get => _tooltipDisplayMarketLowestPrice;
            set
            {
                _tooltipDisplayMarketLowestPrice = value;
                PluginService.FrameworkService.RunOnFrameworkThread(() => { ConfigurationChanged?.Invoke(); });
            }
        }

        public bool TooltipDisplayRetrieveAmount
        {
            get => _tooltipDisplayRetrieveAmount;
            set
            {
                _tooltipDisplayRetrieveAmount = value;
                PluginService.FrameworkService.RunOnFrameworkThread(() => { ConfigurationChanged?.Invoke(); });
            }
        }

        public int TooltipLocationLimit
        {
            get => _tooltipLocationLimit;
            set
            {
                _tooltipLocationLimit = value;
                PluginService.FrameworkService.RunOnFrameworkThread(() => { ConfigurationChanged?.Invoke(); });
            }
        }
        public TooltipLocationDisplayMode TooltipLocationDisplayMode
        {
            get => _tooltipLocationDisplayMode;
            set
            {
                _tooltipLocationDisplayMode = value;
                PluginService.FrameworkService.RunOnFrameworkThread(() => { ConfigurationChanged?.Invoke(); });
            }
        }
        public WindowLayout CraftWindowLayout
        {
            get => _craftWindowLayout;
            set
            {
                _craftWindowLayout = value;
                PluginService.FrameworkService.RunOnFrameworkThread(() => { ConfigurationChanged?.Invoke(); });
            }
        }
        public WindowLayout FiltersLayout
        {
            get => _filtersLayout;
            set
            {
                _filtersLayout = value;
                PluginService.FrameworkService.RunOnFrameworkThread(() => { ConfigurationChanged?.Invoke(); });
            }
        }

        public bool AutomaticallyDownloadMarketPrices
        {
            get => _automaticallyDownloadMarketPrices;
            set
            {
                _automaticallyDownloadMarketPrices = value;
                PluginService.FrameworkService.RunOnFrameworkThread(() => { ConfigurationChanged?.Invoke(); });
            }
        }

        public int MarketRefreshTimeHours
        {
            get => _marketRefreshTimeHours;
            set
            {
                _marketRefreshTimeHours = value;
                if (_marketRefreshTimeHours == 0)
                {
                    _marketRefreshTimeHours = 24;
                }
                PluginService.FrameworkService.RunOnFrameworkThread(() => { ConfigurationChanged?.Invoke(); });
            }
        }

        public int MarketSaleHistoryLimit
        {
            get => _marketSaleHistoryLimit;
            set
            {
                _marketSaleHistoryLimit = value;
                if (_marketSaleHistoryLimit == 0)
                {
                    _marketSaleHistoryLimit = 7;
                }
                PluginService.FrameworkService.RunOnFrameworkThread(() => { ConfigurationChanged?.Invoke(); });
            }
        }

        public bool ColorRetainerList
        {
            get => _colorRetainerList;
            set
            {
                _colorRetainerList = value;
                PluginService.FrameworkService.RunOnFrameworkThread(() => { ConfigurationChanged?.Invoke(); });
            }
        }

        public bool ShowItemNumberRetainerList
        {
            get => _showItemNumberRetainerList;
            set
            {
                _showItemNumberRetainerList = value;
                PluginService.FrameworkService.RunOnFrameworkThread(() => { ConfigurationChanged?.Invoke(); });
            }
        }

        public bool InvertHighlighting
        {
            get => _invertHighlighting;
            set
            {
                _invertHighlighting = value;
                PluginService.FrameworkService.RunOnFrameworkThread(() => { ConfigurationChanged?.Invoke(); });
            }
        }

        public bool InvertDestinationHighlighting
        {
            get => _invertDestinationHighlighting;
            set
            {
                _invertDestinationHighlighting = value;
                PluginService.FrameworkService.RunOnFrameworkThread(() => { ConfigurationChanged?.Invoke(); });
            }
        }

        public bool InvertTabHighlighting
        {
            get => _invertTabHighlighting;
            set
            {
                _invertTabHighlighting = value;
                PluginService.FrameworkService.RunOnFrameworkThread(() => { ConfigurationChanged?.Invoke(); });
            }
        }

        public string HighlightWhen
        {
            get => _highlightWhen;
            set
            {
                _highlightWhen = value;
                PluginService.FrameworkService.RunOnFrameworkThread(() => { ConfigurationChanged?.Invoke(); });
            }
        }

        public bool HighlightDestination
        {
            get => _highlightDestination;
            set
            {
                _highlightDestination = value;
                PluginService.FrameworkService.RunOnFrameworkThread(() => { ConfigurationChanged?.Invoke(); });
            }
        }

        public bool HighlightDestinationEmpty
        {
            get => _highlightDestinationEmpty;
            set
            {
                _highlightDestinationEmpty = value;
                PluginService.FrameworkService.RunOnFrameworkThread(() => { ConfigurationChanged?.Invoke(); });
            }
        }

        public bool TrackMobSpawns
        {
            get => _trackMobSpawns;
            set
            {
                _trackMobSpawns = value;
                PluginService.FrameworkService.RunOnFrameworkThread(() => { ConfigurationChanged?.Invoke(); });
            }
        }
        
        public bool TooltipDisplayHeader
        {
            get => _tooltipDisplayHeader;
            set
            {
                _tooltipDisplayHeader = value;
                PluginService.FrameworkService.RunOnFrameworkThread(() => { ConfigurationChanged?.Invoke(); });
            }
        }
        
        public Dictionary<ulong, HashSet<uint>> AcquiredItems
        {
            get => _acquiredItems ?? new Dictionary<ulong, HashSet<uint>>();
            set => _acquiredItems = value;
        }

        public string? ActiveUiFilter { get; set; } = null;

        public bool TetrisEnabled { get; set; } = false;

        public string? ActiveBackgroundFilter { get; set; } = null;
        public bool SaveBackgroundFilter { get; set; } = false;

        public bool FirstRun { get; set; } = true;
        public bool IntroShown { get; set; } = false;
        public int SelectedHelpPage { get; set; }
        #if DEBUG
        public int SelectedDebugPage { get; set; }
        #endif
        public bool AutoSave { get; set; } = true;
        public int AutoSaveMinutes { get; set; } = 10;
        public int InternalVersion { get; set; } = 0;
        public int Version { get; set; }

        public uint? TooltipColor
        {
            get => _tooltipColor;
            set => _tooltipColor = value;
        }

        public ModifiableHotkey? MoreInformationHotKey
        {
            get => _moreInformationHotKey;
            set => _moreInformationHotKey = value;
        }
        
        public ConcurrentDictionary<string,ModifiableHotkey> Hotkeys
        {
            get
            {
                if (_hotkeys == null)
                {
                    _hotkeys = new ConcurrentDictionary<string, ModifiableHotkey>();
                }
                return _hotkeys;
            }
            set
            {
                _hotkeys = value;
            }
        }

        private ModifiableHotkey? _moreInformationHotKey;
        private ConcurrentDictionary<string,ModifiableHotkey>? _hotkeys;
        private bool _trackMobSpawns = false;

        public ModifiableHotkey? GetHotkey(string hotkey)
        {
            if(Hotkeys.TryGetValue(hotkey, out var modifiableHotkey))
            {
                return modifiableHotkey;
            }

            return null;
        }

        public HashSet<string> OpenWindows
        {
            get
            {
                if (_openWindows == null)
                {
                    _openWindows = new HashSet<string>();
                }
                return _openWindows;
            }
            set => _openWindows = value;
        }
        public Dictionary<string, Vector2> SavedWindowPositions
        {
            get
            {
                if (_savedWindowPositions == null)
                {
                    _savedWindowPositions = new Dictionary<string, Vector2>();
                }
                return _savedWindowPositions;
            }
            set => _savedWindowPositions = value;
        }

        public int TooltipHeaderLines
        {
            get => _tooltipHeaderLines;
            set
            {
                _tooltipHeaderLines = value;
                PluginService.FrameworkService.RunOnFrameworkThread(() => { ConfigurationChanged?.Invoke(); });
            }
        }

        public int TooltipFooterLines
        {
            get => _tooltipFooterLines;
            set
            {
                _tooltipFooterLines = value;
                PluginService.FrameworkService.RunOnFrameworkThread(() => { ConfigurationChanged?.Invoke(); });
            }
        }
        
        public event ConfigurationChangedDelegate? ConfigurationChanged;

        //Configuration Helpers

        public Dictionary<ulong, Character> GetSavedRetainers()
        {
            return SavedCharacters;
        }

        public List<FilterConfiguration> GetSavedFilters()
        {
            return FilterConfigurations;
        }

        public void MarkReloaded()
        {
            if (!SaveBackgroundFilter)
            {
                ActiveBackgroundFilter = null;
            }
        }

        public void RestoreServiceSettings()
        {
            PluginService.MarketCache.CacheAutoRetrieve = _automaticallyDownloadMarketPrices;
            PluginService.MarketCache.CacheTimeHours = _marketRefreshTimeHours;
            PluginService.Universalis.SetSaleHistoryLimit(_marketRefreshTimeHours);
        }
    }
}