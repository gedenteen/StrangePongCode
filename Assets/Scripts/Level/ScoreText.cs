using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreText : MonoBehaviour
{
    private TextMeshProUGUI text;
    private Animator animator;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        animator = GetComponent<Animator>();
    }

    public void SetScore(int value)
    {
        if (text != null)
            text.text = value.ToString();
    }

    public void Highlight()
    {
        if (animator != null)
            animator.SetTrigger("Highlight");
    }
}
