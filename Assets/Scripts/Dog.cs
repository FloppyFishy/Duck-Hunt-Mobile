using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dog : MonoBehaviour
{
    public Sprite[] dogWalking;
    public Sprite[] dogJump;
    public Sprite[] OneDuck;
    public Sprite[] TwoDucks;
    public Sprite[] Laughing;

    float animDelay = 0.1f, movementStep = 0.1f;

    SpriteRenderer sRenderer;

    public void StartGame()
    {
        sRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(IntroAnimation());
    }

    public void CollectDuck(Duck duck)
    {
        transform.position = new Vector3(duck.transform.position.x, duck.limitLeft.transform.position.y - 0.3f, duck.limitLeft.transform.position.z);
        StartCoroutine(CollectDuckAnimation(duck));
    }

    IEnumerator CollectDuckAnimation(Duck duck)
    {
        sRenderer.sprite = OneDuck[0];
        yield return new WaitForSeconds(0.2f);
        for (int i = 0; i < 13; i++)
        {
            transform.position += Vector3.up * movementStep;
            yield return new WaitForSeconds(0.05f);
        }
        yield return new WaitForSeconds(0.3f);
        for (int i = 0; i < 13; i++)
        {
            transform.position += Vector3.down * movementStep;
            yield return new WaitForSeconds(0.05f);
        }
        yield return new WaitForSeconds(0.1f);
        transform.parent.GetComponent<Gameplay>().NextDuck();
    }

    IEnumerator IntroAnimation()
    {
        for (int w = 0; w < 3; w++)
        {
            //walk
            for (int c = 0; c < 5; c++)
            {
                for (int i = 0; i < dogWalking.Length - 1; i++)
                {
                    sRenderer.sprite = dogWalking[i];
                    transform.position += Vector3.right * movementStep;
                    yield return new WaitForSeconds(animDelay);
                }
            }
            yield return new WaitForSeconds(animDelay * 2);
            //sniff
            for (int c = 0; c < 3; c++)
            {
                yield return new WaitForSeconds(animDelay);
                sRenderer.sprite = dogWalking[dogWalking.Length - 1];
                yield return new WaitForSeconds(animDelay);
                sRenderer.sprite = dogWalking[dogWalking.Length - 2];
            }
        }

        yield return new WaitForSeconds(animDelay * 4);
        sRenderer.sprite = dogJump[0];
        yield return new WaitForSeconds(animDelay * 7);

        //jump
        transform.position += new Vector3(0.7f, 1.6f, 0);
        sRenderer.sprite = dogJump[1];
        for (int c = 0; c < 22; c++)
        {
            transform.position += new Vector3(1f, 1.6f, 0) * 0.03f;
            yield return new WaitForSeconds(0.01f);
        }
        sRenderer.sortingOrder = -1;
        sRenderer.sprite = dogJump[2];
        for (int c = 0; c < 16; c++)
        {
            transform.position += new Vector3(1f, 0, 0) * 0.05f;
            yield return new WaitForSeconds(0.01f);
        }
        for (int c = 0; c < 30; c++)
        {
            transform.position += new Vector3(1f, -1.6f, 0) * 0.05f;
            yield return new WaitForSeconds(0.01f);
        }
        yield return new WaitForSeconds(0.1f);
        transform.parent.GetComponent<Gameplay>().StartGame();
        transform.parent.Find("Duck").GetComponent<Duck>().SpawnDuck();
        yield return null;
    }
}
