using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class CutsceneManager : MonoBehaviour
{
    public PlayableDirector director;
    bool isCutscenePause = false;

    void Update()
    {
        if(isCutscenePause && Input.GetKeyDown(KeyCode.Space))
        {
            ResumeCutscene();
        }
    }


    void PauseForDialogue()
    {
        director.Pause();
        isCutscenePause = true;
    }
    void ResumeCutscene()
    {
        isCutscenePause = false;
        director.Play();
        
    }
}
