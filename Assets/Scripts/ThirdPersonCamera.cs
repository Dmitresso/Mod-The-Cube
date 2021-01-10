using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour{
    [SerializeField] private Transform follow;
    [SerializeField] private float distanceAway = 15f;
    
    private Transform t;


    private void Start() {
        t = transform;
    }

    private void LateUpdate() {
        t.position = follow.position - Vector3.forward * distanceAway;
    }
}