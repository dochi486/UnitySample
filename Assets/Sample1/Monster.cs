using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class Monster : MonoBehaviour
{
    public Animator animator;
    public TextMeshPro hpNumber;
    public Transform hpBar;
    public int maxHp = 3;
    public int currentHp;

    private void Start()
    {
        maxHp = Random.Range(1, maxHp + 1);
        currentHp = maxHp;
        UpdateHPUI();
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other);

        StartCoroutine(OnHit());

        // 화살 파괴
        Destroy(other.gameObject);
    }

    public AudioSource dieSound;
    public float delayDieSound = 0.3f;

    public float dieDelay = 1.0f;
    public float speed = -5f;
    private float delayAttackedMoveTime = 0.3f;

    private IEnumerator OnHit()
    {
        currentHp -= 1;
        // 화면의 HP갱신
        UpdateHPUI();

        bool isAlive = currentHp > 0;
        if (isAlive)
        {
            animator.Play("GetHit");
            //피격 애니메이션 움직이는거 일시 정지.
            float originalSpeed = speed;
            speed = 0;
            yield return new WaitForSeconds(delayAttackedMoveTime);
            speed = originalSpeed;
        }
        else
        {
            StartCoroutine(OnDie());
        }
    }

    private IEnumerator OnDie()
    {
        GameManager.instance.AddScore(100);

        GetComponent<Collider>().enabled = false;
        enabled = false;

        StartCoroutine(PlayDieSound());

        animator.Play("Die");
        yield return new WaitForSeconds(dieDelay);
        Destroy(gameObject);
    }

    private void UpdateHPUI()
    {
        Vector3 scale = hpBar.localScale;
        scale.x = currentHp / maxHp;
        hpBar.localScale = scale;
        hpNumber.text = $"{currentHp}/{maxHp}";
    }

    private IEnumerator PlayDieSound()
    {
        yield return new WaitForSeconds(delayDieSound);
        dieSound.Play();
    }

    private void Update()
    {
        transform.Translate(0, 0, speed * Time.deltaTime);
    }
}