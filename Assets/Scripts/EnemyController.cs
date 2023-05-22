using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyController : MonoBehaviour
{
    public int lifePoints = 10;

    private TextMeshProUGUI prompt;
    private GameManager gm;
    private GameObject selectIcon;
    private Animator anim;
    private AnimatorStateInfo state;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        prompt = GameObject.Find("DialogText").GetComponent<TextMeshProUGUI>();
        selectIcon = new GameObject();
        selectIcon.transform.SetParent(gameObject.transform);
        selectIcon.transform.position = transform.position;
        selectIcon.AddComponent<SpriteRenderer>();
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        gm.roomClear = false;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public IEnumerator WaitForDeathAnimation()
    {
        Debug.Log("Started coroutine");
        anim.SetTrigger("Death");
        yield return new WaitUntil(()=> anim.GetCurrentAnimatorStateInfo(0).IsName("FadeOut") &&
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9);
        Debug.Log("waited");
        Destroy(gameObject);
    }
    

    public void OnMouseDown()
    {
        Debug.Log("click");
        Attack attScript = GameObject.Find("Attack").GetComponent<Attack>();
        int enemyQuant = attScript.enemQuant;
        if (gm.selectingEnem)
        {
            if (gm.enemiesSelected.Contains(gameObject))
            {
                Debug.Log("Enemy deselected");
                
                gm.enemiesSelected.Remove(gameObject);
                SpriteRenderer sprite = selectIcon.GetComponent<SpriteRenderer>();
                if (sprite != null) sprite.sprite = null;
                if(gm.enemiesSelected.Count <= 0)
                {
                    attScript.confOrCancel.SetActive(false);
                }
            }
            else if(gm.enemiesSelected.Count < enemyQuant)
            {
                
                Debug.Log("Enemy selected");
                SpriteRenderer sprite = selectIcon.GetComponent<SpriteRenderer>();
                sprite.sprite = gm.spriteForSelection;
                sprite.sortingLayerName = "UI";
                gm.enemiesSelected.Add(gameObject);
                attScript.confOrCancel.SetActive(true);
            }
            
        }
    }
}
