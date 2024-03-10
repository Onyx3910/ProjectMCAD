using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(IVolitile))]
public class Health : MonoBehaviour
{
    public int maxHitPoints = 3;

    public int CurrentHitPoints { get; private set; }
    public bool Invulnerable { get; private set; }
    public SpriteRenderer SpriteRenderer { get; private set; }
    public IVolitile VolitileComponent { get; private set; }

    public event Action OnHit;
    public event Action OnHeal;

    void Start()
    {
        CurrentHitPoints = maxHitPoints;
        SpriteRenderer = GetComponent<SpriteRenderer>();
        VolitileComponent = GetComponent<IVolitile>();
    }

    public void Hit()
    {
        if (Invulnerable) return;

        // Subtrack one hit point
        CurrentHitPoints--;

        // Check if dead
        if (CurrentHitPoints <= 0)
        {
            StartDying();
        }

        // Flash the sprite
        Flash();

        // Fire the OnHit event
        OnHit?.Invoke();
    }

    public void Heal()
    {
        // Add one hit point
        CurrentHitPoints++;

        // Check if over max
        if (CurrentHitPoints > maxHitPoints)
        {
            CurrentHitPoints = maxHitPoints;
        }

        // Fire the OnHeal event
        OnHeal?.Invoke();
    }

    public void Flash()
    {
        if(!Invulnerable) StartCoroutine(FlashSprite());
    }

    private void StartDying()
    {
        VolitileComponent.Die();
    }

    private IEnumerator FlashSprite()
    {
        // set invulnerable
        Invulnerable = true;

        // loop 5 times
        for (var iteration = 0; iteration < 5; iteration++)
        {
            // set the sprite to transparent
            SpriteRenderer.color = new Color(1f, 1f, 1f, 0f);

            // wait for 0.1 seconds
            yield return new WaitForSeconds(0.1f);

            // set the sprite to opaque
            SpriteRenderer.color = new Color(1f, 1f, 1f, 1f);

            // wait for 0.1 seconds
            yield return new WaitForSeconds(0.1f);
        }

        // set invulnerable
        Invulnerable = false;
    }
}
