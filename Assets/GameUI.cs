using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public Sprite duckNotHit, duckHit;
    Image screenEffect;
    Image[] ducks, bullets;
    TextMeshProUGUI scoreText, reloadText;
    GameObject flyAway, pointsIndicator, gameStats, winScreen;
    Duck duck;
    Gameplay gamePlay;

    void Start()
    {
        duck = Resources.FindObjectsOfTypeAll<Duck>()[0];
        gamePlay = Resources.FindObjectsOfTypeAll<Gameplay>()[0];
        screenEffect = transform.Find("ScreenEffect").GetComponent<Image>();
        flyAway = transform.Find("FlyAway").gameObject;
        pointsIndicator = transform.Find("PointsIndicator").gameObject;
        gameStats = transform.Find("GameStats").gameObject;
        winScreen = transform.Find("WinScreen").gameObject;
        flyAway.SetActive(false);
        pointsIndicator.SetActive(false);
        gameStats.SetActive(false);
        winScreen.SetActive(false);
        screenEffect.gameObject.SetActive(true);
        screenEffect.CrossFadeAlpha(0, 0f, false);

        scoreText = gameStats.transform.Find("ScoreBox").transform.Find("Score").GetComponent<TextMeshProUGUI>();
        reloadText = gameStats.transform.Find("Reload").transform.Find("ReloadText").GetComponent<TextMeshProUGUI>();
        ducks = gameStats.transform.Find("HitBox").transform.Find("Ducks").GetComponentsInChildren<Image>();
        bullets = gameStats.transform.Find("BulletBox").transform.Find("Bullets").GetComponentsInChildren<Image>();
    }

    public void StartGame()
    {
        gameStats.SetActive(true);
        StartCoroutine(FlashDuck());
        StartCoroutine(FlashReload());
    }

    public void ShotDuck(int wave)
    {
        if (ducks[wave].sprite == duckNotHit)
        {
           ducks[wave].sprite = duckHit;
        }
    }

    public void ShotBullet()
    {
        for (int i = bullets.Length - 1; i >= 0; i--)
        {
            if (bullets[i].color.r == 1)
            {
                bullets[i].color = Color.black;
                break;
            }
        }
    }

    public void Reload(bool nextRound = false)
    {
        print("reload");
        if (gamePlay.shots == 3 && reloadText.text != "RELOADING" && gamePlay.needReload && !duck.dead)
        {
            reloadText.text = "RELOADING";
            StartCoroutine(ReloadTime());
        }
        else if (nextRound)
        {
            for (int i = 0; i < bullets.Length; i++)
            {
                bullets[i].color = Color.white;
            }
        }
    }

    public void Win()
    {
        winScreen.SetActive(true);
    }

    public IEnumerator ReloadTime()
    {
        yield return new WaitForSeconds(1.5f);
        gamePlay.needReload = false;
        for (int i = 0; i < bullets.Length; i++)
        {
            bullets[i].color = Color.white;
        }
        reloadText.text = "RELOAD";
    }

    public IEnumerator FlyAway()
    {
        print("flyaway text");
        flyAway.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        flyAway.SetActive(false);
        yield return new WaitForSeconds(0.7f);
        gamePlay.NextDuck();
    }

    public IEnumerator FlashDuck()
    {
        while (true)
        {
            if (duck.inGame)
            {
                ducks[gamePlay.wave].color = Color.black;
                yield return new WaitForSeconds(0.15f);
                ducks[gamePlay.wave].color = Color.white;
                yield return new WaitForSeconds(0.15f);
            }
            else
                yield return null;
        }
    }

    public IEnumerator FlashReload()
    {
        Color originalColour = reloadText.color;
        while (true)
        {
            if (gamePlay.needReload)
            {
                reloadText.color = Color.red;
                yield return new WaitForSeconds(0.15f);
                reloadText.color = originalColour;
                yield return new WaitForSeconds(0.15f);
            }
            else
                yield return null;
        }
    }

    public IEnumerator ScreenFlash()
    {
        screenEffect.CrossFadeAlpha(1, 0.03f, false);
        yield return new WaitForSeconds(0.1f);
        screenEffect.CrossFadeAlpha(0, 0.03f, false);
    }

    public IEnumerator ShowScore(int score)
    {
        string scoreString = (Convert.ToInt64(scoreText.text) + score).ToString();
        if (scoreString.Length == 3) scoreString = "000" + scoreString; 
        else if (scoreString.Length == 4) scoreString = "00" + scoreString;
        else if (scoreString.Length == 5) scoreString = "0" + scoreString;
        scoreText.text = scoreString;

        pointsIndicator.SetActive(true);
        pointsIndicator.GetComponent<TextMeshProUGUI>().text = score.ToString();
        pointsIndicator.transform.position = duck.transform.position + (Vector3.up * 1.2f);
        yield return new WaitForSeconds(1.16f);
        pointsIndicator.SetActive(false);
    }
}
