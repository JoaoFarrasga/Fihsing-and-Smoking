using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] public int _Points;
    [SerializeField] public int _Fish;
    [SerializeField] public TMP_Text pointCounter;
    [SerializeField] public TMP_Text fishCounter;

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
    }
}
