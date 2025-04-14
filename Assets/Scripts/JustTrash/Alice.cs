using UnityEngine;

public class Alice : MonoBehaviour {
    public Animator animator;

    private string[] stateNames = { "One", "Two", "Three" };

    public void Start() {
        animator.Play("Two");
    }
}