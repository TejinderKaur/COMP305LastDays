using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SummaryController : MonoBehaviour
{

    public Text Message;
    public Text Time;
    public Text TimeMessage;

    public Text Food;
    public Text Medicine;
    public Text Deads;
    public Text Sicks;
    public Text Starvings;
    public Text Healthies;

    void Start()
    {

        InventoryController iController = Game.inventoryController;
        if (iController.health <= 0) {
            Message.text = "You Died!!";
            TimeMessage.text = "Time Survived:";
        } else if (iController.healthy + iController.starving + iController.sick == 0) {
            Message.text = "You Failed!!";
            TimeMessage.text = "Time Survived:";
        } else {
            Message.text = "You Succeed!!";
            TimeMessage.text = "Time Scavengering:";
        }
        
        Time.text = iController.GetTime();
        Food.text = "" + iController.villageFood;
        Medicine.text = "" + iController.villageMedicine;
        Deads.text = "" + iController.dead;
        Sicks.text = "" + iController.sick;
        Starvings.text = "" + iController.starving;
        Healthies.text = "" + iController.healthy;

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
