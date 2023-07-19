using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Manager
{
    public static GameManager instance;

    public bool onGame;

    public Image pauseGlass;

    [SerializeField] Manager[] managers;
    public override void Init()
    {
        instance = this;
        for (int i = 0; i < managers.Length; i++)
        {
            managers[i].Init();
        }
        onGame = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    private void Update()
    {
        if(onGame)
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                if (!pauseGlass.gameObject.activeSelf)
                {
                    Time.timeScale = 0.0f;
                    pauseGlass.gameObject.SetActive(true);
                }
                else
                {
                    Time.timeScale = 1.0f;
                    pauseGlass.gameObject.SetActive(false);
                }
            }
        }
    }
}
