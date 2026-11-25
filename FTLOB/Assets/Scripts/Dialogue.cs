using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;

public class Dialogue : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI textComponent;
    [SerializeField] public GameObject text;
    public string[] lines;
    public float textSpeed;
    [SerializeField] private InputActionReference continueDialogue;

    private int index;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        textComponent.text = string.Empty;
        StartDialogue();
    }

    void OnEnable()
    {
        if (continueDialogue != null)
        {
            continueDialogue.action.Enable();
            continueDialogue.action.performed += OnContinueDialogue;
        }
    }

    void OnDisable()
    {
        if (continueDialogue != null)
        {
            continueDialogue.action.performed -= OnContinueDialogue;
            continueDialogue.action.Disable();
        }
    }

    void OnContinueDialogue(InputAction.CallbackContext context)
    {
        if (textComponent.text == lines[index])
        {
            NextLine();
        }
        else
        {
            StopAllCoroutines();
            textComponent.text = lines[index];
        }
    }

    void StartDialogue()
    {
        if (lines == null || lines.Length == 0)
        {
            gameObject.SetActive(false);
            return;
        }

        index = 0;
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        // Type each character one by one
        foreach (char c in lines[index].ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    void NextLine()
    {
        if (index < lines.Length - 1)
        {
            index++;
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            gameObject.SetActive(false);
            text.SetActive(false);
        }
    }
}
