using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class GameUI_Controller : MonoBehaviour {
    public GameObject scrapsTxt;
    public GameObject dmgTxt;
    public GameObject currentHealthImg;
    public GameObject currentEnergyImg;
    public GameObject currentSpeedImg;
    public GameObject currentShieldImg;
    public GameObject shieldCapImg;
	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        //update scraps text object
	}

    public void updateScrapsTxt(int scraps)
    {
        scrapsTxt.GetComponent<Text>().text = "Scraps: " + scraps;
    }

    public void updateDmgTxt(int dmg)
    {
        dmgTxt.GetComponent<Text>().text = "Damage: " + dmg;
    }

    public void updateCurrentHealth(int health, int healthCap)
    {
        float percent = ((float)health / (float)healthCap) * 100;
        int rounded = Convert.ToInt32(percent);
        currentHealthImg.GetComponent<RectTransform>().sizeDelta = new Vector2(rounded, 100);
    }

    public void updateCurrentEnergy(int energy, int energyCap)
    {
        float percent = ((float)energy / (float)energyCap) * 100;
        int rounded = Convert.ToInt32(percent);
        currentEnergyImg.GetComponent<RectTransform>().sizeDelta = new Vector2(rounded, 100);
    }

    public void updateCurrentSpeed(float speed)
    {
        currentSpeedImg.GetComponent<RectTransform>().sizeDelta = new Vector2(speed*10, 100);
    }

    public void updateCurrentShield(int shield, int shieldCap)
    {
        float percent = ((float)shield / (float)shieldCap) * 100;
        int rounded = Convert.ToInt32(percent);
        currentShieldImg.GetComponent<RectTransform>().sizeDelta = new Vector2(rounded, 100);
    }

    public void updateShieldCap(int shieldCap)
    {
        shieldCapImg.GetComponent<RectTransform>().sizeDelta = new Vector2(shieldCap, 100);
    }
}
