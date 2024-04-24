using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Battle_Evolution : MonoBehaviour
{
    public static Battle_Evolution Inst;
    void Awake() => Inst = this;

    public Card_Control select_card;
    //카드 개체
    [HideInInspector][SerializeField] Transform trans_spawn;
    [HideInInspector][SerializeField] GameObject[] trans_objs;
    [SerializeField] Transform trans_hand; //잠시 오브젝트를 옮겨놓을 장소

    //select_card를 재입력하면서 이펙트 정리하기
    public void Select_Card_Set(Card_Control control)
    {
        select_card = control;
    }



    #region 타겟
    public lineArrows line;
    public bool isLine;

    public void OnBeginDrag_Active_Line(Card_Control card)
    {
        isLine = true;
        line.target = card;
        line.gameObject.SetActive(true);
        
    }

    public void OnEndDrag_Active_Line()
    {
        isLine = false;

        //타겟팅 라인 제거
        line.Disable();
    }

    #endregion
    #region 카드 관련
    [SerializeField] List<Evolution_Control> evolution_Controls;
    //진화 카드 생성하기
    public void Create_Card(Have_Digimon[] digimons)
    {
        Destory_Evolution_Card();

        int n = digimons.Length;
        for (int i = 0; i < trans_objs.Length; i++)
        {
            if (i < n)
                trans_objs[i].SetActive(true);
            else
                trans_objs[i].SetActive(false);
        }

        for (int i = 0; i < digimons.Length; i++)
        {
            var obj =  Evolution_Pool.Inst.Pool.Get().GetComponent<Evolution_Control>();
            evolution_Controls.Add(obj);

            obj.GetComponent<Evolution_Touch>().parent_hands = trans_objs[i].transform;
            obj.GetComponent<Evolution_Touch>().house_hand = trans_hand;

            obj.name = "Hand_Card";
            obj.transform.SetParent(trans_objs[i].transform);

            obj.transform.localRotation = Quaternion.Euler(0, 0, 0);
            obj.transform.localPosition = new Vector3(0, 0, 0);

            obj.SetUp(digimons[i]);
        }
    }

    public void Hand_Child_Search()
    {
        for (int i = 0; i < trans_objs.Length; i++)
        {

            if (trans_objs[i].transform.childCount == 0)
                trans_objs[i].gameObject.SetActive(false);

        }
    }

    void Destory_Evolution_Card()
    {
        for (int i = 0; i < evolution_Controls.Count; i++)
        {
            Destroy(evolution_Controls[i].gameObject);
        }
        evolution_Controls.Clear();
    }
    #endregion
}
