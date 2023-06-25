using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using CriticalCommonLib;
using CriticalCommonLib.Enums;
using CriticalCommonLib.Models;
using CriticalCommonLib.Services.Ui;
using Dalamud.Logging;
using Lumina.Excel.GeneratedSheets;

namespace InventoryTools.Logic
{
    public class FilterState
    {
        public FilterConfiguration FilterConfiguration;
        public RenderTableBase? FilterTable;
        public ulong? ActiveRetainerId => PluginService.CharacterMonitor.ActiveRetainer == 0 ? null : PluginService.CharacterMonitor.ActiveRetainer;
        public ulong? ActiveFreeCompanyId => PluginService.CharacterMonitor.ActiveFreeCompanyId == 0 ? null : PluginService.CharacterMonitor.ActiveFreeCompanyId;
        public ulong? ActiveHousingId => PluginService.CharacterMonitor.ActiveHouseId == 0 ? null : PluginService.CharacterMonitor.ActiveHouseId;

        public bool InvertHighlighting
        {
            get
            {
                var activeFilter = FilterConfiguration;
                return activeFilter.InvertHighlighting ?? ConfigurationManager.Config.InvertHighlighting;
            }
        }

        public bool InvertDestinationHighlighting
        {
            get
            {
                var activeFilter = FilterConfiguration;
                return activeFilter.InvertDestinationHighlighting ?? ConfigurationManager.Config.InvertDestinationHighlighting;
            }
        }

        public bool InvertTabHighlighting
        {
            get
            {
                var activeFilter = FilterConfiguration;
                return activeFilter.InvertTabHighlighting ?? ConfigurationManager.Config.InvertTabHighlighting;
            }
        }

        public Vector4 BagHighlightColor => FilterConfiguration.HighlightColor ?? ConfigurationManager.Config.HighlightColor;
        public Vector4 BagDestinationHighlightColor => FilterConfiguration.DestinationHighlightColor ?? ConfigurationManager.Config.DestinationHighlightColor;
        public Vector4 TabHighlightColor => FilterConfiguration.TabHighlightColor ?? ConfigurationManager.Config.TabHighlightColor;

        public bool ShouldHighlight
        {
            get
            {
                var activeFilter = FilterConfiguration;
                RenderTableBase? activeTable = FilterTable;
                bool shouldHighlight = false;
                if (PluginService.WindowService.HasFilterWindowOpen)
                {
                    if (activeTable != null)
                    {
                        //Allow table to override highlight mode on filter
                        var activeTableHighlightItems = activeTable.HighlightItems;
                        if (activeTableHighlightItems)
                        {
                            shouldHighlight = activeTableHighlightItems;
                            if (activeFilter.HighlightWhen is "When Searching" || activeFilter.HighlightWhen == null && ConfigurationManager.Config.HighlightWhen == "When Searching")
                            {
                                if (!activeTable.IsSearching)
                                {
                                    return false;
                                }
                            }

                            if (activeFilter.FilterItemsInRetainersEnum == FilterItemsRetainerEnum.Only)
                            {
                                if (PluginService.CharacterMonitor.ActiveRetainer == 0 && !PluginService.GameUi.IsWindowVisible(WindowName.RetainerList))
                                {
                                    return false;
                                }
                            }
                        }
                    }
                }
                else
                {
                    shouldHighlight = true;
                    if (activeFilter.FilterItemsInRetainersEnum == FilterItemsRetainerEnum.Only)
                    {
                        if (PluginService.CharacterMonitor.ActiveRetainer == 0 && !PluginService.GameUi.IsWindowVisible(WindowName.RetainerList))
                        {
                            shouldHighlight = false;
                        }
                    }
                    
                }

                return shouldHighlight;
            }
        }

        public bool ShouldHighlightDestination => ShouldHighlight && FilterConfiguration.HighlightDestination != null && FilterConfiguration.HighlightDestination.Value || FilterConfiguration.HighlightDestination == null && ConfigurationManager.Config.HighlightDestination;
        public bool ShouldHighlightDestinationEmpty => ShouldHighlight && FilterConfiguration.HighlightDestinationEmpty != null && FilterConfiguration.HighlightDestinationEmpty.Value || FilterConfiguration.HighlightDestinationEmpty == null && ConfigurationManager.Config.HighlightDestinationEmpty;

