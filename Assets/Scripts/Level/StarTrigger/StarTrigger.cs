using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarTrigger : MonoBehaviour
{
    public bool IsReady = false;

    [Header("References to my components")]
    [SerializeField] private SpriteRenderer _spriteRenderer;

    private void OnValidate()
    {
        _spriteRenderer ??= GetComponent<SpriteRenderer>();
    }

    public void SetSpriteAndColor(Sprite sprite, Color color)
    {
        if (_spriteRenderer == null)
        {
            Debug.LogWarning($"StarTrigger: SetSpriteAndColor: _spriteRenderer is null");
            return;
        }

        _spriteRenderer.sprite = sprite;
        _spriteRenderer.color = color;
    }
}
