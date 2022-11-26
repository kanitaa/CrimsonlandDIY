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
        endSize = 25;  //default sizes for aim indicator
        startSize = 100;
    }
    public void ShowAim()
    {
        timeWhenStart = Time.time;
        shouldChangeSize = true;
        GetComponent<Image>().enabled = true;
    }

  
    void Update()
    {
        if (shouldChangeSize)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                float dist = Vector3.Distance(hit.point, cc.transform.position);

                //resize aimindicator based on distance from player
                if (dist < 10)
                {
                    startSize = dist * 12;
                }
                else if (dist < 15)
                {
                    startSize = dist * 8;
                }
                else if (dist > 20)
                {
                    startSize = dist * 6;
                }

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
 
        transform.position = Input.mousePosition;
       
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
        Cursor.SetCursor(noAmmoTexture, new Vector2(12, 8.5f), CursorMode.Auto);
    }

    public void AmmoAvailableTexture()
    {
        Cursor.SetCursor(ammoTexture, new Vector2(12, 8.5f), CursorMode.Auto);
    }
}


