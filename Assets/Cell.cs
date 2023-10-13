using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Cell : MonoBehaviour
{
    public bool alive;
    public bool willLive;
    public Sprite livingCell;
    //public Sprite deadCell;
    public float generation;
    public float gensDead;
    int redGen = 100;
    int yellowGen = 1000;
    SpriteRenderer spriteRenderer;
    
    
    public void UpdateStatus()
    {
        spriteRenderer ??= GetComponent<SpriteRenderer>();
        if (alive)
        {
            gensDead = 0;
            generation++;
            if (generation <= 20) spriteRenderer.color = new(0f, 1f - (generation * 0.05f), 0f + (generation * 0.05f), 1f);
            else if (generation > redGen && generation < yellowGen) spriteRenderer.color = new(0f + ((generation - redGen) * 0.05f), 0f, 1f - ((generation - redGen) * 0.05f), 1f);
            else if (generation > yellowGen) spriteRenderer.color = new(1f, 0f + ((generation - yellowGen) * 0.05f), 1f - ((generation - yellowGen) * 0.05f), 1f);
        }
        else
        {
            generation = 0;
            gensDead++;
            spriteRenderer.color = new(0f, 1f, 0f, 1 - (gensDead * 0.1f));
        }
    }
}
