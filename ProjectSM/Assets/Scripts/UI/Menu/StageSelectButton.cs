using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageSelectButton : MonoBehaviour
{
    [SerializeField] string stageName;

    public void OnClick()
    {
        DataBus.Add("stage", stageName);
        SceneManager.LoadScene("InGame");
    }
}
