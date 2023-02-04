using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PlantLifebar : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image background;
    [SerializeField] private Image fill;

    public Transform origin;

    private float m_lifetime;
    private bool m_locked;
    public Canvas canvas;

    public void SetLockState(bool _l)
    {
        m_locked = _l;
    }

    private void FixedUpdate()
    {
        UpdatePosition();

        if (m_locked)
            return;

        m_lifetime -= Time.fixedDeltaTime;

        Color tempCol = background.color;
        tempCol.a = m_lifetime;
        background.color = tempCol;

        tempCol = fill.color;
        tempCol.a = m_lifetime;
        fill.color = tempCol;

        if (m_lifetime <= 0)
            Destroy(gameObject);
    }

    private void UpdatePosition()
    {
        RectTransform CanvasRect = canvas.GetComponent<RectTransform>();

        Vector2 ViewportPosition = LifeCanvas.instance.playerController.Cam.WorldToViewportPoint(origin.position);
        Vector2 WorldObject_ScreenPosition = new Vector2(
        ((ViewportPosition.x * CanvasRect.sizeDelta.x) - (CanvasRect.sizeDelta.x * 0.5f)),
        ((ViewportPosition.y * CanvasRect.sizeDelta.y) - (CanvasRect.sizeDelta.y * 0.5f)));

        GetComponent<RectTransform>().anchoredPosition = WorldObject_ScreenPosition;
    }

    public void SetFillAmount(float _v)
    {
        fill.fillAmount = _v;
    }
}
