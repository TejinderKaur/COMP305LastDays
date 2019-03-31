using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameController : MonoBehaviour
{
    public PlayerController playerController;
    private VillageController villageController;
    private EnemyController enemyController;

    public EnvironmentController environmentController;
    
    private float time;//time in game

    private int minutes;
    private int hours;

    private bool end;
    public int minutesToIncrement = 1;
    public int Level;
    private float timeRate = .2f;//time to convert in minutes

    public Text title;
    void Start()
    {
        if (Game.Level == 0){
            Game.Level = Level;
        }

        switch (Game.Level){
            case 2 : 
                title.text = Game.Title_02;
                Game.environmentIssues = true;
                Game.environmentTemperature = -9;
                Game.enemyFollowsPlayer = true;
                Game.enemySpeed = 2f;
                break;
            case 3 : 
                Game.environmentIssues = true;
                title.text = Game.Title_03;
                Game.environmentTemperature = -15;
                Game.enemyFollowsPlayer = true;
                Game.enemySpeed = 4f;
                break;
            default : 
                title.text = Game.Title_01;
                Game.environmentTemperature = 0;
                Game.enemyFollowsPlayer = false;
                break;
        }        

        if (!Game.settingsGenerated) {
            villageController = (VillageController)ScriptableObject.CreateInstance("VillageController");
            villageController.VillageHappening = VillageHappening;
            Game.villageController = villageController;
            Game.inventoryController = playerController.inventoryController;
        } else {
            villageController = Game.villageController;
            
            playerController.inventoryController.food = Game.inventoryController.food;
            playerController.inventoryController.medicine = Game.inventoryController.medicine;
            playerController.inventoryController.health = Game.inventoryController.health;
            playerController.inventoryController.fatigue = Game.inventoryController.fatigue;
            playerController.inventoryController.hungry = Game.inventoryController.hungry;
            playerController.inventoryController.hours = Game.inventoryController.hours;
            playerController.inventoryController.minutes = Game.inventoryController.minutes;
            villageController.UpdateVillageStatus(playerController.inventoryController);

            this.hours = Game.inventoryController.hours;
            this.minutes = Game.inventoryController.minutes;
        }
        Game.settingsGenerated = true;
        if (playerController.inventoryController != null) {
            Game.inventoryController = playerController.inventoryController;
            playerController.inventoryController.temperature = Game.environmentTemperature;
            playerController.inventoryController.bodyTemperature = Game.RegularBodyTemperature;
            playerController.inventoryController.UpdateTemperature();
            playerController.inventoryController.UpdateBodyTemperature();
            
        }

        //Game.villageController.printStats();
        //playerController.inventoryController.printStats ();

        playerController.EndLevel = End;
        playerController.GameOver = GameOver;
        villageController.GameOver = GameOver;
    }

    void End(InventoryController inventoryController) {
        end = true;
        if (villageController.ReachVillage(inventoryController)) {
            gameObject.GetComponent<MainMenuController>().GoToLevelComplete();
        }
        //Game.villageController.printStats();
        //playerController.inventoryController.printStats ();
    }

    void GameOver(InventoryController inventoryController) {
        end = true;
        playerController.UpdateTime(this.hours, this.minutes);        

        Game.villageController = villageController;
        Game.inventoryController = inventoryController;
        gameObject.GetComponent<MainMenuController>().GoToLevelComplete();
    }

    // Update is called once per frame
    void Update()
    {
        if (end) return;
        
        bool villageAllive = true;
        time += Time.deltaTime;

        if (time >= timeRate) {
            minutes+=minutesToIncrement;
            time = 0;   
        }

        if (minutes >= 60) {
            minutes = 0;
            hours++;

            villageAllive = villageController.UpdateVillageStatus(playerController.inventoryController);
            if (villageAllive){
                VillageHappening(this.hours, this.minutes);
            }
        }

        if (villageAllive && minutes%15 == 0 && time == 0) {
            playerController.UpdateTime(this.hours, this.minutes);
        }
        if (minutes%50 == 0 && time == 0) {
            playerController.UpdateTemperature();
        }
        
    }

    public void VillageHappening(int hours, int minutes) {
        // village people stats update
        if (hours != 0) {
            if (hours%Game.TimeToHeppenSomething == 0 && minutes == 0) {
                if(hours%2 == 0) {
                    //Debug.Log(" 2 . " + hours + " : " + minutes);
                    villageController.ReachSecondThreshold();
                } else {
                    //Debug.Log(" 1 . " + hours + " : " + minutes);
                    villageController.ReachFirstThreshold();
                }
            }

            //meal time (eight to eight hours)
            if (hours%Game.TimeToEat == 0 && minutes == 0) {
                villageController.MealTime();
            }

            //time to use medicine(four to four hours)
            if (hours%Game.TimeToHeal == 0 && minutes == 0) {
                villageController.UseMedicine();
            }
        }
    }
}   
