
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class Player : UdonSharpBehaviour
{
    [SerializeField] Highscore highscore;

    [HideInInspector]
    public FishingRod fishingRod;
    public MiniGame miniGame;

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

        if (Owner.isLocal)
        {
            miniGame.localPlayer = this;
        }
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
