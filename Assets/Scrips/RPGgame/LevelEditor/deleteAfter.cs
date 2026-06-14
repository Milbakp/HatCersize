using UnityEngine;
using System;

public class deleteAfter : MonoBehaviour
{
    public GameObject activeGameObject;
    private LevelEditorManager levelEditorManager;
    public int actionCounter = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        levelEditorManager = FindAnyObjectByType<LevelEditorManager>();
        levelEditorManager.savingAction += delete;
        levelEditorManager.undoingAction += minusCounterOnUndo;
    }

    // Update is called once per frame
    void Update()
    {
        if (activeGameObject.activeSelf)
        {
            Destroy(this.gameObject);
        }
    }
    void delete()
    {
        actionCounter++;
        if(actionCounter > 15)
        {
            Destroy(activeGameObject);
            Destroy(this.gameObject);
        }
    }

    void minusCounterOnUndo()
    {
        actionCounter--;
    }

}
