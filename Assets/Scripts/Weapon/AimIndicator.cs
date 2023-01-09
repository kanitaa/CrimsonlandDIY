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
    public CharacterController cc;

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
    bool isPlayerMoving;

    private void Start()
    {
        endSize = 10;  //default sizes for aim indicator
        startSize = 60;
        AmmoAvailableTexture();
    }
    public void ShowAim(bool moving)
    {
        isPlayerMoving = moving;
        timeWhenStart = Time.time;
        shouldChangeSize = true;
        GetComponent<Image>().enabled = true;
    }

  
    void Update()
    {
        if (shouldChangeSize)
        {
            //resize aimindicator based on whether player is moving or not
            if (isPlayerMoving)
            {
                startSize = 80;
            }
            else
            {
                startSize = 35;
            }

            //change indicator to new size with lerp function
            GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Lerp(startSize, endSize, timeWhenStart, timeWhenEnd));
            GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Lerp(startSize, endSize, timeWhenStart, timeWhenEnd));

            //if size is min size, lerp function has ended
            if (GetComponent<RectTransform>().sizeDelta.x==25)
            {
                GetComponent<Image>().enabled = false;
                shouldChangeSize = false;
            }
        }
 
        transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y-3.5f);
       
    }

    //lerp aim indicators size between two values
  public float Lerp(float start, float end, float timeStartedSizeChange, float lerpTime = 1)
    {
        float timeSinceStarted = Time.time - timeStartedSizeChange;

        float percentageComplete = timeSinceStarted / lerpTime;

        var result = Mathf.Lerp(start, end, percentageComplete);

        return result;
    }

    //change cursor texture depending if theres ammo or no
    public void OutOfAmmoTexture()
    {
        Cursor.SetCursor(noAmmoTexture, new Vector2(7, 4.5f), CursorMode.ForceSoftware);

    }

    public void AmmoAvailableTexture()
    {
        Cursor.SetCursor(ammoTexture, new Vector2(5,4), CursorMode.ForceSoftware);
    }
}


