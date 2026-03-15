using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    public TextMeshProUGUI mainText;
    public GameObject scanObject;
    public GameObject dialogueBox;
    public bool isAction = false;
    public Image potrait;
    public int talkIdx;

    public void Action()
    {
        ObjectData objData = scanObject.GetComponent<ObjectData>();
        Talk(objData);
        dialogueBox.SetActive(isAction);

    }



    public void Talk(ObjectData objData)
    {
        // 1. 대사 데이터를 먼저 가져옵니다.
        string talkData = DialogueManager.Instance.GetTalk(objData, talkIdx);

        // 2. 대사가 끝났는지(null인지) 먼저 확인합니다.
        if (talkData == null)
        {
            isAction = false;
            talkIdx = 0;
            return;
        }

        // 3. 대사가 존재할 때만 아래 로직을 실행합니다.
        bool isNpc = objData.isNpc;

        if (isNpc)
        {
            // NPC일 때만 초상화 데이터를 가져오고 색상을 조절합니다.
            potrait.sprite = DialogueManager.Instance.GetPotrait(objData, talkIdx);
            potrait.color = new Color(1, 1, 1, 1);
        }
        else
        {
            // NPC가 아니면 초상화를 투명하게 만듭니다.
            potrait.color = new Color(1, 1, 1, 0);
        }

        mainText.text = talkData;
        isAction = true;
        talkIdx++;
    }
}
