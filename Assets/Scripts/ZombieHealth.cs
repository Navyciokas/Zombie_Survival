using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieHealth : MonoBehaviour
{
    public WaveSpawner waveSpawner;

    [Header("Health")]
    public int health = 100;

    [Header("Follow")]
    public Transform player;
    public float attackDistance = 2f;

    [Header("Attack")]
    public int attackDamage = 10;
    public float attackCooldown = 2f;

    [Header("Sounds")]
    public AudioSource audioSource;

    public AudioClip[] randomZombieSounds;
    public AudioClip walkingSound;
    public AudioClip[] attackSounds;
    public AudioClip[] deathSounds;

    public float minRandomSoundTime = 3f;
    public float maxRandomSoundTime = 8f;

    private Player playerScript;
    private NavMeshAgent agent;
    private Animator animator;

    private bool isAlive = true;
    private float nextAttackTime = 0f;
    private bool isWalkingSoundPlaying = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (player != null)
        {
            playerScript = player.GetComponent<Player>();
        }

        StartCoroutine(RandomZombieSoundLoop());
    }

    void Update()
    {
        if (!isAlive) return;
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > attackDistance)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);

            if (animator != null)
            {
                animator.SetBool("Walk", true);
            }

            PlayWalkingSound();
        }
        else
        {
            agent.isStopped = true;

            if (animator != null)
            {
                animator.SetBool("Walk", false);
            }

            StopWalkingSound();

            if (Time.time >= nextAttackTime)
            {
                Attack();
                nextAttackTime = Time.time + attackCooldown;
            }
        }
    }

    void Attack()
    {
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        if (audioSource != null && attackSounds.Length > 0)
        {
            AudioClip clip = attackSounds[Random.Range(0, attackSounds.Length)];

            if (clip != null)
                audioSource.PlayOneShot(clip);
        }

        if (playerScript != null)
        {
            playerScript.TakeDamage(attackDamage);
        }
    }

    void PlayWalkingSound()
    {
        if (audioSource == null || walkingSound == null) return;
        if (isWalkingSoundPlaying) return;

        audioSource.clip = walkingSound;
        audioSource.loop = true;
        audioSource.Play();

        isWalkingSoundPlaying = true;
    }

    void StopWalkingSound()
    {
        if (audioSource == null) return;
        if (!isWalkingSoundPlaying) return;

        audioSource.Stop();
        audioSource.loop = false;
        audioSource.clip = null;

        isWalkingSoundPlaying = false;
    }

    IEnumerator RandomZombieSoundLoop()
    {
        while (isAlive)
        {
            float waitTime = Random.Range(minRandomSoundTime, maxRandomSoundTime);
            yield return new WaitForSeconds(waitTime);

            if (!isAlive) yield break;

            if (audioSource != null && randomZombieSounds.Length > 0)
            {
                AudioClip clip = randomZombieSounds[Random.Range(0, randomZombieSounds.Length)];

                if (clip != null)
                {
                    audioSource.PlayOneShot(clip);
                }
            }
        }
    }

    public void TakeDamage(int amount)
    {
        if (!isAlive) return;

        health -= amount;

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isAlive = false;

        StopWalkingSound();

        if (waveSpawner != null)
        {
            waveSpawner.ZombieDied();
        }

        if (audioSource != null && deathSounds.Length > 0)
        {
            AudioClip clip = deathSounds[Random.Range(0, deathSounds.Length)];

            if (clip != null)
                audioSource.PlayOneShot(clip);
        }

        if (agent != null)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }

        if (animator != null)
        {
            animator.SetBool("Walk", false);
            animator.SetTrigger("Death");
        }

        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = false;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
            rb.isKinematic = true;

        Destroy(gameObject, 3f);
    }
}