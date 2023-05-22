using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WaitCooldown : MonoBehaviour
{
    public int baseCD;
    public int abilityCD;
    public int remainingCD;

    private int turnOnCD;
    private GameManager gm;
    private Image cdImage;
    private TextMeshProUGUI cdCounter;
    // Start is called before the first frame update
    void Start()
    {
        baseCD = abilityCD;
        remainingCD = 0;
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        cdImage = transform.GetComponentInChildren<Image>();
        cdCounter = transform.GetComponentInChildren<TextMeshProUGUI>();
        cdImage.fillAmount = (float)remainingCD / (float)abilityCD;
    }

    // Update is called once per frame
    void Update()
    {
        if(remainingCD != 0)
        {
            remainingCD = abilityCD - (gm.turn - turnOnCD);
            cdCounter.SetText(remainingCD.ToString());
        } else if(cdCounter.text != "")
        {
            cdCounter.SetText("");
        }
        cdImage.fillAmount = (float)remainingCD / (float)abilityCD;
    }

    public void PutOnCooldown()
    {
        remainingCD = abilityCD;
        turnOnCD = gm.turn + 1;
    }
}
