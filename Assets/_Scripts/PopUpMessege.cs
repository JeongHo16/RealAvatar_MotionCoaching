using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUpMessege : MonoBehaviour
{
    public Text popUpMessege;

    private WaitForSeconds messegeTime = new WaitForSeconds(2f);

    void Start()
    {
        MessegePopUp("안녕하세요");
    }

    public void MessegePopUp(string message)
    {
        if (StateUpdater.isPopUpMessege)
        {
            StopCoroutine("alert");
            StateUpdater.isPopUpMessege = false;
        }

        StartCoroutine(alert(message));
    }

    IEnumerator alert(string message)
    {
        StateUpdater.isPopUpMessege = true;
        popUpMessege.text = message;
        popUpMessege.CrossFadeAlpha(1f, 0, true);
        yield return messegeTime;
        popUpMessege.CrossFadeAlpha(0, 1.5f, true);
        yield return messegeTime;
        StateUpdater.isPopUpMessege = false;
    }
}
