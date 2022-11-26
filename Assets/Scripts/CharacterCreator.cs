using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class CharacterCreator : MonoBehaviour
{
    [SerializeField]
    GameObject[] shapes;

    [SerializeField]
    Material[] colors;

    [SerializeField] TextMeshProUGUI playerName;

    [SerializeField]
    Button shapeButtonLeft;
    [SerializeField]
    Button shapeButtonRight;
    [SerializeField]
    Button colorButtonLeft;
    [SerializeField]
    Button colorButtonRight;


    [SerializeField]
    Button readyButton;

    GameObject currentObject;
    int colorIndex = 0;
    int shapeIndex = 0;


    private void Start()
    {
        currentObject = Instantiate(shapes[0]);
        currentObject.GetComponent<MeshRenderer>().material = colors[colorIndex];

        readyButton.onClick.AddListener(ReadyToStart);

        colorButtonLeft.onClick.AddListener(delegate { ChangeColor(-1); });
        colorButtonRight.onClick.AddListener(delegate { ChangeColor(1); });

        shapeButtonLeft.onClick.AddListener(delegate { ChangeShape(-1); });
        shapeButtonRight.onClick.AddListener(delegate { ChangeShape(1); });
    }
    void ReadyToStart()
    {
        //if two different players are ready, start game
        Debug.Log("Clicked ready");
        //save data from char create to database
        Debug.Log("playername: "+playerName.text);
        CheckColorAndShape();
    }

    void CheckColorAndShape()
    {
        switch (shapeIndex){
            case 0: 
                Debug.Log("Capsule");
                break;
            case 1:
                Debug.Log("Cube");
                break;
            case 2:
                Debug.Log("Sphere");
                break;
            default:
                Debug.Log("Shape Error");
                break;
        }

        switch (colorIndex)
        {
            case 0:
                Debug.Log("Blue");
                break;
            case 1:
                Debug.Log("Red");
                break;
            case 2:
                Debug.Log("Yellow");
                break;
            default:
                Debug.Log("Color Error");
                break;
        }
    }
    void ChangeShape(int direction)
    {
        //right button, going to positive direction
        if (direction == 1)
        {
            shapeIndex += 1;
            if (shapeIndex > shapes.Length - 1)
            {
                shapeIndex = 0;
            }
        }
        //left button, going to negative direction
        else if (direction == -1)
        {
            shapeIndex -= 1;
            if (shapeIndex < 0)
            {
                shapeIndex = colors.Length - 1;
            }
        }
        Destroy(currentObject);
        currentObject = currentObject = Instantiate(shapes[shapeIndex]);
        currentObject.GetComponent<MeshRenderer>().material = colors[colorIndex];
    }

    void ChangeColor(int direction)
    {
        //right button, going to positive direction
        if (direction == 1)
        {
            colorIndex += 1;
            if (colorIndex > colors.Length-1)
            {
                colorIndex=0;
            }
        }
        //left button, going to negative direction
        else if (direction == -1)
        {
            colorIndex -= 1;
            if (colorIndex < 0)
            {
                colorIndex = colors.Length - 1;
            }
        }
        currentObject.GetComponent<MeshRenderer>().material = colors[colorIndex];
    }

    public string GetPlayerNickname()
    {
        return playerName.text;
    }
}
