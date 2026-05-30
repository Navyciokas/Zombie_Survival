using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GunShoot : MonoBehaviour
{
    public ParticleSystem muzzleFlash;

    public AudioSource audioSource;
    public AudioClip shootSound;
    public AudioClip reloadSound;
    public AudioClip emptySound;

    public Animator animator;

    public TMP_Text ammoText;
    public TMP_Text magText;

    public bool isAR = false;

    public GameObject bulletPrefab;
    public Transform shootPoint;
    public Camera playerCamera;

    public float bulletSpeed = 50f;
    public float range = 1000f;

    public float spread = 0.02f;

    public float pistolCooldown = 0.4f;
    public float arCooldown = 0.1f;

    public int magazineSize = 30;
    public int bulletsInMagazine = 30;
    public int magazinesLeft = 3;

    public float reloadTime = 1.5f;

    private float nextShootTime = 0f;
    private bool isReloading = false;

    void Update()
    {

        UpdateAmmoUI();


        if (isReloading) return;

        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
            return;
        }

        if (Input.GetMouseButtonDown(0) && Time.time >= nextShootTime)
        {
            if (bulletsInMagazine <= 0)
            {
                audioSource.PlayOneShot(emptySound);
                //Reload();
                return;
            }
        }
        
        if (isAR)
        {
            if (Input.GetMouseButton(0) && Time.time >= nextShootTime && bulletsInMagazine > 0)
            {
                
                Shoot();
                nextShootTime = Time.time + arCooldown;
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0) && Time.time >= nextShootTime && bulletsInMagazine > 0)
            {
                Shoot();
                nextShootTime = Time.time + pistolCooldown;
            }
        }
    }

    void Shoot()
    {
        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }

        audioSource.PlayOneShot(shootSound);


        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);

        if (!state.IsName("Shoot"))
        {
            animator.SetTrigger("Shoot");
        }

        bulletsInMagazine--;

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Vector3 targetPoint;

        if (Physics.Raycast(ray, out RaycastHit hit, range))
            targetPoint = hit.point;
        else
            targetPoint = ray.GetPoint(range);

        Vector3 direction = (targetPoint - shootPoint.position).normalized;

        direction += new Vector3(
            Random.Range(-spread, spread),
            Random.Range(-spread, spread),
            Random.Range(-spread, spread)
        );

        direction.Normalize();

        GameObject bullet = Instantiate(
            bulletPrefab,
            shootPoint.position,
            Quaternion.LookRotation(direction)
        );

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.velocity = direction * bulletSpeed;

        Destroy(bullet, 5f);
    }

    void Reload()
    {
        if (magazinesLeft <= 0) { return; }
        if (bulletsInMagazine == magazineSize) return;
        animator.SetTrigger("Reload");

        StartCoroutine(ReloadCoroutine());
    }

    System.Collections.IEnumerator ReloadCoroutine()
    {
        audioSource.PlayOneShot(reloadSound);

        isReloading = true;

        yield return new WaitForSeconds(reloadTime);

        magazinesLeft--;
        bulletsInMagazine = magazineSize;

        isReloading = false;
    }

    void UpdateAmmoUI()
    {
        ammoText.text = bulletsInMagazine + " / " + magazineSize;
        magText.text = magazinesLeft.ToString();
    }
}