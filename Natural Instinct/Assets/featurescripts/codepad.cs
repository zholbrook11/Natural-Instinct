using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class codepad : MonoBehaviour

{
    [SerializeField] public Text guess;
    [SerializeField] public Animator Door;

    private string Answer = "57904";

    public void Number(int number)
    {
        guess.text += number.ToString();

    }

    public void Execute()
    {

        if (guess.text == Answer)
        {
            guess.text = "Access Granted";
            Door.SetBool("Open", true);
            StartCoroutine("Stopdoor");
        }
        else
        {
            guess.text = "Access Denied";
        }
    }

    IEnumerator StopDoor()
    {
        yield return new WaitForSeconds(0.5f);
        Door.SetBool("open", false);
        Door.enabled = false;
    }

}

