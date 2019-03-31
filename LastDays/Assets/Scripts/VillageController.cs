using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillageController : ScriptableObject
{
    public int starvingPeople;
    public int sickPeople;
    public int healthyPeople;
    public int deadPeople;

    public int totalFood;

    public int totalMedicine;

    private float starvenessRate = .7f;
    private float sicknessRate = .6f;
    private float deathRate = .3f;
    public delegate void Action(InventoryController inventory);
    public delegate void Happening(int hours, int minutes);
    public Action GameOver;
    public Happening VillageHappening;

    public VillageController() {
        this.healthyPeople = Game.NumberVillagePeople;
        this.totalFood = Game.NumberVillageFood;
        this.totalMedicine = Game.NumberVillageMedicine;
        this.starvenessRate = Game.StarvenessSeed;
        this.sicknessRate = Game.SicknessSeed;
        this.deathRate = Game.DeathSeed;
        this.starvingPeople = 0;
        this.sickPeople = 0;
        this.deadPeople = 0;

    }

    public void ReachFirstThreshold() {
        //it is possible any person suffer
        int dead = Random.Range(0, Mathf.FloorToInt(deathRate*sickPeople));        
        dead = dead<=sickPeople?dead:sickPeople;
        sickPeople = sickPeople - dead;
        deadPeople = deadPeople + dead;

        int sick = Random.Range(0, Mathf.FloorToInt(sicknessRate*starvingPeople));
        sick = sick<=starvingPeople?sick:starvingPeople;
        starvingPeople = starvingPeople - sick;
        sickPeople = sickPeople + sick;

        int starving = Random.Range(1, Mathf.FloorToInt(starvenessRate*healthyPeople));
        starving = starving<=healthyPeople?starving:healthyPeople;
        healthyPeople = healthyPeople - starving;
        starvingPeople = starvingPeople + starving;

        //Debug.Log("1. D/Sk/S/H: " + deadPeople + ", " + sickPeople + ", " + starvingPeople+ ", " + healthyPeople);
    }

    public void ReachSecondThreshold() {        
        //at least one person will suffer
        int dead = Random.Range(Mathf.FloorToInt(deathRate*sickPeople), Mathf.FloorToInt(sickPeople));
        dead = dead<=sickPeople?dead:sickPeople;
        sickPeople = sickPeople - dead;
        deadPeople = deadPeople + dead;

        int sick = Random.Range(Mathf.FloorToInt(sicknessRate*starvingPeople), Mathf.FloorToInt(starvingPeople));
        sick = sick<=starvingPeople?sick:starvingPeople;
        starvingPeople = starvingPeople - sick;
        sickPeople = sickPeople + sick;

        int starving = Random.Range(Mathf.FloorToInt(0.5f*healthyPeople), Mathf.FloorToInt(starvenessRate*healthyPeople));
        starving = starving<=healthyPeople?starving:healthyPeople;
        healthyPeople = healthyPeople - starving;
        starvingPeople = starvingPeople + starving;

        //Debug.Log("2. D/Sk/S/H: " + deadPeople + ", " + sickPeople + ", " + starvingPeople+ ", " + healthyPeople);
    }

    //beverage considered as medicine
    public bool ReachVillage(InventoryController inventory) {
        int food = inventory.food - 1;
        int medicine = inventory.medicine - 1;
        if (food > 0) {
            inventory.food = 1;
            totalFood += food;
        }

        if (medicine > 0) {
            inventory.medicine = 1;
            totalMedicine += medicine;
        }

        //finishing the day
        int left = inventory.hours%24;
        int hours_backup = (inventory.hours - left);
        for (int time = left; time <= 24; time++)
        {
            VillageHappening.Invoke(time, 0);       
            inventory.UpdateTime(time, 0);
            //Debug.Log(" left : " + time + " : " + inventory.hours);
        }
        inventory.hours = inventory.hours + hours_backup;
        return UpdateVillageStatus(inventory);
    }

    // 2 items of medicine for each person
    public void UseMedicine() {
        int medicineToBeUsed = this.totalMedicine;
        if (medicineToBeUsed > 0 && sickPeople > 0) {

            if(medicineToBeUsed > sickPeople) {
                medicineToBeUsed = sickPeople;
            }
            sickPeople = sickPeople - medicineToBeUsed;
            starvingPeople = starvingPeople + medicineToBeUsed;
            this.totalMedicine = this.totalMedicine - medicineToBeUsed;
            MealTime();
        }
        if (totalMedicine == 0) {
            //increment the rate up to 1
            sicknessRate += sicknessRate < 1f?.1f:0;
            if (sicknessRate >= 1f) {
                deathRate += deathRate < 1f?.1f:0;
            }
        }
        //Debug.Log("Medicine: " + this.totalMedicine);
        //Debug.Log("D/Sk/S/H: " + deadPeople + ", " + sickPeople + ", " + starvingPeople+ ", " + healthyPeople);
    }

    public void MealTime() {

        //First Starving
        int foodToBeConsumed = this.totalFood;
        if (foodToBeConsumed > 0 && starvingPeople > 0) {

            if(foodToBeConsumed > starvingPeople) {
                foodToBeConsumed = starvingPeople;
            }
            starvingPeople = starvingPeople - foodToBeConsumed;
            healthyPeople = healthyPeople  + foodToBeConsumed;
            this.totalFood = this.totalFood - foodToBeConsumed;
        }
        
        if (totalFood == 0) {
            //increment the rate up to 1
            starvenessRate += starvenessRate < 1f?.1f:0;            
        }
        //Debug.Log("Meal: " + this.totalFood);
        //Debug.Log("D/Sk/S/H: " + deadPeople + ", " + sickPeople + ", " + starvingPeople+ ", " + healthyPeople);
    }

    public bool UpdateVillageStatus(InventoryController inventory) {
        //Debug.Log("1" + inventory.GetTime());

        inventory.sick = sickPeople;
        inventory.healthy = healthyPeople + starvingPeople;
        inventory.dead = deadPeople;
        inventory.starving = starvingPeople;
        inventory.villageFood = totalFood;
        inventory.villageMedicine = totalMedicine;

        Game.villageController = this;
        Game.inventoryController = inventory;

        if (sickPeople + starvingPeople + healthyPeople == 0) {
            GameOver.Invoke(inventory);
            return false;
        }
        return true;
        //Debug.Log("Sk/S/H: " + sickPeople + ", " + starvingPeople+ ", " + healthyPeople);
    }

    public void printStats () {

        Debug.Log(string.Format("healthyPeople >> {0} ", this.healthyPeople));
        Debug.Log(string.Format("totalFood >> {0}", this.totalFood));
        Debug.Log(string.Format("totalMedicine >> {0}", this.totalMedicine));
        //Debug.Log(string.Format("starvenessRate >> {0}", this.starvenessRate));
        //Debug.Log(string.Format("sicknessRate >> {0}", this.sicknessRate ));
       // Debug.Log(string.Format("deathRate >> {0}", this.deathRate));
        Debug.Log(string.Format("starvingPeople >> {0}", this.starvingPeople));
        Debug.Log(string.Format("sickPeople >> {0}", this.sickPeople));
        Debug.Log(string.Format("deadPeople >> {0}", this.deadPeople));

        
    }
}
