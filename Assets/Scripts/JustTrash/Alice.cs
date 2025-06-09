using UnityEngine;

public class Alice : MonoBehaviour {
    private void Update() {
        Debug.Log(GetComponent<ItemUI>().ForceOffInteractable);
    }
}
