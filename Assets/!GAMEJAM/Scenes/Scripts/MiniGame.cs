
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

public class MiniGame : UdonSharpBehaviour {

    public Transform skull, target;
    public Slider progression;
    public bool press;
    private float minSkull = 360, maxSkull = 495f, minTarget = 365f, maxTarget = 485f;
    private bool state = true, up = true;
    private int timer, count = 0, progress, random;

    void Start() {
        this.press = false;
        this.timer = 0;
        this.progress = 30;
        this.random = 50;
    }

    private void FixedUpdate() {
        if(press) {
            if(target.position.y < maxTarget) {
                target.position = new Vector3(target.position.x, target.position.y + 1, target.position.z);
            }
        } else {
            if(target.position.y > minTarget) {
                target.position = new Vector3(target.position.x, target.position.y - 1, target.position.z);
            }
        }
        if(count == 5) {
            if(checkInside()) {
                progress += 1;
            } else {
                progress -= 1;
            }
            count = 0;
        }
        moveSkull();
        progression.value = progress;
        count++;
    }
    private bool checkInside() {
        return (this.skull.position.y >= this.target.position.y - 5) && (this.skull.position.y <= this.target.position.y + 10);
    }

    private void moveSkull() {
        if(!state) {
            random = Random.Range(1,134);
            state = !state;
        } else {
            if((this.skull.position.y >= this.minSkull + this.random)) {
                skull.position = new Vector3(skull.position.x, skull.position.y - 0.5f, skull.position.z);
                if(this.skull.position.y <= this.minSkull + this.random) {
                    state = !state;
                }
            } else {
                skull.position = new Vector3(skull.position.x, skull.position.y + 0.5f, skull.position.z);
                if(this.skull.position.y >= this.minSkull + this.random) {
                    state = !state;
                }
            }
        }
    }
}
