using UnityEngine;

public class Module : MonoBehavior 
{
    private float brightness;
    private float state;
    void Start()
    {
        SetActivateModule(false);
        SetBrightness(50.0);
        SetColor();
    }

    public void SetActivateModule(bool state)
    {
        //Insert from Integrator file % Inspector
        state = state;
    }

    public bool GetActivateModule()
    {
        return state;
    }
    public void SetBrightness(float brightness)
    {
        //Insert from Integrator file & Inspector
        brightness = brightness;
    }

    public float GetBrightness()
    {
        return brightness;
    }

    public void SetColor()
    {

    }

    public void GetColor()
    {
        //Needs Data 
        return ;
    }

    //Implemented In Future
    public void SetPattern()
    {
        
    }

    public Texture2D GetPattern()
    {
        return  ;
    } 
}
