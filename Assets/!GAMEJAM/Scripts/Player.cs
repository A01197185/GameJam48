
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Player : UdonSharpBehaviour
{
    [SerializeField] Highscore highscore;


    [HideInInspector]
    public VRCPlayerApi Owner;

    [HideInInspector]
    public bool isAssigned;

    [UdonSynced, FieldChangeCallback(nameof(Score))]
    private int _score;

    public int Score
    {
        set
        {
            _score = value;
            highscore._UpdateScores();
        }
        get => _score;
    }

    public int GetScore()
    {
        return Score;
    }

    public void _OnOwnerSet()
    {
        isAssigned = true;
        highscore._UpdateScores();
    }

    public void _OnCleanup()
    {
        isAssigned = false;
    }

    public void _CatchFish(int value)
    {
        Score = _score + value;
        RequestSerialization();
    }
}
