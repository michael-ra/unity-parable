using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndPoints : MonoBehaviour
{
        
    public Text pointsUiText;
    private int _points;
    
    void OnEnable()
    {
        _points  =  PlayerPrefs.GetInt("score");
    }
    
    void Start()
    {
        pointsUiText.text = _points+" collected. You need 12.";
    }
}