        public Vector4? GetTabHighlight(Dictionary<Vector2, Vector4?> bagHighlights)
        {
            if (InvertHighlighting)
            {
                if (InvertTabHighlighting)
                {
                    return bagHighlights.All(c => c.Value != null) ? TabHighlightColor : null;
                }
                else
                {
                    return bagHighlights.All(c => c.Value != null) ? null : TabHighlightColor;
                }
            }
            else
            {
                if (InvertTabHighlighting)
                {
                    return bagHighlights.All(c => c.Value == null) ? TabHighlightColor : null;
                }
                else
                {
                    return bagHighlights.Any(c => c.Value != null) ? TabHighlightColor : null;
                }
            }
        }

        public Vector4? GetTabHighlights(List<Dictionary<Vector2, Vector4?>> bagHighlights)
        {
            if (InvertHighlighting)
            {
                if (InvertTabHighlighting)
                {
                    return bagHighlights.All(c => c.All(d => d.Value != null)) ? TabHighlightColor : null;
                }
                else
                {
                    return bagHighlights.Any(c => c.Any(d => d.Value == null)) ? TabHighlightColor : null;
                }
            }
            else
            {
                if (InvertTabHighlighting)
                {
                    return bagHighlights.Any(c => c.Any(d => d.Value != null)) ? null : TabHighlightColor;
                }
                else
                {
                    return bagHighlights.Any(c => c.Any(d => d.Value != null)) ? TabHighlightColor : null;
                }
            }
        }

        public List<Vector4?> GetSelectIconStringItems(FilterResult? resultOverride = null)
        {
            var itemHighlights = new List<Vector4?>();
            if (PluginService.CharacterMonitor.ActiveCharacterId == 0)
            {
                return itemHighlights;
            }
            var filterResult = resultOverride ?? FilterResult;
            if (filterResult != null)
            {
                HashSet<uint> requiredItems;
                if (FilterConfiguration.FilterType == FilterType.CraftFilter)
                {
                    PluginLog.Verbose("Craft filter, getting flattened materials");
                    requiredItems = FilterConfiguration.CraftList.GetFlattenedMaterials().Select(c => c.Item.RowId).Distinct()
                        .ToHashSet();
                }
                else if (filterResult.AllItems.Count != 0)
                {
                    requiredItems = filterResult.AllItems.Select(c => c.RowId).Distinct().ToHashSet();
                }
                else if (filterResult.SortedItems.Count != 0)
                {
                    requiredItems = filterResult.SortedItems.Select(c => c.InventoryItem.ItemId).Distinct().ToHashSet();
                }
                else
                {
                    return itemHighlights;
                }
                

                var target = Service.Targets.Target;
                    if (target != null)
                    {
                        PluginLog.Verbose("Target found for SelectIconString");

                        var npcId = target.DataId;
                        var npc = Service.ExcelCache.ENpcCollection.Get(npcId);
                        if (npc.Base != null)
                        {
                            PluginLog.Verbose("NPC found for SelectIconString");
                            //TODO: Probably need to deal with custom talk and shit
                            for (var index = 0; index < npc.Base.ENpcData.Length; index++)
                            {
                                var talkItem = npc.Base.ENpcData[index];
                                var preHandler = Service.ExcelCache.GetPreHandlerSheet().GetRow(talkItem);
                                if (preHandler != null)
                                {
                                    talkItem = preHandler.Target;
                                }
                                var shop = Service.ExcelCache.ShopCollection.GetShop(talkItem);
                                if (shop != null)
                                {
                                    PluginLog.Verbose("Shop found for SelectIconString");
                                    var shouldHighlight = shop.Items.Any(c => requiredItems.Contains(c.Row));
                                    if (shouldHighlight)
                                    {
                                        PluginLog.Verbose("Found item for shop" + shop.RowId);
                                        itemHighlights.Add(FilterConfiguration.RetainerListColor ?? ConfigurationManager.Config.RetainerListColor);
                                    }
                                    else
                                    {
                                        PluginLog.Verbose("Did not find item for shop" + shop.RowId);
                                        itemHighlights.Add(null);
                                    }
                                }
                            }
                        }
                    }
                    return itemHighlights;
            }

            return itemHighlights;
        }

