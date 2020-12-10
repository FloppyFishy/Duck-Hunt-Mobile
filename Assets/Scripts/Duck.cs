using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Duck : MonoBehaviour
{
    public Sprite[] blackDuckFly1, blackDuckFly2, blackDuckFlyAway, blackDuckDie;

    public Sprite[] blueDuckFly1, blueDuckFly2, blueDuckFlyAway, blueDuckDie;

    public Sprite[] brownDuckFly1, brownDuckFly2, brownDuckFlyAway, brownDuckDie;

    Sprite[] fly1Sprites, fly2Sprites, flyAwaySprites, dieSprites;

    Transform duckSprite;

    Gameplay gameplay;

    public GameObject limitLeft, limitRight;

    public float duckSpeed;
    float duckScale;

    int bounces = 0;
    [HideInInspector]
    public bool inGame, dead;
    bool stun;
    public Vector3 duckDirection;

    void Start()
    {
        duckSprite = transform.GetChild(0);
        duckScale = duckSprite.localScale.x;
        gameplay = transform.parent.GetComponent<Gameplay>();
        InputSystem.EnableDevice(GravitySensor.current);
        InputSystem.EnableDevice(Accelerometer.current);
    }

    private void Update()
    {
        if (Touchscreen.current.press.wasPressedThisFrame && inGame && !dead && !gameplay.needReload)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Touchscreen.current.position.ReadValue());
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
            if (hit)
            {
                if (hit.transform.name == "Duck") DuckHit();
                else if (hit.transform.name == "BackCol") gameplay.MissDuck();
            }
        }
        BlowWind(GravitySensor.current.gravity.ReadValue().x);

        if ((Accelerometer.current.acceleration.ReadValue().x > 2 || Accelerometer.current.acceleration.ReadValue().y + 1 > 2) && inGame && !dead)
            stun = true;
    }

    private void FixedUpdate()
    {
        transform.position += (duckDirection * duckSpeed);
    }

    public void SpawnDuck()
    {
        print("spawn");
        DuckReset();
        StopAllCoroutines();
        duckDirection = Vector3.zero;

        Vector3 spawnPosition = new Vector3(Random.Range(limitLeft.transform.position.x + 1, limitRight.transform.position.x - 1), limitLeft.transform.position.y, limitLeft.transform.position.z);
        transform.position = spawnPosition;

        int randomBird = Random.Range(0, 3);
        switch (randomBird)
        {
            case 0:
                fly1Sprites = blackDuckFly1;
                fly2Sprites = blackDuckFly2;
                flyAwaySprites = blackDuckFlyAway;
                dieSprites = blackDuckDie;
                break;
            case 1:
                fly1Sprites = blueDuckFly1;
                fly2Sprites = blueDuckFly2;
                flyAwaySprites = blueDuckFlyAway;
                dieSprites = blueDuckDie;
                break;
            case 2:
                fly1Sprites = brownDuckFly1;
                fly2Sprites = brownDuckFly2;
                flyAwaySprites = brownDuckFlyAway;
                dieSprites = brownDuckDie;
                break;
            default:
                fly1Sprites = blackDuckFly1;
                fly2Sprites = blackDuckFly2;
                flyAwaySprites = blackDuckFlyAway;
                dieSprites = blackDuckDie;
                break;
        }

        if (Random.Range(0, 2) == 1)duckDirection = new Vector3(Random.Range(0.20f, 1.00f), Random.Range(0.20f, 1.00f), 0);
        else duckDirection = new Vector3(Random.Range(-1.00f, -0.20f), Random.Range(0.20f, 1.00f), 0);

        duckDirection = duckDirection.normalized;

        if (duckDirection.x > 0 && duckDirection.y > 0)
        {
            duckSprite.localScale = new Vector3(duckScale, duckScale, duckScale);
            StartCoroutine(animateBird(fly2Sprites));
        }
        if (duckDirection.x > 0 && duckDirection.y < 0)
        {
            duckSprite.localScale = new Vector3(duckScale, duckScale, duckScale);
            StartCoroutine(animateBird(fly1Sprites));
        }
        if (duckDirection.x < 0 && duckDirection.y < 0)
        {
            duckSprite.localScale = new Vector3(-duckScale, duckScale, duckScale);
            StartCoroutine(animateBird(fly1Sprites));
        }
        if (duckDirection.x < 0 && duckDirection.y > 0)
        {
            duckSprite.localScale = new Vector3(-duckScale, duckScale, duckScale);
            StartCoroutine(animateBird(fly2Sprites));
        }
    }

    public void ChangeDirection(Vector3 dir)
    {
        if (inGame)
        {
            StopAllCoroutines();
            duckDirection = new Vector3(duckDirection.x * dir.x, duckDirection.y * dir.y, 0);

            if (duckDirection.x > 0 && duckDirection.y > 0)
            {
                duckSprite.localScale = new Vector3(duckScale, duckScale, duckScale);
                StartCoroutine(animateBird(fly2Sprites));
            }
            if (duckDirection.x > 0 && duckDirection.y < 0)
            {
                duckSprite.localScale = new Vector3(duckScale, duckScale, duckScale);
                StartCoroutine(animateBird(fly1Sprites));
            }
            if (duckDirection.x < 0 && duckDirection.y < 0)
            {
                duckSprite.localScale = new Vector3(-duckScale, duckScale, duckScale);
                StartCoroutine(animateBird(fly1Sprites));
            }
            if (duckDirection.x < 0 && duckDirection.y > 0)
            {
                duckSprite.localScale = new Vector3(-duckScale, duckScale, duckScale);
                StartCoroutine(animateBird(fly2Sprites));
            }

            bounces++;

            if (bounces >= 4) { inGame = false; DuckFlyAway(); }
        }
    }

    IEnumerator animateBird(Sprite[] animFrames, bool death = false)
    {
        SpriteRenderer duckRenderer = duckSprite.GetComponent<SpriteRenderer>();
        if (!death)
        {
            while (true)
            {
                for (int i = 0; i < animFrames.Length; i++)
                {
                    duckRenderer.sprite = animFrames[i];
                    yield return new WaitForSeconds(0.05f);
                }
                for (int i = animFrames.Length - 1; i >= 0; i--)
                {
                    duckRenderer.sprite = animFrames[i];
                    yield return new WaitForSeconds(0.05f);
                }
                if (stun)
                {
                    Vector3 temp = duckDirection;
                    duckDirection = Vector3.zero;
                    duckRenderer.sprite = dieSprites[0];
                    yield return new WaitForSeconds(2.3f);
                    duckDirection = temp;
                    stun = false;
                }
            }
        }
        else
        {
            print("deadd");
            duckRenderer.sprite = animFrames[0];
            yield return new WaitForSeconds(0.6f);
            duckRenderer.sprite = animFrames[1];
            duckDirection = new Vector3(0, -1, 0);
            while (true)
            {
                duckSprite.localScale = new Vector3(-duckSprite.localScale.x, duckSprite.localScale.y, duckSprite.localScale.z);
                yield return new WaitForSeconds(0.1f);
                if (transform.position.y < limitLeft.transform.position.y)
                {
                    DuckDead();
                    StopAllCoroutines();
                    break;
                }
            }
        }
    }

    public void OnCollisionEnter2D(Collision2D hit)
    {
        print("hit edge");
        if (inGame && !dead)
        {
            if (hit.transform.tag == "HorizontalSide")
                ChangeDirection(new Vector3(-1, 1, 0));
            else if (hit.transform.tag == "VerticalSide")
                ChangeDirection(new Vector3(1, -1, 0));
        }
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        if (!inGame && collision.gameObject.name == "BottomEdge")
        {
            print("now in game");
            inGame = true;
        }      
    }

    public void DuckFlyAway()
    {
        print("flyaway");
        inGame = false;
        duckDirection = new Vector3(0, 1, 0);
        StopAllCoroutines();
        StartCoroutine(animateBird(flyAwaySprites));
        gameplay.FlyAway();
    }

    public void BlowWind(float value)
    {
        if (inGame && !stun)
        {
            Vector3 storeDD = duckDirection;
            if (value > 0.05f)
            {
                storeDD.x += value * 0.2f;
            }
            else if (value < -0.05f)
            {
                storeDD.x += value * 0.2f;
            }
            duckDirection = storeDD;
        }
    }

    public void DuckHit()
    {
        print("hit");
        inGame = false;
        dead = true;
        duckDirection = Vector3.zero;
        StopAllCoroutines();
        StartCoroutine(animateBird(dieSprites, true));
        gameplay.HitDuck(500);
    }

    void DuckDead()
    {
        print("dead");
        duckDirection = Vector3.zero;
        gameplay.DuckDead();
    }

    void DuckReset()
    {
        inGame = false;
        dead = false;
        bounces = 0;
        duckSprite.localScale = new Vector3(duckScale, duckScale, duckScale);
    }
}
