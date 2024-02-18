namespace InventoryTools.Logic.Filters.RuleSystem;

public class FilterRule
{
    private bool? booleanValue;


    public bool? BooleanValue
    {
        get => booleanValue;
        set => booleanValue = value;
    }
}

/*
 * FilterRuleGroup
  FilterRuleGroups[]
  Filters
  Mode: All/Any/None

FilterRule



FilterRuleGroup


SourceFilterRuleGroup

DestinationFilterRuleGroup


Example:

SourceFilterRuleGroup:
  Filters:
    - AllCharacters
    - World: Sophia
    - CanBeDyed: true
  Mode: All

DestinationFilterRuleGroup:
  Filters:
    - AllRetainers
    - World: Sophia
 

SourceFilterRuleGroup:
  FilterRuleGroups:
    - FilterRuleGroup1
      Mode: All
      Filters:
       - AllCharacters
       - World: Sophia
       - CanBeDyed: true
    - FilterRuleGroup2
      Mode: None
  Mode: All


FilterRule:
  Key:
  

Flow
Collect source items based on filter/etc
Iterate through each item in the source but also provide the full list of source items

 */
