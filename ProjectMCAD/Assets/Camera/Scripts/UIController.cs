using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public Health PlayerHealth { get; private set; }
    public Stack<GameObject> HealthStack { get; private set; }

    //public GameObject gameOverPanel;
    //public GameObject gameWinPanel;

    void Start()
    {
        PlayerHealth = GameObject.Find("Player").GetComponent<Health>();
        PlayerHealth.OnHit += RemoveHeart;
        SetupPlayerHearts();

        //gameOverPanel.SetActive(false);
        //gameWinPanel.SetActive(false);
    }

    protected void SetupPlayerHearts()
    {
        HealthStack = new Stack<GameObject>();
        var hitPointSprite = transform.GetChild(0).gameObject;
        HealthStack.Push(hitPointSprite);
        for (var index = 0; index < PlayerHealth.maxHitPoints; index++)
        {
            if (index == 0) continue;
            var newHitPointSprite = Instantiate(hitPointSprite, transform);
            newHitPointSprite.GetComponent<RectTransform>().position += 128 * index * Vector3.right;
            HealthStack.Push(newHitPointSprite);
        }
    }

    protected void RemoveHeart()
    {
        StartCoroutine(FlashHeart());
    }

    protected IEnumerator FlashHeart()
    {
        for (var interation = 0; interation < 5; interation++)
        {
            var heart = HealthStack.Peek();
            heart.SetActive(false);
            yield return new WaitForSeconds(0.1f);
            heart.SetActive(true);
            yield return new WaitForSeconds(0.1f);
        }

        HealthStack.Pop().SetActive(false);
    }

    protected void AddHeart()
    {
        var newHitPointSprite = Instantiate(HealthStack.Peek(), transform);
        newHitPointSprite.GetComponent<RectTransform>().position += 128 * Vector3.right;
        HealthStack.Push(newHitPointSprite);
    }
}
