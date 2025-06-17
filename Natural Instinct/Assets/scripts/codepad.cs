using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class codepad : MonoBehaviour
{
  [SerializeField] private Text answertext;

    public void Number( int number)
    {
        answertext.text += number.ToString();
    }
       
}

 
    
     

