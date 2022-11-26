using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AimIndicator : MonoBehaviour
{
    [SerializeField]
    CharacterController cc;

    public static AimIndicator instance;
    [SerializeField]
    Texture2D aimTexture;

    [SerializeField]
    Texture2D noAmmoTexture;
    [SerializeField]
    Texture2D ammoTexture;



    bool shouldChangeSize = false;
    public float timeWhenStart;
    public float timeWhenEnd;

    public float endSize;
    public float startSize;


    private void Start()
    {
        AimIndicator.instance = this;

        endSize = 25;
        startSize = 100;
    }
    public void StartLerping()
    {
        timeWhenStart = Time.time;
        shouldChangeSize = true;
        GetComponent<Image>().enabled = true;
    }

  
    void Update()
    {
        if (shouldChangeSize)
        {
          
            GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Lerp(startSize, endSize, timeWhenStart, timeWhenEnd));
            GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Lerp(startSize, endSize, timeWhenStart, timeWhenEnd));

            if (GetComponent<RectTransform>().sizeDelta.x==25)
            {
                GetComponent<Image>().enabled = false;
                shouldChangeSize = false;
            }
        }
 
        transform.position = Input.mousePosition;
    }

  public float Lerp(float start, float end, float timeStartedSizeChange, float lerpTime = 1)
    {
        float timeSinceStarted = Time.time - timeStartedSizeChange;

        float percentageComplete = timeSinceStarted / lerpTime;

        var result = Mathf.Lerp(start, end, percentageComplete);

        return result;
    }
    public void OutOfAmmoTexture()
    {
        Cursor.SetCursor(noAmmoTexture, new Vector2(12, 8.5f), CursorMode.Auto);
    }

    public void AmmoAvailableTexture()
    {
        Cursor.SetCursor(ammoTexture, new Vector2(12, 8.5f), CursorMode.Auto);
    }
}