        public Dictionary<string, Vector4?> GetArmoireHighlights(FilterResult? resultOverride = null)
        {
            var bagHighlights = new Dictionary<string, Vector4?>();
            if (PluginService.CharacterMonitor.ActiveCharacterId == 0)
            {
                return bagHighlights;
            }
            var filterResult = resultOverride ?? FilterResult;
            if (filterResult != null)
            {
                if (filterResult.AllItems.Count != 0)
                {
                    //TODO: Implement highlighting
                    return new Dictionary<string, Vector4?>();
                }
                else
                {
                    var fullInventory =
                        PluginService.InventoryMonitor.GetSpecificInventory(PluginService.CharacterMonitor
                            .ActiveCharacterId, InventoryCategory.Armoire);
                    
                    var filteredItems = filterResult.SortedItems.Where(c => c.SourceBag == InventoryType.Armoire);
                    foreach (var item in filteredItems)
                    {
                        if(item.SourceBag == InventoryType.Armoire && (MatchesFilter(FilterConfiguration, item, InvertHighlighting) || MatchesRetainerFilter(FilterConfiguration, item, InvertHighlighting)))
                        {
                            if (!bagHighlights.ContainsKey(item.InventoryItem.FormattedName))
                            {
                                bagHighlights.Add(item.InventoryItem.FormattedName, BagHighlightColor);
                            }
                        }
                    }
                    
                    if (InvertHighlighting)
                    {
                        var invertedHighlights = new Dictionary<string, Vector4?>();
                        foreach (var item in fullInventory)
                        {
                            if (!bagHighlights.ContainsKey(item.FormattedName))
                            {
                                if (!invertedHighlights.ContainsKey(item.FormattedName))
                                {
                                    invertedHighlights.Add(item.FormattedName, BagHighlightColor);
                                }
                            }
                        }

                        return invertedHighlights;
                    }

                    
                    return bagHighlights;
                }
            }

            return new Dictionary<string, Vector4?>();
        }

        public Dictionary<uint, Vector4?> GetArmoireTabHighlights(CabinetCategory? currentCategory, FilterResult? resultOverride = null)
        {
            var bagHighlights = new Dictionary<uint, Vector4?>();
            if (PluginService.CharacterMonitor.ActiveCharacterId == 0)
            {
                return bagHighlights;
            }
            var filterResult = resultOverride ?? FilterResult;
            if (filterResult != null)
            {
                if (filterResult.AllItems.Count != 0)
                {
                    //TODO: Implement highlighting
                    return new Dictionary<uint, Vector4?>();
                }
                else
                {
                    var filteredItems = filterResult.SortedItems.Where(c => c.SourceBag == InventoryType.Armoire);
                    var cabinetDictionary = Service.ExcelCache.GetCabinetCategorySheet().Where(c => c.Category.Row != 0).ToDictionary(c => c.Category.Row, c => (uint)c.MenuOrder - 1);
                    foreach (var item in filteredItems)
                    {
                        if(item.SourceBag == InventoryType.Armoire && (MatchesFilter(FilterConfiguration, item, InvertHighlighting) || MatchesRetainerFilter(FilterConfiguration, item, InvertHighlighting)))
                        {
                            if (cabinetDictionary.ContainsKey(item.InventoryItem.Item.CabinetCategory) && !bagHighlights.ContainsKey(cabinetDictionary[item.InventoryItem.Item.CabinetCategory]))
                            {
                                bagHighlights.Add(cabinetDictionary[item.InventoryItem.Item.CabinetCategory], TabHighlightColor);
                            }
                        }
                    }

                    if (InvertTabHighlighting)
                    {
                        var invertedHighlights = new Dictionary<uint, Vector4?>();
                        
                        foreach (var cab in cabinetDictionary)
                        {
                            if (!bagHighlights.ContainsKey(cab.Value))
                            {
                                if (!invertedHighlights.ContainsKey(cab.Value))
                                {
                                    invertedHighlights.Add(cab.Value, TabHighlightColor);
                                }
                            }
                            else
                            {
                                if (!invertedHighlights.ContainsKey(cab.Value))
                                {
                                    invertedHighlights.Add(cab.Value, null);
                                }
                            }
                        }

                        return invertedHighlights;

                    }
                   
                    return bagHighlights;
                }
            }

            return new Dictionary<uint, Vector4?>();
        }

