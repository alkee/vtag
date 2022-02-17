using UnityEngine;
using UnityEngine.UIElements;

public class SampleScene
    : MonoBehaviour
{
    private VisualElement ui;

    void Start()
    {
        ui = FindObjectOfType<UIDocument>().rootVisualElement;

        ui.Q<Button>("Test").RegisterCallback<ClickEvent>(ev => Test());
    }

    private void Test()
    {
        Debug.Log($"clicked !!");
    }
}
