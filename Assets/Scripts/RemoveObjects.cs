using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RemoveObjects : MonoBehaviour
{
    private Button button;
    private ProgrammManager ProgrammManagerScript;

    // Start is called before the first frame update
    void Start()
    {
        ProgrammManagerScript = FindObjectOfType<ProgrammManager>();

        button = GetComponent<Button>();
        button.onClick.AddListener(Remove_Objects);

    }


    void Remove_Objects()
    {
        ProgrammManagerScript.Remove_Obj();
    }
}