        public Dictionary<Vector2, Vector4?> GetGlamourHighlights(AtkInventoryMiragePrismBox.DresserTab dresserTab, int page, bool displayEquippableOnly,uint classJobSelected, FilterResult? resultOverride = null)
        {
            var bagHighlights = new Dictionary<Vector2, Vector4?>();
            if (PluginService.CharacterMonitor.ActiveCharacterId == 0)
            {
                return bagHighlights;
            }
            var filterResult = resultOverride ?? FilterResult;
            if (filterResult != null)
            {
                if (filterResult.AllItems.Count != 0)
                {
                    /*var correctResults = filterResult.Value.AllItems.Where(c =>
                        AtkInventoryMiragePrismBox.EquipSlotCategoryToDresserTab(c.EquipSlotCategory.Value) ==
                        dresserTab).Skip(page * 50).Take(50).ToList();
                    resultOverride = new FilterResult(new List<SortingResult>(), new List<InventoryItem>(),
                        correctResults);
                    //TODO: REDO ME
                    return GetBagHighlights(InventoryType.GlamourChest, resultOverride);*/
                    return new Dictionary<Vector2, Vector4?>();
                }
                else
                {
                    var fullInventory =
                        PluginService.InventoryMonitor.GetSpecificInventory(PluginService.CharacterMonitor
                            .ActiveCharacterId, InventoryCategory.GlamourChest);
                    
                    var filteredItems = fullInventory.Where(c =>
                        AtkInventoryMiragePrismBox.EquipSlotCategoryToDresserTab(c.EquipSlotCategory) ==
                        dresserTab);
                    
                    if (classJobSelected != 0)
                    {
                        filteredItems = filteredItems.Where(c => Service.ExcelCache.IsItemEquippableBy(c.Item.ClassJobCategory.Row, classJobSelected));
                    }

                    if (displayEquippableOnly && PluginService.CharacterMonitor.ActiveCharacter != null)
                    {
                        var race = PluginService.CharacterMonitor.ActiveCharacter.Race;
                        var gender = PluginService.CharacterMonitor.ActiveCharacter.Gender;
                        filteredItems = filteredItems.Where(c => c.Item.CanBeEquippedByRaceGender(race, gender));
                        
                    }

                    var inventoryItems =
                        filteredItems.Skip(page * 50).Take(50).OrderBy(c => c.SortedSlotIndex).ToList();
                    var glamourIndex = 0;
                    foreach (var item in inventoryItems)
                    {
                        item.GlamourIndex = glamourIndex;
                        glamourIndex++;
                    }
                    var correctResults = filterResult.SortedItems.OrderBy(c => c.InventoryItem.Slot).ToList();
                    var x = 0;
                    var y = 0;
                    foreach (var item in correctResults)
                    {
                        if(inventoryItems.Contains(item.InventoryItem) && item.SourceBag == InventoryType.GlamourChest && (MatchesFilter(FilterConfiguration, item, InvertHighlighting) || MatchesRetainerFilter(FilterConfiguration, item, InvertHighlighting)))
                        {
                            var itemBagLocation = item.BagLocation;
                            if (!bagHighlights.ContainsKey(itemBagLocation))
                            {
                                if (!InvertHighlighting && !item.InventoryItem.IsEmpty)
                                {
                                    bagHighlights.Add(itemBagLocation, BagHighlightColor);
                                }
                                else if (InvertHighlighting && item.InventoryItem.IsEmpty)
                                {
                                    bagHighlights.Add(itemBagLocation, BagHighlightColor);
                                }
                                else if(InvertHighlighting)
                                {
                                    bagHighlights.Add(itemBagLocation, null);
                                }
                                else
                                {
                                    bagHighlights.Add(itemBagLocation, null);
                                }
                            }

                            x++;
                            if (x >= 10)
                            {
                                x = 0;
                                y++;
                            }
                        }
                    }
                    for (int x2 = 0; x2 < 10; x2++)
                    {
                        for (int y2 = 0; y2 < 5; y2++)
                        {
                            var position = new Vector2(x2,y2);
                            if(!bagHighlights.ContainsKey(position))
                            {
                                if (InvertHighlighting)
                                {
                                    bagHighlights.Add(position, BagHighlightColor);
                                }
                                else
                                {
                                    bagHighlights.Add(position, null);
                                }
                            }
                        }
                    }
                    return bagHighlights;
                }
            }

            return new Dictionary<Vector2, Vector4?>();
        }
        
