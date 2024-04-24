using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Pool;

public class Evolution_Control : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    #region 오브젝트 폴링
    public IObjectPool<GameObject> Pool { get; set; }

    void Return_pool() //회수하기
    {
        Pool.Release(this.gameObject);
    }
    #endregion

    #region 카드 오브젝트 변수
    //카드 정보 관련 오브젝트들
    [SerializeField] SpriteRenderer sprite_card;
    [SerializeField] SpriteRenderer sprite_outline;
    [SerializeField] SpriteRenderer sprite_outline_select;
    [SerializeField] SpriteRenderer sprite_outline_defualt;

    public TextMeshPro tmp_cost;
    [SerializeField] TextMeshPro tmp_form;
    [SerializeField] TextMeshPro tmp_name;

    [HideInInspector] public Animator animator;
    #endregion

    public Have_Digimon have_digimon;


    void Start()
    {
        sprite_card.material = Instantiate(sprite_card.material);
        sprite_outline.material = Instantiate(sprite_outline.material);
        sprite_outline_select.material = Instantiate(sprite_outline_select.material);
        sprite_outline_defualt.material = Instantiate(sprite_outline_defualt.material);
    }

    #region 카드 정보 새로고침 (SetUp)
    public void SetUp(Have_Digimon digimon)
    {
        have_digimon = digimon;

        Digimon_Data data = Digimon_Data_Spread.Inst.digimon_Datas[digimon.id - 1];
        tmp_form.text = data.form;
        tmp_name.text = data.name;

        tmp_cost.text = data.cost.ToString();
        if (Battle_Data.Inst.player_mana >= data.cost)
            tmp_cost.color = Color.white;
        else
            tmp_cost.color = Color.red;
        

        sprite_card.sprite = data.card;
    }
    #endregion
    #region SetOrder
    public void Set_Order()
    {
        SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();
        TextMeshPro[] tmp = GetComponentsInChildren<TextMeshPro>();
        TextMeshProUGUI[] tmp_ugui = GetComponentsInChildren<TextMeshProUGUI>();

        for (int i = 0; i < sprites.Length; i++)
            sprites[i].sortingOrder += 10;

        for (int i = 0; i < tmp.Length; i++)
            tmp[i].sortingOrder += 10;

        for (int i = 0; i < tmp_ugui.Length; i++)
            tmp_ugui[i].geometrySortingOrder += 10;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //return;

        if(name != "Copy_Card")
            sprite_outline_select.color = new Color(255 / 255f, 255 / 255f, 255 / 255f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //return;

        if (name != "Copy_Card")
            sprite_outline_select.color = new Color(255 / 255f, 255 / 255f, 255 / 255f, 0f);
    }

    #endregion
    #region Dissolve Effect
    public void Dissolve_off()
    {
        StopAllCoroutines();
        StartCoroutine(Coroutine_Dissolve_off());
    }

    public IEnumerator Coroutine_Dissolve_off()
    {
        float n = 1;

        sprite_card.material.SetFloat("_DissolveAmount", 0);
        sprite_outline_select.color = new Color(1, 1, 1, 0);
        tmp_form.text = "";
        tmp_name.text = "";
        tmp_cost.text = "";
        while (n > 0)
        {
            n -= Time.deltaTime * 4f;

            sprite_outline.material.SetFloat("_DissolveAmount", n);
            sprite_outline_select.material.SetFloat("_DissolveAmount", n);
            sprite_outline_defualt.material.SetFloat("_DissolveAmount", n);

            

            yield return new WaitForSeconds(0.01f);
        }

        Battle_Evolution.Inst.Select_Card_Set(null); //현재 선택한 카드 제거하기
        
        Destroy(gameObject);
    }
    #endregion
}
