using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UISpriteAnimator : MonoBehaviour
{
    public Image targetImage;            
    public Sprite[] frames;              
    public float frameRate = 15f;        
    public bool loop = true;             
    private int currentFrame;
    private float timer;

    void Start()
    {
        if (targetImage == null)
            targetImage = GetComponent<Image>();
    }

    void Update()
    {
        if (frames == null || frames.Length == 0) return;

        timer += Time.deltaTime;

        if (timer >= 1f / frameRate)
        {
            timer -= 1f / frameRate;
            currentFrame++;

            if (currentFrame >= frames.Length)
            {
                if (loop)
                    currentFrame = 0;
                else
                    currentFrame = frames.Length - 1;
            }

            targetImage.sprite = frames[currentFrame];
        }
    }
}
