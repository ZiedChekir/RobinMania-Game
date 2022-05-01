using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ethereum : MonoBehaviour
{
    public TMPro.TMP_Text text;
    public void printCall(string a)
    {
        text.text = a;
    }
}