        public Dictionary<Vector2, Vector4?> GetBagHighlights(InventoryType bag, FilterResult? resultOverride = null)
        {
            var bagHighlights = new Dictionary<Vector2, Vector4?>();

            var filterResult = resultOverride ?? FilterResult;
            if (filterResult != null)
            {
                if (filterResult.AllItems.Count != 0)
                {
                    var allItems = filterResult.AllItems;
                    var itemHashes = new HashSet<uint>();
                    Dictionary<uint, HashSet<Vector2>> availableItems = new ();
                    
                    //Hack until I work out somewhere else to push this

                    var characterId = PluginService.CharacterMonitor.ActiveCharacterId;

                    if (bag == InventoryType.FreeCompanyBag0 || bag == InventoryType.FreeCompanyBag1 ||
                        bag == InventoryType.FreeCompanyBag2 || bag == InventoryType.FreeCompanyBag3 ||
                        bag == InventoryType.FreeCompanyBag4 || bag == InventoryType.FreeCompanyBag5)
                    {
                        characterId = PluginService.CharacterMonitor.ActiveFreeCompanyId;
                    }
                    
                    
                    foreach (var item in PluginService.InventoryMonitor.GetSpecificInventory(characterId, bag))
                    {
                        if (!availableItems.ContainsKey(item.ItemId))
                        {
                            availableItems[item.ItemId] = new HashSet<Vector2>();
                        }

                        var bagLocation = item.BagLocation(bag);
                        if (!availableItems[item.ItemId].Contains(bagLocation))
                        {
                            availableItems[item.ItemId].Add(bagLocation);
                        }
                    }
                    
                    //Only hash the items we actually need
                    foreach (var item in allItems)
                    {
                        if (availableItems.ContainsKey(item.RowId))
                        {
                            itemHashes.Add(item.RowId);
                        }
                    }

                    
                    foreach (var availableItem in availableItems.AsParallel())
                    {
                        if (itemHashes.Contains(availableItem.Key))
                        {
                            foreach (var position in availableItem.Value)
                            {
                                if (!InvertHighlighting)
                                {
                                    bagHighlights.TryAdd(position, BagHighlightColor);
                                }
                                else if (InvertHighlighting)
                                {
                                    bagHighlights.TryAdd(position, null);
                                }
                            }
                        }
                    }

                    if (bag is InventoryType.Bag0 or InventoryType.Bag1 or InventoryType.Bag2 or InventoryType.Bag3 or InventoryType.RetainerBag0 or InventoryType.RetainerBag1 or InventoryType.RetainerBag2 or InventoryType.RetainerBag3 or InventoryType.RetainerBag4 or InventoryType.SaddleBag0 or InventoryType.SaddleBag1 or InventoryType.PremiumSaddleBag0 or InventoryType.PremiumSaddleBag1)
                    {
                        for (int x = 0; x < 5; x++)
                        {
                            for (int y = 0; y < 7; y++)
                            {
                                var position = new Vector2(x,y);
                                if(!bagHighlights.ContainsKey(position))
                                {
                                    if (InvertHighlighting)
                                    {
                                        bagHighlights.Add(position, BagHighlightColor);
                                    }
                                    else
                                    {
                                        bagHighlights.Add(position, null);
                                    }
                                }
                            }
                        }
                    }

                    if (bag is InventoryType.ArmoryBody or InventoryType.ArmoryEar or InventoryType.ArmoryFeet or InventoryType.ArmoryHand or InventoryType.ArmoryHead or InventoryType.ArmoryLegs or InventoryType.ArmoryMain or InventoryType.ArmoryNeck or InventoryType.ArmoryOff or InventoryType.ArmoryRing  or InventoryType.ArmoryWrist or InventoryType.ArmorySoulCrystal  or InventoryType.FreeCompanyBag0 or InventoryType.FreeCompanyBag1 or InventoryType.FreeCompanyBag2 or InventoryType.FreeCompanyBag3 or InventoryType.FreeCompanyBag4)
                    {
                        for (int x = 0; x < 50; x++)
                        {
                            var position = new Vector2(x,0);
                            if(!bagHighlights.ContainsKey(position))
                            {
                                if (InvertHighlighting)
                                {
                                    bagHighlights.Add(position, BagHighlightColor);
                                }
                                else
                                {
                                    bagHighlights.Add(position, null);
                                }
                            }
                        }
                    }
                    return bagHighlights;
                }

                
                foreach (var item in filterResult.SortedItems)
                {
                    var matchesSource = item.SourceBag == bag && (MatchesFilter(FilterConfiguration, item, InvertHighlighting) || MatchesRetainerFilter(FilterConfiguration, item, InvertHighlighting) || MatchesFreeCompanyFilter(FilterConfiguration, item, InvertHighlighting) || MatchesHousingFilter(FilterConfiguration, item, InvertHighlighting));
                    var matchesDestination = ShouldHighlightDestination && item.DestinationBag == bag && (MatchesFilter(FilterConfiguration, item, InvertHighlighting) || MatchesRetainerFilter(FilterConfiguration, item, InvertHighlighting, true) || MatchesFreeCompanyFilter(FilterConfiguration, item, InvertHighlighting, true) || MatchesHousingFilter(FilterConfiguration, item, InvertHighlighting, true));
                    if(matchesSource)
                    {
                        var itemBagLocation = item.BagLocation;
                        if (!bagHighlights.ContainsKey(itemBagLocation))
                        {
                            if (!InvertHighlighting && !item.InventoryItem.IsEmpty)
                            {
                                bagHighlights.Add(itemBagLocation, BagHighlightColor);
                            }
                            else if (InvertHighlighting && item.InventoryItem.IsEmpty)
                            {
                                bagHighlights.Add(itemBagLocation, BagHighlightColor);
                            }
                            else if(InvertHighlighting)
                            {
                                bagHighlights.Add(itemBagLocation, null);
                            }
                            else
                            {
                                bagHighlights.Add(itemBagLocation, null);
                            }
                        }
                    }

                    if (matchesDestination && item.DestinationSlot != null)
                    {
                        if (!ShouldHighlightDestinationEmpty && item.IsEmptyDestinationSlot == true)
                        {
                            continue;
                        }
                        var itemBagLocation = item.DestinationSlot.Value;
                        if (!bagHighlights.ContainsKey(itemBagLocation))
                        {
                            if (!InvertDestinationHighlighting && !item.InventoryItem.IsEmpty)
                            {
                                bagHighlights.Add(itemBagLocation, BagDestinationHighlightColor);
                            }
                            else if (InvertDestinationHighlighting && item.InventoryItem.IsEmpty)
                            {
                                bagHighlights.Add(itemBagLocation, BagDestinationHighlightColor);
                            }
                            else if(InvertDestinationHighlighting)
                            {
                                bagHighlights.Add(itemBagLocation, null);
                            }
                            else
                            {
                                bagHighlights.Add(itemBagLocation, null);
                            }
                        }
                    }
                }
            }

            if (bag is InventoryType.Bag0 or InventoryType.Bag1 or InventoryType.Bag2 or InventoryType.Bag3 or InventoryType.RetainerBag0 or InventoryType.RetainerBag1 or InventoryType.RetainerBag2 or InventoryType.RetainerBag3 or InventoryType.RetainerBag4 or InventoryType.SaddleBag0 or InventoryType.SaddleBag1 or InventoryType.PremiumSaddleBag0 or InventoryType.PremiumSaddleBag1)
            {
                for (int x = 0; x < 5; x++)
                {
                    for (int y = 0; y < 7; y++)
                    {
                        var position = new Vector2(x,y);
                        if(!bagHighlights.ContainsKey(position))
                        {
                            if (InvertHighlighting)
                            {
                                bagHighlights.Add(position, BagHighlightColor);
                            }
                            else
                            {
                                bagHighlights.Add(position, null);
                            }
                        }
                    }
                }
            }
            
            if (bag is InventoryType.ArmoryBody or InventoryType.ArmoryEar or InventoryType.ArmoryFeet or InventoryType.ArmoryHand or InventoryType.ArmoryHead or InventoryType.ArmoryLegs or InventoryType.ArmoryMain or InventoryType.ArmoryNeck or InventoryType.ArmoryOff or InventoryType.ArmoryRing  or InventoryType.ArmoryWrist or InventoryType.ArmorySoulCrystal or InventoryType.FreeCompanyBag0 or InventoryType.FreeCompanyBag1 or InventoryType.FreeCompanyBag2 or InventoryType.FreeCompanyBag3 or InventoryType.FreeCompanyBag4)
            {
                for (int x = 0; x < 50; x++)
                {
                    var position = new Vector2(x,0);
                    if(!bagHighlights.ContainsKey(position))
                    {
                        if (InvertHighlighting)
                        {
                            bagHighlights.Add(position, BagHighlightColor);
                        }
                        else
                        {
                            bagHighlights.Add(position, null);
                        }
                    }
                }
            }
            return bagHighlights;
        }
        
