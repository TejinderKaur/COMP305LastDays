using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : ScriptableObject
{
    public static InventoryController inventoryController;
    public static VillageController villageController;
    public static int Level = 0;
    public static bool settingsGenerated = false;
    public static bool environmentIssues = false;
    public static bool enemyFollowsPlayer = false;
    public static int environmentTemperature = 0;

    public const string Title_01 = "The first day of the Ending!";
    public const string Title_02 = "When everything goes bad...";
    public const string Title_03 = "Is this the last day of the Ending?";

    public const int RegularBodyTemperature = 36;
    public const float DamageRate = .25f;

    public const int enemies01_Level01 = 5;
    public const int enemies01_Level02 = 7;
    public const int enemies01_Level03 = 9;
    
    public const int enemies02_Level01 = 2;
    public const int enemies02_Level02 = 4;
    public const int enemies02_Level03 = 6;
    
    public const int FireBarrels = 5;
    //Player Stats
    public const float HealthRate = .05f;    
    public const float FatigueRate = .05f;
    public const float StarvenessRate = .05f;

    //Village Stats
    public const float StarvenessSeed = .7f;
    public const float SicknessSeed = .6f;
    public const float DeathSeed = .3f;

    public const int NumberVillagePeople = 20;
    public const int NumberVillageFood = 2;
    public const int NumberVillageMedicine = 1;

    public const int TimeToEat = 4;
    public const int TimeToHeal = 3;
    public const int TimeToHeppenSomething = 5;

    //enemy
    public static float enemySpeed = 1f;   
}
