using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public GameObject QuestPanel;
    // Start is called before the first frame update
  

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !QuestPanel.activeSelf )
        {
            QuestPanel.SetActive(true);
        }
    }
   private void OnTriggerExit2D(Collider2D collision) 
   { 
        if (collision.gameObject.CompareTag("Player") && QuestPanel.activeSelf )
        {
            QuestPanel.SetActive(false);
        }
    }
}