        private bool MatchesRetainerFilter(FilterConfiguration activeFilter, SortingResult item, bool invertHighlighting = false, bool destinationFilter = false)
        {
            bool matches = (activeFilter.FilterType.HasFlag(FilterType.SearchFilter) || activeFilter.FilterType.HasFlag(FilterType.SortingFilter) || activeFilter.FilterType.HasFlag(FilterType.CraftFilter));
            if (item.SourceRetainerId != PluginService.CharacterMonitor.ActiveRetainer)
            {
                return false;
            }

            if (destinationFilter && item.SourceRetainerId == ActiveRetainerId)
            {
                return false;
            }
            if (matches)
            {
                if (!item.InventoryItem.IsEmpty)
                {
                    return true;
                }
            }

            if (item.InventoryItem.IsEmpty && invertHighlighting)
            {
                return true;
            }

            return false;
        }
        
        private bool MatchesFreeCompanyFilter(FilterConfiguration activeFilter, SortingResult item, bool invertHighlighting = false, bool destinationFilter = false)
        {
            bool matches = (activeFilter.FilterType.HasFlag(FilterType.SearchFilter) || activeFilter.FilterType.HasFlag(FilterType.SortingFilter) || activeFilter.FilterType.HasFlag(FilterType.CraftFilter));
            if (item.SourceRetainerId != PluginService.CharacterMonitor.ActiveFreeCompanyId)
            {
                return false;
            }

            if (destinationFilter && item.SourceRetainerId == ActiveFreeCompanyId)
            {
                return false;
            }
            if (matches)
            {
                if (!item.InventoryItem.IsEmpty)
                {
                    return true;
                }
            }

            if (item.InventoryItem.IsEmpty && invertHighlighting)
            {
                return true;
            }

            return false;
        }
        
