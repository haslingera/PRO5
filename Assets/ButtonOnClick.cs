using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonOnClick : MonoBehaviour, IPointerClickHandler
{

    public bool red;
    public bool cyan;
    public bool yellow;
    Button button_red;
    Button button_cyan;
    Button button_yellow;

    // Use this for initialization
    void Start () {

        button_red = GameObject.Find("RED").GetComponent<Button>();
        button_cyan = GameObject.Find("CYAN").GetComponent<Button>();
        button_yellow = GameObject.Find("YELLOW").GetComponent<Button>();

    }
	
	// Update is called once per frame
	void Update () {

        
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        var temp = eventData.pointerPress;
        
        if(temp.name == "YELLOW")
        {
            yellow = true;
            cyan = false;
            red = false;
            button_yellow.image.color = Color.yellow;
            button_cyan.image.color = Color.white;
            button_red.image.color = Color.white;
        }

        else if (temp.name == "RED")
        {
            yellow = false;
            cyan = false;
            red = true;
            button_yellow.image.color = Color.white;
            button_cyan.image.color = Color.white;
            button_red.image.color = Color.red;
        }

        else if (temp.name == "CYAN")
        {
            yellow = false;
            cyan = true;
            red = false;
            button_yellow.image.color = Color.white;
            button_cyan.image.color = Color.cyan;
            button_red.image.color = Color.white;
        }

    }

   public Color getColor()
    {
        if(button_cyan.image.color == Color.cyan)
        {
            return Color.cyan;
        }
        else if(button_red.image.color == Color.red)
        {
            return Color.red;
        }
        else if (button_yellow.image.color == Color.yellow)
        {
            return Color.yellow;
        }
        return Color.white;
    }

}
