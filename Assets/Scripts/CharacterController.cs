using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public SpriteRenderer CharacterSpriteRenderer;
    public TextMeshProUGUI CharacterNameTextMesh;
    
    public void CharacterSpawningAnimation()
    {
        gameObject.transform.DOComplete();
        var baseScale = gameObject.transform.localScale;
        const float animationSpeed = .5f;
        gameObject.transform.localScale = Vector3.zero;
        gameObject.transform.DOScale(baseScale, animationSpeed);
    }

    public void CharacterExitAnimation()
    {
        const float animSpeed = .5f;
        transform.DOComplete();
        transform.DOScale(0, animSpeed).OnComplete(DestroyCharacter);
    }

    private void DestroyCharacter()
    {
        Destroy(gameObject);
    }
}