        private bool MatchesHousingFilter(FilterConfiguration activeFilter, SortingResult item, bool invertHighlighting = false, bool destinationFilter = false)
        {
            bool matches = (activeFilter.FilterType.HasFlag(FilterType.SearchFilter) || activeFilter.FilterType.HasFlag(FilterType.SortingFilter) || activeFilter.FilterType.HasFlag(FilterType.CraftFilter));
            if (item.SourceRetainerId != PluginService.CharacterMonitor.ActiveHouseId)
            {
                return false;
            }

            if (destinationFilter && item.SourceRetainerId == ActiveHousingId)
            {
                return false;
            }
            if (matches)
            {
                if (!item.InventoryItem.IsEmpty)
                {
                    return true;
                }
            }

            if (item.InventoryItem.IsEmpty && invertHighlighting)
            {
                return true;
            }

            return false;
        }
        
        private bool MatchesFilter(FilterConfiguration activeFilter, SortingResult item, bool invertHighlighting = false)
        {
            bool matches = false;
            if (activeFilter.FilterType == FilterType.SearchFilter &&
                item.SourceRetainerId == PluginService.CharacterMonitor.ActiveCharacterId)
            {
                matches = true;
            }
            else if (activeFilter.FilterType == FilterType.GameItemFilter)
            {
                matches = true;
            }
            
            if (item.SourceRetainerId == PluginService.CharacterMonitor.ActiveCharacterId && (ActiveRetainerId == null ||
                ActiveRetainerId != null &&
                item.DestinationRetainerId ==
                ActiveRetainerId))
            {
                matches = true;
            }



            if (matches)
            {
                if (!item.InventoryItem.IsEmpty)
                {
                    return true;
                }
            }

            if (item.InventoryItem.IsEmpty && invertHighlighting)
            {
                return false;
            }

            return false;
        }

        public bool HasFilterResult
        {
            get
            {
                return FilterConfiguration.FilterResult != null;
            }
        }

        public FilterResult? FilterResult
        {
            get
            {
                var activeFilter = FilterConfiguration;
                RenderTableBase? activeTable = FilterTable;
                FilterResult? filteredList = activeFilter.FilterResult;
                
                if (activeTable != null)
                {
                    filteredList = new FilterResult(activeTable.SortedItems, new List<InventoryItem>(), activeTable.Items, activeTable.InventoryChanges);
                }
                else if (activeFilter.FilterResult != null)
                {
                    filteredList = activeFilter.FilterResult;
                }

                return filteredList;
            }
        }
    }
}