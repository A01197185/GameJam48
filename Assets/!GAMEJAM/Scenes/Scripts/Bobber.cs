
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[RequireComponent(typeof(Rigidbody))]

public class Bobber : UdonSharpBehaviour {

    public float underDrag = 1.5f, underAngularDrag = 2f, airDrag = 0f, airAngularDrag = 0.05f, floatingPower = 70f, waterHeight = 0f;
    public bool catching = false, automatic = false;

    public Rigidbody rb;
    private bool up = true;

    public bool reeledIn = true;

    void Start() {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate() {
        if (reeledIn) return;

        if (automatic) {
            if (catching && up && floatingPower > 30f){
                floatingPower -= 1f;
            }else if (catching){
                floatingPower += 1f;
                up = floatingPower > 90f;
            }else{
                floatingPower = 70f;
            }
        } else {

        }
        if (((transform.position.y - waterHeight) < 0)) {
            rb.AddForceAtPosition(Vector3.up*floatingPower*Mathf.Abs((transform.position.y - waterHeight)), transform.position, ForceMode.Force);
            rb.drag = underDrag;
            rb.angularDrag = underAngularDrag;
        } else {
            rb.drag = airDrag;
            rb.angularDrag = airAngularDrag;
        }
    }
}
