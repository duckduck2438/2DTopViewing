using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : Singleton<DialogueManager>
{
    void Awake()
    {


    }


    public string GetTalk(ObjectData objData, int talkIdx)
    {
        if(talkIdx == objData.talks.Length)
            return null;
        else
            return objData.talks[talkIdx];
    }

public Sprite GetPotrait(ObjectData objData, int talkIdx)
{
    // 1. NPC가 아니면 즉시 null 반환 (초상화 필요 없음)
    if (!objData.isNpc)
    {
        return null;
    }

    // 2. talkSequence 배열 자체가 비어있거나, 인덱스가 범위를 벗어났는지 확인
    if (objData.talkSequence == null || talkIdx >= objData.talkSequence.Length)
    {
        // 인덱스가 범위를 벗어나면 마지막 초상화나 기본 초상화를 반환하거나 null 반환
        return null; 
    }

    // 3. 위 조건을 다 통과했을 때만 배열에 접근
    int spriteIdx = objData.talkSequence[talkIdx];
    
    // 4. imgs 배열 범위도 체크해주면 더 완벽합니다.
    if (objData.imgs == null || spriteIdx >= objData.imgs.Length)
    {
        return null;
    }

    return objData.imgs[spriteIdx];
}

}
