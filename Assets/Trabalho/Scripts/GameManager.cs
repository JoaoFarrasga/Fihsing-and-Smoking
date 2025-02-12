using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] public int _Points;
    [SerializeField] public int _Fish;
    [SerializeField] public TMP_Text pointCounter;
    [SerializeField] public TMP_Text pointCounterPC;
    [SerializeField] public TMP_Text fishCounter;
    [SerializeField] public TMP_Text fishCounterPC;

    public static GameManager Instance;

    public void Awake()
    {
        Instance = this;
    }

    public void AddPoints(int points)
    {
        _Points += points;
        _Fish++;
        pointCounter.text = _Points.ToString();
        fishCounter.text = _Fish.ToString();
        pointCounterPC.text = _Points.ToString();
        fishCounterPC.text = _Fish.ToString();
    }
}
