using UnityEngine;
using UnityEngine.UI;

public class ToggleController : MonoBehaviour {
    [SerializeField] public Text status;    
    
    private Toggle[] toggles;
    private Cube cube;

    private void Awake() {
        cube = FindObjectOfType<Cube>();
        toggles = GetComponentsInChildren<Toggle>();


        foreach (var t in toggles) {
            t.onValueChanged.AddListener(delegate { OnToggleValueChanged(t.isOn, t.name.Substring(7)); });
        }

        cube.statusChanged += ApplyNewText;
    }
    

    private void ApplyNewText(string newText) {
        status.text = "Current action: " + newText;
    }
    
    private void OnToggleValueChanged(bool isOn, string actionName) {
        cube.OnToggleChange(isOn, actionName);
    }
}