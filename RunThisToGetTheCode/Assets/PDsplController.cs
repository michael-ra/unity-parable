using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PDsplController : MonoBehaviour
{

    public Movement mvmnt;
    
    public Text pointsUiText;
    // Start is called before the first frame update
    void Start()
    {
        pointsUiText.text = '0'+" collected";
    }

    void OnDisable()
    {
        PlayerPrefs.SetInt("score", mvmnt.collectables);
    }
    
    // Update is called once per frame
    void Update()
    {
       pointsUiText.text = mvmnt.collectables+ " collected";

       if (mvmnt.collectables == 12)
       {
           SceneManager.LoadScene("YouWin");
       }
    }
}
