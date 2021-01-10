using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Random;

/**
    https://learn.unity.com/tutorial/mod-the-cube#5f7cf305edbc2a0020f7d42c
**/

public delegate void TextChanged(string newText);

[RequireComponent(typeof(MeshRenderer))]

public class Cube : MonoBehaviour {
    [SerializeField] public GameOver gameOver;
    [SerializeField] public bool colorsRepeat;
    
    public event TextChanged statusChanged;
    
    private Transform t;
    
    private MeshRenderer meshRenderer;
    private List<Color> colors;
    private Color currentColor, nextColor;
    private static readonly int Color = Shader.PropertyToID("_Color");
    
    private List<(Action method, bool isContinuous)> actions, actualActions, removedActions;
    private Action currentAction, nextAction;
    
    private Vector3 targetPosition;
    private Vector3 targetScale;
    
    private float sphereRadius = 4f;
    
    private float functionsChangeRate = 3f;

    private float movingSpeed = 0.01f;
    private float scalingSpeed = 0.01f;    
    private float rotationSpeed;
    private float minRotationSpeed = 0.01f;
    private float maxRotationSpeed = 0.05f;
    
    private float minScale, maxScale;
    private float minUpScale = 0.1f;
    private float maxUpScale = 0.3f;
    private float minDownScale = -0.1f;
    private float maxDownScale = -0.3f;
    
    private float xAngle, yAngle, zAngle;
    private float minAngle = 0f;
    private float maxAngle = 90f;
    
    private float alpha;
    private float minAlpha = 0.3f;
    private float maxAlpha = 1f;
    
    private bool isContinuousAction, isCalled, scaleDirection = true;



    private void Awake() {
        gameOver.gameObject.SetActive(false);
        t = transform;
        
        colors = new List<Color> {
            new Color32(255, 255, 255, 255),
            new Color32(254, 107, 100, 255),
            new Color32(119, 221, 119, 255),
            new Color32(100, 100, 230, 255),
            new Color32(253, 253, 152, 255)
        };
        currentColor = GetRandomColor();
        meshRenderer = GetComponent<MeshRenderer>();        
        meshRenderer.material.SetColor(Color, currentColor);

        
        removedActions = new List<(Action method, bool isContinous)>();
        actions = new List<(Action, bool)> {
            (ChangePosition, true),
            (ChangeRotation, true),
            (ChangeScale, true),
            (ChangeRotationSpeed, false),
            (ChangeColor, false),
            (ChangeOpacity, false)
        };
        actualActions = new List<(Action method, bool isContinous)>(actions);        
        currentAction = GetRandomAction(actualActions);
    }

    private void Start() {
        InvokeRepeating(nameof(ChangeCurrentAction), 0, functionsChangeRate);
    }
    
    private void Update() {
        foreach (var action in actualActions) {
            if (action.method != currentAction) continue;
            if (action.isContinuous) action.method();
            else if (!isCalled) action.method(); isCalled = true;
        }
    }

    private void ChangeCurrentAction() {
        currentAction = GetRandomAction(actualActions);
        statusChanged?.Invoke(currentAction.Method.Name);
        
        if (currentAction == ChangeScale) scaleDirection = !scaleDirection;
        
        isCalled = false;

        ResetRandomValues();
    }

    private void ResetRandomValues() {
        targetPosition = insideUnitSphere * sphereRadius * movingSpeed;
        rotationSpeed = Range(minRotationSpeed, maxRotationSpeed);
        
        xAngle = Range(minAngle, maxAngle) * rotationSpeed;
        yAngle = Range(minAngle, maxAngle) * rotationSpeed;
        zAngle = Range(minAngle, maxAngle) * rotationSpeed;
        
        if (scaleDirection) {
            minScale = minUpScale;
            maxScale = maxUpScale;
        }
        else {
            minScale = minDownScale;
            maxScale = maxDownScale;
        }

        var s = Range(minScale, maxScale);
        targetScale = new Vector3(s, s, s) * scalingSpeed;

        currentColor = meshRenderer.material.color;
        nextColor = GetRandomColor();
        if (!colorsRepeat) while (currentColor == nextColor) nextColor = GetRandomColor();
        alpha = Range(minAlpha, maxAlpha);
    }

    private void ChangePosition() {
        t.position += targetPosition;
    }    
    
    private void ChangeRotation() {
        t.Rotate(xAngle, yAngle, zAngle);
    }
    
    private void ChangeScale() {
        t.localScale += targetScale;
        if (t.localScale.x <= 0f) EndGame();
    }

    private void ChangeRotationSpeed() {
        rotationSpeed = Range(minRotationSpeed, maxRotationSpeed);
    }

    private void ChangeColor() {
        meshRenderer.material.SetColor(Color, nextColor);
    }

    private void ChangeOpacity() {
        var color = meshRenderer.material.color;
        meshRenderer.material.SetColor(Color, new Color(color.r, color.g, color.b, alpha));
    }

    private void ChangeNothing() { }
    
    
    
    
    public void OnToggleChange(bool isOn, string actionName) {
        if (isOn) {
            AddAction(actionName);
        }
        else {
            DelAction(actionName);
        }
    } 
    
    private void AddAction(string actionName) {
        var action = GetActionWithTypeByName(actionName);
        actualActions.Add(action);
        removedActions.Remove(action);
    }

    private void DelAction(string actionName) { 
        var action = GetActionWithTypeByName(actionName);
        actualActions.Remove(action);
        removedActions.Add(action);
    }

    private (Action, bool) GetActionWithTypeByName(string name) {
        foreach (var action in actions) {
            if (action.method.Method.Name == name) return action;
        }
        return (ChangeNothing, false);
    }
    
    private Action GetRandomAction(List<(Action method, bool isContinuous)> actionsList) {
        return actionsList.Count > 0 ? actionsList[Range(0, actionsList.Count)].method : ChangeNothing;
    }
    
    private Color GetRandomColor() {
        return colors[Range(0, colors.Count)];
    }

    private void EndGame() {
        CancelInvoke();
        gameObject.SetActive(false);
        gameOver.gameObject.SetActive(true);
    }
}