using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonScript : MonoBehaviour
{
    private Vector3 baseScale;
    private float animSpeed = .5f;
    const float scaleAdd = .2f;

    private Collider _collider;

    private void Start()
    {
        _collider = GetComponent<Collider>();

        baseScale = transform.localScale;
        transform.localScale = Vector3.zero;
        transform.DOComplete();
        transform.DOScale(baseScale, animSpeed);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        AnimateEnter();
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        AnimateExit();
    }

    private void AnimateEnter()
    {
        print("mouse enter");
        transform.DOComplete();
        transform.DOScale(new Vector3(baseScale.x+scaleAdd, baseScale.y+scaleAdd, baseScale.z),animSpeed);
    }

    private void AnimateExit()
    {
        transform.DOComplete();
        transform.DOScale(new Vector3(baseScale.x, baseScale.y, baseScale.z),animSpeed);
    }
}
