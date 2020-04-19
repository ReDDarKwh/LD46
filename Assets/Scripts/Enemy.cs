using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{

  public RobotFriend robotTarget;

  public PlayerController playerTarget;

  public CircleCollider2D neighborCollider;

  public CircleCollider2D targetNeighborCollider;


  public Rigidbody2D rb;

  public GameObject projectile;

  public float speed;

  public float separationSpeed;

  public string separationLayerName;

  public float playerDistanceFocus;

  public float shootingCooldown;

  private float lastShot;

  public Transform target;
  public Animator animator;
  public float minSpeed;
  public float stopDistance;
  private SceneController sceneController;

  public AudioSource damageNoise;

  public AudioClip weaponNoise;


  // Start is called before the first frame update
  void Start()
  {
    sceneController = GameObject.FindGameObjectWithTag("SceneController").GetComponent<SceneController>();
    lastShot = Time.time;
    Init();
  }

  // Update is called once per frame
  void Update()
  {

    if (robotTarget.isDead || (playerTarget.transform.position - transform.position).sqrMagnitude < Mathf.Pow(playerDistanceFocus, 2))
    {
      target = playerTarget.transform;
      targetNeighborCollider = playerTarget.playerSeparationCircle;
    }
    else
    {
      target = robotTarget.transform;
      targetNeighborCollider = robotTarget.robotSeparationCircle;
    }

    if (Time.time - lastShot > shootingCooldown)
    {
      Shoot();
    }
    
    var separation = Separation();

    var velocity = separation.normalized * separationSpeed;

    if ((target.position - transform.position).sqrMagnitude > Mathf.Pow(targetNeighborCollider.radius + neighborCollider.radius + 2, 2))
    {
      velocity += (target.position - transform.position).normalized * speed;
    }

    var dir = Mathf.Sign(Vector3.Dot(target.position - transform.position, Vector3.right));

    if (dir == 1)
    {
      transform.eulerAngles = new Vector3(0, 0, 0);
    }
    else if (dir == -1)
    {
      transform.eulerAngles = new Vector3(0, 180, 0);
    };

    if (velocity.sqrMagnitude != 0)
    {
      animator.SetBool("running", true);
    }
    else
    {
      animator.SetBool("running", false);
    }


    rb.MovePosition(rb.position + (Vector2)velocity);
  }

  private void Shoot()
  {
    lastShot = Time.time;

    sceneController.PlaySound(weaponNoise);

    var m = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Projectile>();
    m.target = target;
  }

  Vector3 Separation()
  {

    Vector3 v = Vector2.zero;
    Collider2D[] colliders = new Collider2D[20];
    ContactFilter2D contactFilter = new ContactFilter2D();
    contactFilter.SetLayerMask(LayerMask.GetMask(separationLayerName));
    neighborCollider.OverlapCollider(contactFilter, colliders);

    foreach (var collider in colliders)
    {
      if (collider != null)
      {
        if (target != playerTarget.transform || collider.GetComponentInParent<RobotFriend>() == null)
        {
          v += collider.transform.position - transform.position;
        }
      }
    }

    return v *= -1;
  }

  public override void AddDamage(float damage)
  {
    hp -= damage;

    damageNoise.Play();

    if (hp <= 0) {
      Die();
    }
  }

  private void Die()
  {
    BaseDie();
    sceneController.OnEnemyKilled(transform.position);
    Destroy(gameObject);
  }
}
