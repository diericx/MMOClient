using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class GameUI_Controller : MonoBehaviour {
    public GameObject upgradeGUI;

    public GameObject scrapsTxt;
    public GameObject dmgTxt;
    public GameObject currentHealthImg;
    public GameObject healthValueText;
    public GameObject currentShieldImg;

    public GameObject currentEnergyObj;
    public GameObject speedValueText;
    public GameObject shieldValueText;
    public GameObject energyRegenText;
    public GameObject shieldRegenText;
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
        dmgTxt.GetComponent<Text>().text = dmg.ToString();
    }

    public void updateCurrentHealth(float health, float healthCap)
    {
        ////update text
        healthValueText.GetComponent<Text>().text = health+"%";
        ////update bars
        float percent = ((float)health / (float)healthCap) * 100;
        int rounded = Convert.ToInt32(percent);
        currentHealthImg.GetComponent<Image>().fillAmount = (float)rounded / 100f;
        //currentHealthImg.GetComponent<RectTransform>().sizeDelta = new Vector2(rounded, 100);
    }

    public void updateCurrentEnergy(float energy, float energyCap)
    {
        //update text
        //energyValueText.GetComponent<Text>().text = energy + "/" + energyCap;
        //update bars
        float percent = ((float)energy / (float)energyCap) * 100;
        int rounded = Convert.ToInt32(percent);
        currentEnergyObj.GetComponent<ProgressBar.ProgressBarBehaviour>().Value = rounded;
    }

    public void updateCurrentShield(float shield, float shieldCap)
    {
        //get percent data
        float percent = ((float)shield / (float)shieldCap) * 100;
        int rounded = Convert.ToInt32(percent);

        //update text
        shieldValueText.GetComponent<Text>().text = rounded + "%";
        //update bars

        currentShieldImg.GetComponent<Image>().fillAmount = (float)rounded / 100f;
        //currentShieldImg.GetComponent<RectTransform>().sizeDelta = new Vector2(rounded, 100);
    }

    public void updateShieldRegen(float regen)
    {
        shieldRegenText.GetComponent<Text>().text = regen.ToString();
    }

    public void updateEnergyRegen(float regen)
    {
        energyRegenText.GetComponent<Text>().text = regen.ToString();
    }

    public void updateCurrentSpeed(float speed)
    {
        //update text
        speedValueText.GetComponent<Text>().text = (speed*10).ToString();
        //update bars
        //currentSpeedImg.GetComponent<RectTransform>().sizeDelta = new Vector2(speed*10, 100);
    }



    public void updateShieldCap(float shieldCap)
    {
        //shieldCapImg.GetComponent<RectTransform>().sizeDelta = new Vector2(shieldCap, 100);
    }
}
