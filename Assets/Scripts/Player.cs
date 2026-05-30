using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Player : MonoBehaviour
{
    [Header("Weapons")]
    public GameObject arObject;
    public GameObject pistolObject;
    public GameObject axeObject;

    public GunShoot pistolGun;
    public GunShoot arGun;

    [Header("Health")]
    public int health = 100;

    [Header("Switching")]
    public float switchTime = 0.1f;

    [Header("UI")]
    public Image healthBar;
    public GameObject DeathScreen;
    public TMP_Text bulletText;
    public TMP_Text magText;
    public Image magImage;


    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip pickUpSound;

    [Header("Camera")]
    public MonoBehaviour cameraController;

    private bool isSwitching = false;

    void Start()
    {
        EquipWeapon(arObject);

        if (DeathScreen != null)
            DeathScreen.SetActive(false);

        UpdateHealthBar();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            StartCoroutine(SwitchWeapon(arObject));
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            StartCoroutine(SwitchWeapon(pistolObject));
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            StartCoroutine(SwitchWeapon(axeObject));
        }
    }

    IEnumerator SwitchWeapon(GameObject weaponToEnable)
    {
        if (isSwitching) yield break;
        if (weaponToEnable == null) yield break;
        if (weaponToEnable.activeSelf) yield break;

        isSwitching = true;

        yield return new WaitForSeconds(switchTime);

        EquipWeapon(weaponToEnable);

        isSwitching = false;
    }

    void EquipWeapon(GameObject weaponToEnable)
    {
        if (arObject != null)
            arObject.SetActive(false);

        if (pistolObject != null)
            pistolObject.SetActive(false);

        if (axeObject != null)
            axeObject.SetActive(false);

        if (weaponToEnable != null)
            weaponToEnable.SetActive(true);

        bool usingAxe = weaponToEnable == axeObject;

        if (bulletText != null)
            bulletText.gameObject.SetActive(!usingAxe);

        if (magImage != null)
            magImage.gameObject.SetActive(!usingAxe);

        if (magText != null)
            magText.gameObject.SetActive(!usingAxe);
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health < 0)
            health = 0;

        Debug.Log("Player HP: " + health);

        UpdateHealthBar();

        if (health <= 0)
        {
            Die();
        }
    }

    void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.transform.localScale = new Vector3(health / 100f, 1, 1);
        }
    }

    void Die()
    {
        if (DeathScreen != null)
            DeathScreen.SetActive(true);

        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (cameraController != null)
            cameraController.enabled = false;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Medkit"))
        {
            if (health < 100)
            {
                if (audioSource != null && pickUpSound != null)
                    audioSource.PlayOneShot(pickUpSound);

                health += 25;

                if (health > 100)
                    health = 100;

                UpdateHealthBar();

                Destroy(collision.gameObject);
            }
        }
        else if (collision.gameObject.CompareTag("PistolMag"))
        {
            if (audioSource != null && pickUpSound != null)
                audioSource.PlayOneShot(pickUpSound);

            if (pistolGun != null)
                pistolGun.magazinesLeft++;

            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("ArMag"))
        {
            if (audioSource != null && pickUpSound != null)
                audioSource.PlayOneShot(pickUpSound);

            if (arGun != null)
                arGun.magazinesLeft++;

            Destroy(collision.gameObject);
        }
    }
}