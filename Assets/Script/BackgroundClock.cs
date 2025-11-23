using UnityEngine;

public class BackgroundClock : MonoBehaviour
{
    [Header("Referensi Komponen")]
    [SerializeField] private SpriteRenderer clockSpriteRenderer;

    [Header("Sprite Jam (Total 7)")]
    [SerializeField] private Sprite hour_12; 
    [SerializeField] private Sprite hour_01; 
    [SerializeField] private Sprite hour_02; 
    [SerializeField] private Sprite hour_03; 
    [SerializeField] private Sprite hour_04; 
    [SerializeField] private Sprite hour_05; 
    [SerializeField] private Sprite hour_06; 

    private Sprite[] clockSprites;

    void Awake()
    {
        clockSprites = new Sprite[7];
        clockSprites[0] = hour_12;
        clockSprites[1] = hour_01;
        clockSprites[2] = hour_02;
        clockSprites[3] = hour_03;
        clockSprites[4] = hour_04;
        clockSprites[5] = hour_05;
        clockSprites[6] = hour_06;

        if (clockSpriteRenderer == null)
        {
            clockSpriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    public void SetClockHour(int hour)
    {
        if (clockSpriteRenderer == null)
        {
            return;
        }

        if (hour >= 0 && hour < clockSprites.Length)
        {
            if(clockSprites[hour] != null)
            {
                clockSpriteRenderer.sprite = clockSprites[hour];
            }
        }
        else
        {
            if(clockSprites[0] != null)
            {
                clockSpriteRenderer.sprite = clockSprites[0]; 
            }
        }
    }
}