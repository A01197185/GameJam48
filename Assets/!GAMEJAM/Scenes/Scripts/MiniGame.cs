
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

public class MiniGame : UdonSharpBehaviour {

    [SerializeField] GameObject[] fish;
    [SerializeField] int[] points;
    [SerializeField] TextMeshPro pointsText;

    [SerializeField] Animator animator;

    [SerializeField] RectTransform targetRectTransform;

    public Transform skull, target;
    public Slider progression;
    public bool press;
    [SerializeField] private float minSkull = 360, maxSkull = 495f, minTarget = 110, maxTarget = 220;
    private bool state = true, up = true;
    private int timer, count = 0, random;
    public float progress;

    public bool playing;

    public Player localPlayer;

    public void _StartPlaying()
    {
        animator.Play("ShowMiniGame");
        playing = true;
    }

    void Start() {
        minSkull = skull.localPosition.y;
        maxSkull = skull.localPosition.y + 135f;
        //minTarget = skull.localPosition.y + 5f+10;
        //maxTarget = skull.localPosition.y + 130f-10;
        ResetGame();
    }

    private void FixedUpdate() {
        if (!playing) return;

        if(press) {
            if(target.localPosition.y < maxTarget) {
                target.localPosition = new Vector3(target.localPosition.x, target.localPosition.y + 0.7f, target.localPosition.z);
            }
        } else {
            if(target.localPosition.y > minTarget) {
                target.localPosition = new Vector3(target.localPosition.x, target.localPosition.y - 0.7f, target.localPosition.z);
            }
        }
        if(count == 5) {
            if(checkInside()) {
                progress = progress >= 100 ?  100 : progress + 1;
            } else {
                progress = progress <= 0f ? 0 : progress - 0.5f;
            }
            count = 0;

            if(progress == 100)
            {
                _WinGame();
            } else if(progress == 0)
            {
                _LooseGame();
            }

        }
        moveSkull();
        progression.value = progress;
        count++;
    }

    private bool checkInside() {
        return (this.skull.localPosition.y >= this.target.localPosition.y - targetRectTransform.sizeDelta.y/2) && (this.skull.localPosition.y <= this.target.localPosition.y + targetRectTransform.sizeDelta.y / 2);
    }


    public void _WinGame()
    {
        localPlayer.fishingRod.CatchFish();
        playing = false;

        int random = Random.Range(0, fish.Length);

        localPlayer._CatchFish(points[random]);
        fish[random].SetActive(true);
        pointsText.text = $"{points[random]} Points!";
        pointsText.enabled = true;
        
        SendCustomEventDelayedSeconds(nameof(ResetGame), 3f);
    }

    public void _LooseGame()
    {
        localPlayer.fishingRod.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(localPlayer.fishingRod.ReelInLine));
        playing = false;
        ResetGame();
    }

    public void ResetGame()
    {
        pointsText.enabled = false;
        this.press = false;
        this.timer = 0;
        this.progress = 30;
        this.random = 50;
        foreach (GameObject f in fish) f.SetActive(false);
        animator.Play("HideMiniGame");
    }

    private void moveSkull() {
        if(!state) {
            random = Random.Range(1,134);
            state = !state;
        } else {
            if((this.skull.localPosition.y >= this.minSkull + this.random)) {
                skull.localPosition = new Vector3(skull.localPosition.x, skull.localPosition.y - 0.5f, skull.localPosition.z);
                if(this.skull.localPosition.y <= this.minSkull + this.random) {
                    state = !state;
                }
            } else {
                skull.localPosition = new Vector3(skull.localPosition.x, skull.localPosition.y + 0.5f, skull.localPosition.z);
                if(this.skull.localPosition.y >= this.minSkull + this.random) {
                    state = !state;
                }
            }
        }
    }
}
