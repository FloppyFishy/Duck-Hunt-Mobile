using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Gameplay : MonoBehaviour
{
    public GameUI gameUI;
    public Dog dog;
    public Duck duck;

    public int shots = 6, ducksShot = 0, wave = 0;
    public bool needReload;

    private void Start()
    {
        dog.StartGame(); 
    }

    public void StartGame()
    {
        gameUI.StartGame();
    }

    public void NextDuck()
    {
        shots = 6;
        gameUI.Reload(true);
        needReload = false;
        if (ducksShot < 10 && wave < 10) duck.SpawnDuck();
        if (wave == 10) Finish();
        else wave++;
    }

    public void FlyAway()
    {
        StartCoroutine(gameUI.FlyAway());
    }

    public void MissDuck()
    {
        StartCoroutine(gameUI.ScreenFlash());
        shots--;
        if (shots == 3 && !needReload) needReload = true;
        else if (shots == 0 && !duck.dead) duck.DuckFlyAway();
        gameUI.ShotBullet();
    }

    public void HitDuck(int score)
    {
        gameUI.ShotBullet();
        StartCoroutine(gameUI.ScreenFlash());
        StartCoroutine(gameUI.ShowScore(score));
        ducksShot++;
        shots--;
        if (shots == 3 && !needReload) needReload = true;
        else if (shots == 0 && !duck.dead) duck.DuckFlyAway();
        gameUI.ShotDuck(wave);
    }

    public void Finish()
    {
        duck.gameObject.SetActive(false);
        gameUI.Win();
    }

    public void DuckDead()
    {
        dog.CollectDuck(duck);
    }
}
