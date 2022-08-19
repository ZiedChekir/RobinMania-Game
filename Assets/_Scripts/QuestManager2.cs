using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager2 : MonoBehaviour
{

    public static Dictionary<int,string> Quests;
    public static bool questAccepted = false;
    public static bool questCompleted = false;
    public static int EnemiesKilled = 0;

     public GameObject QuestPanel;
     public static bool questAccepted1 = false;
   
    public GameObject QuestPanel1;

    public void AcceptQuest(){
        questAccepted = true;
        QuestPanel.SetActive(false);
        ToastManager.Instance.AddToast("New Quest Added !");

    }

     public void AcceptQuest1(){
        questAccepted1 = true;
        QuestPanel1.SetActive(false);
        ToastManager.Instance.AddToast("New Quest Added !");
    }


    public static void CheckCompleted(){
        if(EnemiesKilled >= 5 && questAccepted && !questCompleted ){
            questCompleted = true;
            ToastManager.Instance.AddToast("Quest Completed!");
        }
    }
}
