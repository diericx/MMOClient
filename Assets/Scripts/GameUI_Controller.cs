using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameUI_Controller : MonoBehaviour {
    public GameObject scrapsTxt;
    public GameObject dmgTxt;
    public GameObject currentHealthImg;
    public GameObject healthValueText;
    public GameObject shieldHudObj;
    public GameObject shieldHudTxtObj;
    public GameObject energyHudObj;

    public GameObject healthCapStatTxt;
    public GameObject healthRegenStatTxt;
    public GameObject shieldCapStatTxt;
    public GameObject shieldRegenStatTxt;
    public GameObject energyCapStatTxt;
    public GameObject energyRegenStatTxt;
    public GameObject damageStatTxt;
    public GameObject bulletSpeedStatTxt;
    public GameObject fireRateStatTxt;
    public GameObject moveSpeedStatTxt;

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

    public void updateCurrentHealth(float health, int healthCap)
    {
        ////update text
        healthValueText.GetComponent<Text>().text = health+"%";
        ////update bars
        float healthCapValue = (healthCap * 10f) + 100;
        float percent = ((float)health / healthCapValue) * 100;
        int rounded = Convert.ToInt32(percent);
        currentHealthImg.GetComponent<Image>().fillAmount = (float)rounded / 100f;
        //currentHealthImg.GetComponent<RectTransform>().sizeDelta = new Vector2(rounded, 100);
    }

    public void updateStats(Dictionary<string, object> msg)
    {
        int healthCap = int.Parse(msg["HealthCap"].ToString());
        int healthRegen = int.Parse(msg["HealthRegen"].ToString());
        int shieldCap = int.Parse(msg["ShieldCap"].ToString());
        int shieldRegen = int.Parse(msg["ShieldRegen"].ToString());
        int energyCap = int.Parse(msg["EnergyCap"].ToString());
        int energyRegen = int.Parse(msg["EnergyRegen"].ToString());
        int dmg = int.Parse(msg["Damage"].ToString());
        int fireRate = int.Parse(msg["FireRate"].ToString());
        int speed = int.Parse(msg["Speed"].ToString());

        healthCapStatTxt.GetComponent<Text>().text = healthCap.ToString();
        healthRegenStatTxt.GetComponent<Text>().text = healthRegen.ToString();
        shieldCapStatTxt.GetComponent<Text>().text = shieldCap.ToString();
        shieldRegenStatTxt.GetComponent<Text>().text = shieldRegen.ToString();
        energyCapStatTxt.GetComponent<Text>().text = energyCap.ToString();
        energyRegenStatTxt.GetComponent<Text>().text = energyRegen.ToString();
        damageStatTxt.GetComponent<Text>().text = dmg.ToString();
        fireRateStatTxt.GetComponent<Text>().text = fireRate.ToString();
        moveSpeedStatTxt.GetComponent<Text>().text = speed.ToString();

    }

    public void updateEnergyHUD(float energy, int energyCap)
    {
        //update text
        //energyValueText.GetComponent<Text>().text = energy + "/" + energyCap;
        //update bars
        float energyCapValue = (energyCap * 10f) + 50f;
        float percent = ((float)energy / energyCapValue) * 100;
        int rounded = Convert.ToInt32(percent);
        energyHudObj.GetComponent<ProgressBar.ProgressBarBehaviour>().Value = rounded;
    }

    public void updateShieldHUD(float shield, int shieldCap)
    {
        //get percent data
        float shieldCapValue = (shieldCap * 10f) + 10f;
        float percent = ((float)shield / shieldCapValue) * 100;
        int rounded = Convert.ToInt32(percent);

        //update text
        shieldHudTxtObj.GetComponent<Text>().text = rounded + "%";
        //update bars

        shieldHudObj.GetComponent<Image>().fillAmount = (float)rounded / 100f;
        //currentShieldImg.GetComponent<RectTransform>().sizeDelta = new Vector2(rounded, 100);
    }

    //public void updateShieldRegen(float regen)
    //{
    //    shieldRegenText.GetComponent<Text>().text = regen.ToString();
    //}

    //public void updateEnergyRegen(float regen)
    //{
    //    energyRegenText.GetComponent<Text>().text = regen.ToString();
    //}

    //public void updateCurrentSpeed(float speed)
    //{
    //    //update text
    //    speedValueText.GetComponent<Text>().text = (speed*10).ToString();
    //    //update bars
    //    //currentSpeedImg.GetComponent<RectTransform>().sizeDelta = new Vector2(speed*10, 100);
    //}



    public void updateShieldCap(float shieldCap)
    {
        //shieldCapImg.GetComponent<RectTransform>().sizeDelta = new Vector2(shieldCap, 100);
    }
}
