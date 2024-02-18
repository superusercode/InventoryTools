using Dalamud.Interface.Colors;
using ImGuiNET;
using Dalamud.Interface.Utility.Raii;
using InventoryTools.Logic.Filters.RuleSystem;

namespace InventoryTools.Logic.Filters.Abstract
{
    public abstract class BooleanFilter : Filter<bool?>
    {
        public override bool? DefaultValue { get; set; } = null;

        public override bool HasValueSet(FilterConfiguration configuration)
        {
            return CurrentValue(configuration) != DefaultValue;
        }

        public override bool HasValueSet(FilterRule configuration)
        {
            return CurrentValue(configuration) != DefaultValue;
        }

        public override bool? CurrentValue(FilterConfiguration configuration)
        {
            return configuration.GetBooleanFilter(Key);
        }

        public override bool? CurrentValue(FilterRule configuration)
        {
            return configuration.BooleanValue;
        }

        public string CurrentSelection(FilterConfiguration configuration)
        {
            var currentValue = CurrentValue(configuration);
            if (currentValue == null)
            {
                return "N/A";
            }
            
            if (currentValue == true)
            {
                return "Yes";
            }

            return "No";
        }

        public string CurrentSelection(FilterRule filterRule)
        {
            var currentValue = CurrentValue(filterRule);
            if (currentValue == null)
            {
                return "N/A";
            }
            
            if (currentValue == true)
            {
                return "Yes";
            }

            return "No";
        }

        public bool? ConvertSelection(string selection)
        {
            if (selection == "N/A")
            {
                return null;
            }

            if (selection == "Yes")
            {
                return true;
            }

            return false;
        }

        private readonly string[] Choices = new []{"N/A", "Yes", "No"};

        public virtual string[] GetChoices()
        {
            return Choices;
        }

        public override void Draw(FilterConfiguration configuration)
        {
            var currentValue = CurrentSelection(configuration);
            
            ImGui.SetNextItemWidth(LabelSize);
            if (HasValueSet(configuration))
            {
                ImGui.PushStyleColor(ImGuiCol.Text,ImGuiColors.HealerGreen);
                ImGui.LabelText("##" + Key + "Label", Name + ":");
                ImGui.PopStyleColor();
            }
            else
            {
                ImGui.LabelText("##" + Key + "Label", Name + ":");
            }
            ImGui.SameLine();
            ImGui.SetNextItemWidth(InputSize);
            using (var combo = ImRaii.Combo("##"+Key+"Combo", currentValue))
            {
                if (combo.Success)
                {
                    foreach (var item in GetChoices())
                    {
                        if (ImGui.Selectable(item, currentValue == item))
                        {
                            UpdateFilterConfiguration(configuration, ConvertSelection(item));
                        }
                    }
                }
            }
            ImGui.SameLine();
            UiHelpers.HelpMarker(HelpText);            
            if (HasValueSet(configuration) && ShowReset)
            {
                ImGui.SameLine();
                if (ImGui.Button("Reset##" + Key + "Reset"))
                {
                    ResetFilter(configuration);
                }
            }
        }

        public override void Draw(FilterRule filterRule)
        {
            var currentValue = CurrentSelection(filterRule);
            
            ImGui.SetNextItemWidth(LabelSize);
            if (HasValueSet(filterRule))
            {
                ImGui.PushStyleColor(ImGuiCol.Text,ImGuiColors.HealerGreen);
                ImGui.LabelText("##" + Key + "Label", Name + ":");
                ImGui.PopStyleColor();
            }
            else
            {
                ImGui.LabelText("##" + Key + "Label", Name + ":");
            }
            ImGui.SameLine();
            ImGui.SetNextItemWidth(InputSize);
            using (var combo = ImRaii.Combo("##"+Key+"Combo", currentValue))
            {
                if (combo.Success)
                {
                    foreach (var item in GetChoices())
                    {
                        if (ImGui.Selectable(item, currentValue == item))
                        {
                            UpdateFilterConfiguration(filterRule, ConvertSelection(item));
                        }
                    }
                }
            }
            ImGui.SameLine();
            UiHelpers.HelpMarker(HelpText);            
            if (HasValueSet(filterRule) && ShowReset)
            {
                ImGui.SameLine();
                if (ImGui.Button("Reset##" + Key + "Reset"))
                {
                    ResetFilter(filterRule);
                }
            }
        }

        public override void UpdateFilterConfiguration(FilterConfiguration configuration, bool? newValue)
        {
            if (newValue.HasValue)
            {
                configuration.UpdateBooleanFilter(Key, newValue.Value);
            }
            else
            {
                configuration.RemoveBooleanFilter(Key);
            }
        }

        public override void UpdateFilterConfiguration(FilterRule filterRule, bool? newValue)
        {
            if (newValue.HasValue)
            {
                filterRule.BooleanValue = newValue.Value;
            }
            else
            {
                filterRule.BooleanValue = null;
            }
        }

        public override void ResetFilter(FilterConfiguration configuration)
        {
            UpdateFilterConfiguration(configuration, DefaultValue);
        }

        public override void ResetFilter(FilterRule filterRule)
        {
            UpdateFilterConfiguration(filterRule, DefaultValue);
        }

    }
}