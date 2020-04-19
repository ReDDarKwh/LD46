using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RobotFriend : Character
{

  public float shieldEnergy;
  public float maxShieldEnergy;

  public GameObject initialProjectilePrefab;
  public GameObject projectilePrefab;


  public Transform gunMuzzle;
  public GameObject gun;
  public CircleCollider2D robotSeparationCircle;

  // ui
  public Image shieldBar;
  public Image healthBar;

  public float initialShootingCooldown;

  public float shootingCooldown;
  private float lastShot;
  public Enemy target;

  public GameObject superWeaponPrefab;
  private float superWeaponTimeRunOut;
  private float gunSpeedRunOut;

  public float improvedShootingCooldown;
  private Color originalHealthBarColor;
  public Animator animator;

  public AudioSource weaponNoise;
  public AudioSource superWeaponNoise;


  public override void AddDamage(float damage)
  {
    if (shieldEnergy > 0)
    {
      shieldEnergy -= damage;

    }
    else
    {
      hp -= damage;
    }

    shieldBar.fillAmount = shieldEnergy / maxShieldEnergy;
    healthBar.fillAmount = hp / maxHp;

    if (healthBar.fillAmount < 0.1)
    {
      healthBar.color = Color.red;
    }
    else
    {
      healthBar.color = originalHealthBarColor;
    }

    if (hp <= 0)
    {
      Die();
    }
  }

  private void Die()
  {
    animator.enabled = false;
    BaseDie();
  }

  public void GiveEnergy(float amount)
  {
    shieldEnergy = Mathf.Min(shieldEnergy + amount, maxShieldEnergy);
    shieldBar.fillAmount = shieldEnergy / maxShieldEnergy;
  }

  // Start is called before the first frame update
  void Start()
  {
    originalHealthBarColor = healthBar.color;
    shieldBar.fillAmount = 1;
    healthBar.fillAmount = 1;

    shieldEnergy = maxShieldEnergy;
    Init();
  }

  void ChooseTarget()
  {
    var enemies = GameObject.FindGameObjectsWithTag("Enemy");
    var smallest = float.MaxValue;
    GameObject closestObject = null;

    foreach (var enemy in enemies)
    {
      var dis = (enemy.transform.position - transform.position).sqrMagnitude;
      if (dis < smallest)
      {
        smallest = dis;
        closestObject = enemy;
      }
    }

    if (closestObject != null)
    {
      target = closestObject.GetComponent<Enemy>();
    }
  }

  internal void GiveSuperWeapon(float battEnergyAmount)
  {
    superWeaponTimeRunOut = Time.time + battEnergyAmount;
  }

  internal void GiveGunSpeed(float battEnergyAmount)
  {
    gunSpeedRunOut = Time.time + battEnergyAmount;
    shootingCooldown = improvedShootingCooldown;
  }

  // Update is called once per frame
  void Update()
  {
    if (isDead)
    {
      return;
    }

    if (target != null && !target.isDead)
    {

      if (Time.time - lastShot > shootingCooldown)
      {
        Shoot();
      }

      var localToTarget = target.transform.position - transform.position;

      float rot_z = Mathf.Atan2(localToTarget.y, localToTarget.x) * Mathf.Rad2Deg;

      gun.transform.rotation = Quaternion.Euler(0f, 0f, rot_z);


      var dir = Mathf.Sign(Vector3.Dot(target.transform.position - transform.position, Vector3.right));

      if (dir == 1)
      {
        transform.eulerAngles = new Vector3(0, 0, 0);
      }
      else if (dir == -1)
      {
        transform.eulerAngles = new Vector3(0, 180, 0);
      };

    }
    else
    {
      ChooseTarget();
    }

    if (gunSpeedRunOut != 0 && Time.time > gunSpeedRunOut)
    {
      shootingCooldown = initialShootingCooldown;
    }

    if (superWeaponTimeRunOut != 0 && Time.time > superWeaponTimeRunOut)
    {

    }

  }

  private void Shoot()
  {
    lastShot = Time.time;
    var m = Instantiate(Time.time > superWeaponTimeRunOut ? projectilePrefab : superWeaponPrefab, gunMuzzle.transform.position, Quaternion.identity).GetComponent<Projectile>();
    m.target = target.transform;

    if (Time.time > superWeaponTimeRunOut)
    {
      weaponNoise.Play();
    }
    else { superWeaponNoise.Play(); };
  }
}
