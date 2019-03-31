using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour
{
    public Slider healthBar;
    public Slider fatigueBar;
    public Slider hungryBar;

    public Text foodAmount;
    public Text medicineAmount;

    public float health;
    public float fatigue;
    public float hungry;

    public int food;
    public int medicine;

    public int villageFood;
    public int villageMedicine;

    public int hours;
    public int minutes;


    //Village stats
    public int sick;
    public int healthy;

    public int dead;
    public int starving;

    public Text sickPeople;
    public Text healthyPeople;

    public Text Day;
    public Text Time;
    public Text Temperature;
    public int temperature;
    
    public Text BodyTemperature;
    public int bodyTemperature;

    void Start() {
        food = 0;
        medicine = 0;

        health = 1;
        fatigue = 0;
        hungry = 0;
        temperature = Game.environmentTemperature;
        bodyTemperature = Game.RegularBodyTemperature;

    }

    void Update()
    {
        if (health < 0)
        {
            health = 0;
        }

        if (fatigue < 0)
        {
            fatigue = 0;
        }

        if (hungry < 0)
        {
            hungry = 0;
        }


        healthBar.value = health;
        fatigueBar.value = fatigue;
        hungryBar.value = hungry;

        foodAmount.text = "x " + food;
        medicineAmount.text = "x " + medicine;

        healthyPeople.text = "" + healthy;
        sickPeople.text = "" + sick;

        UpdateStats();
        Time.text = GetTime();
    }

    public void UpdateTime(int hours, int minutes) {
        this.hours = hours;
        this.minutes = minutes;        

        Time.text = GetTime();
        Day.text = GetDays();
    }

    public int UpdateTemperature() {

        float v1 = 0.8f;
        float v2 = 1.0f;
        float v3 = 1.0f;

        if (Game.environmentIssues) {
            v1 = 1.5f;
            v2 = 2.0f;
            v3 = 1.4f;
        }

        float r = Random.Range(0.0f, v1);
        if (Mathf.Round(r) > 0) {
            float v = Random.Range(0.4f, v2);// decrease in 1 or 2 degrees
            temperature = temperature - Mathf.RoundToInt(v);
        } else {
            float v = Random.Range(0.3f, v3);// increase in 0 or 1 degree
            temperature = temperature + Mathf.RoundToInt(v);
        }

        Temperature.text = GetTemperature(temperature);       

        return temperature;
    }

    public int UpdateBodyTemperature() {
        int rate = -10;
        float t = 0f;

        if (temperature < rate) {
            t = temperature/rate;
        }
        bodyTemperature = bodyTemperature - Mathf.RoundToInt(t);
        BodyTemperature.text =  GetTemperature(bodyTemperature);
        return bodyTemperature;
    }

    public void UpdateStats() {
        Temperature.text = GetTemperature(temperature);  
        BodyTemperature.text =  GetTemperature(bodyTemperature);
    }

    public string GetTime() {
        int h = hours%24;
        return string.Format("{0:00}", hours) + ":" + string.Format("{0:00}", minutes);
    }
    public string GetDays() {
        int h = hours%24;
        int days = (hours - h)/24;
        return string.Format("{0} Days", days);
    }

    public string GetTemperature(int temperature) {
        return string.Format("{0} °C", temperature);
    }

    public void printStats () {
        print(string.Format("sick >> {0}", sick));
        print(string.Format("health >> {0}", health));
        print(string.Format("fatigue >> {0}", fatigue));
        print(string.Format("hungry >> {0}", hungry));
        print(string.Format("food >> {0}", food));
        print(string.Format("medicine >> {0}", medicine));
        print(string.Format("villageFood >> {0}", villageFood));
        print(string.Format("villageMedicine >> {0}", villageMedicine));
        print(string.Format("hours >> {0}", hours));
        print(string.Format("minutes >> {0}", minutes));
        print(GetDays());
        print(GetTime());
    }
}
