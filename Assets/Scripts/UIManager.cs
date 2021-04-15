using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _hpText;
    [SerializeField]
    private TextMeshProUGUI _endScreen;
    private int _hp = 100;
    // Start is called before the first frame update
    void Start()
    {
        _hpText.text = "HP: " + _hp;
        _endScreen.text = "";
    }

    // Update is called once per frame
    public void UpdateHP(int hp)
    {
        _hp = hp;
        _hpText.text = "HP: " + _hp;
        if (_hp <= 0)
        {
            StartCoroutine(GameLost());
        }
    }

    IEnumerator GameLost()
    {
        _endScreen.text = "You Lost!";
        yield return new WaitForSeconds(2);
        _endScreen.text = "Try Again!";
        yield return new WaitForSeconds(2);
        _endScreen.text = "";

    }
}
