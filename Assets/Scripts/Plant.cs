using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{
    public float cdCoin;
    public bool isSauvage = true;
    public int photocoin;
    [SerializeField] private Transform lifeBarTransform;

    private PlayerController m_playerController;
    private PlantLifebar m_lifebar;

    [SerializeField] protected int pv;
    [SerializeField] protected int maxPv;

    public void TakeDamage(int _damage)
    {
        pv -= _damage;
        m_lifebar.SetFillAmount((float)pv / maxPv);
    }

    private void ShowLifebar()
    {
        if (m_lifebar)
            Destroy(m_lifebar);

        m_lifebar = Instantiate(LifeCanvas.instance.lifeBarPrefab, LifeCanvas.instance.canvas.transform).GetComponent<PlantLifebar>();

        m_lifebar.SetFillAmount((float)pv / maxPv);
        m_lifebar.SetLockState(true);
        m_lifebar.origin = lifeBarTransform;
        m_lifebar.canvas = LifeCanvas.instance.canvas;
    }

    private void FixedUpdate()
    {
        if (Vector3.SqrMagnitude(transform.position - LifeCanvas.instance.playerController.transform.position) <= 25)
        {
            if (m_playerController)
                return;

            PlayerController playerController = LifeCanvas.instance.playerController;

            if (playerController)
            {
                m_playerController = playerController;
                ShowLifebar();
            }

            return;
        }

        if (m_playerController && m_lifebar)
        {
            m_lifebar.SetLockState(false);
            m_playerController = null;
        }
    }
}
