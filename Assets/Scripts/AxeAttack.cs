using System.Collections;
using UnityEngine;

public class AxeAttack : MonoBehaviour
{
    [Header("Attack")]
    public int damage = 35;
    public float attackCooldown = 0.8f;
    public float hitActiveTime = 0.25f;

    [Header("Animation")]
    public Animator animator;

    [Header("Axe Hitbox")]
    public Collider axeCollider;

    [Header("Effects")]
    public GameObject bloodEffect;

    public AudioSource audioSource;
    public AudioClip hitSound;
    public AudioClip swingSound;

    private bool canAttack = true;
    private bool isAttacking = false;
    private bool hasHit = false;

    void Start()
    {
        if (axeCollider != null)
            axeCollider.enabled = false;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && canAttack)
        {
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack()
    {
        canAttack = false;
        isAttacking = true;
        hasHit = false;

        audioSource.PlayOneShot(swingSound);
        if (animator != null)
            animator.SetTrigger("Attack");

        if (axeCollider != null)
            axeCollider.enabled = true;

        yield return new WaitForSeconds(hitActiveTime);

        if (axeCollider != null)
            axeCollider.enabled = false;

        isAttacking = false;

        yield return new WaitForSeconds(attackCooldown);

        canAttack = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isAttacking) return;
        if (hasHit) return;

        ZombieHealth zombie = other.GetComponentInParent<ZombieHealth>();

        if (zombie != null)
        {
            audioSource.PlayOneShot(hitSound);
            zombie.TakeDamage(damage);
            hasHit = true;

            if (bloodEffect != null)
            {
                Vector3 hitPoint = other.ClosestPoint(transform.position);

                Instantiate(
                    bloodEffect,
                    hitPoint,
                    Quaternion.identity
                );
            }
        }
    }
}