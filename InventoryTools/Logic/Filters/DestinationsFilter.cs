using System;
using System.Collections.Generic;
using CriticalCommonLib.Extensions;
using Dalamud.Interface.Colors;
using ImGuiNET;
using InventoryTools.Logic.Filters.Abstract;

namespace InventoryTools.Logic.Filters
{
    public class DestinationsFilter : DisplayFilter
    {
        public override int Order { get; set; } = 2;
        public override string Key { get; set; } = "Destinations";
        public override string Name { get; set; } = "Destinations";

        public override string HelpText { get; set; } =
            "This lists all the destinations that are applicable given the destinations picked above.";

        public override FilterCategory FilterCategory { get; set; } = FilterCategory.Inventories;
        
        public override bool HasValueSet(FilterConfiguration configuration)
        {
            return false;
        }

        public override FilterType AvailableIn { get; set; } = FilterType.SortingFilter | FilterType.CraftFilter;

        public override void Draw(FilterConfiguration configuration)
        {
            ImGui.NewLine();
            ImGui.Text("Destination Information: ");
            ImGui.SameLine();
            UiHelpers.HelpMarker(HelpText);
            var allCharacters = PluginService.CharacterMonitor.Characters;
            
            //Retainers
            List<string> destinations = new();
            foreach (var retainerCategories in configuration.DestinationRetainerCategories)
            {
                foreach (var retainerCategory in retainerCategories.Value)
                {
                    if (allCharacters.ContainsKey(retainerCategories.Key) &&
                        retainerCategories.Key.ToString().StartsWith("3"))
                    {
                        var formattedName = allCharacters[retainerCategories.Key].FormattedName + " - " +
                                            retainerCategory.FormattedName();
                        destinations.Add(formattedName);
                    }
                }
            }
            ImGui.SetNextItemWidth(LabelSize);
            if (destinations.Count != 0)
            {
                ImGui.PushStyleColor(ImGuiCol.Text,ImGuiColors.HealerGreen);
            }
            ImGui.LabelText("##" + Key + "RetainerLabel", "Retainer Destinations" + ":");
            if (destinations.Count != 0)
            {
                ImGui.PopStyleColor();
            }
            ImGui.SameLine();
            ImGui.TextWrapped(String.Join(", ", destinations));
            
            //Characters
            destinations = new();
            foreach (var characterCategories in configuration.DestinationCharacterCategories)
            {
                foreach (var characterCategory in characterCategories.Value)
                {
                    if (allCharacters.ContainsKey(characterCategories.Key) &&
                        characterCategories.Key.ToString().StartsWith("1"))
                    {
                        var formattedName = allCharacters[characterCategories.Key].FormattedName + " - " +
                                            characterCategory.FormattedName();
                        destinations.Add(formattedName);
                    }
                }
            }
            ImGui.SetNextItemWidth(LabelSize);
            if (destinations.Count != 0)
            {
                ImGui.PushStyleColor(ImGuiCol.Text,ImGuiColors.HealerGreen);
            }
            ImGui.LabelText("##" + Key + "CharacterLabel", "Character Destinations" + ":");
            if (destinations.Count != 0)
            {
                ImGui.PopStyleColor();
            }
            ImGui.SameLine();
            ImGui.TextWrapped(String.Join(", ", destinations));
            
            //Free Companies
            destinations = new();
            foreach (var characterCategories in configuration.DestinationFreeCompanyCategories)
            {
                foreach (var characterCategory in characterCategories.Value)
                {
                    if (allCharacters.ContainsKey(characterCategories.Key) &&
                        characterCategories.Key.ToString().StartsWith("9"))
                    {
                        var formattedName = allCharacters[characterCategories.Key].FormattedName + " - " +
                                            characterCategory.FormattedName();
                        destinations.Add(formattedName);
                    }
                }
            }
            ImGui.SetNextItemWidth(LabelSize);
            if (destinations.Count != 0)
            {
                ImGui.PushStyleColor(ImGuiCol.Text,ImGuiColors.HealerGreen);
            }
            ImGui.LabelText("##" + Key + "CharacterLabel", "Free Company Destinations" + ":");
            if (destinations.Count != 0)
            {
                ImGui.PopStyleColor();
            }
            ImGui.SameLine();
            ImGui.TextWrapped(String.Join(", ", destinations));
            
            //Houses
            destinations = new();
            foreach (var characterCategories in configuration.DestinationHouseCategories)
            {
                foreach (var characterCategory in characterCategories.Value)
                {
                    if (allCharacters.ContainsKey(characterCategories.Key))
                    {
                        var formattedName = allCharacters[characterCategories.Key].FormattedName + " - " +
                                            characterCategory.FormattedName();
                        destinations.Add(formattedName);
                    }
                }
            }
            ImGui.SetNextItemWidth(LabelSize);
            if (destinations.Count != 0)
            {
                ImGui.PushStyleColor(ImGuiCol.Text,ImGuiColors.HealerGreen);
            }
            ImGui.LabelText("##" + Key + "CharacterLabel", "Free Company Destinations" + ":");
            if (destinations.Count != 0)
            {
                ImGui.PopStyleColor();
            }
            ImGui.SameLine();
            ImGui.TextWrapped(String.Join(", ", destinations));
        }
    }
}