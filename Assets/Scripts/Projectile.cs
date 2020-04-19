using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Projectile : MonoBehaviour
{
  public Transform target;
  public Vector3? targetPos;
  private Vector3 velocity;
  public float creationTime;
  public float improveTime;
  public float collisionRadius;
  public float speed;
  public float maxSteeringForce;
  public float improveDuration;
  public float mass;
  public float increasedSteeringForce;
  public float increasedSteeringSpeed;
  public float maxSteeringSpeed;
  public LayerMask missileLayerMask;
  public float damage;
  public bool homing;

  public UnityEvent beforeDestroy;

  // Start is called before the first frame update
  void Start()
  {
    creationTime = Time.time;

    var missileToTargetVec = ((targetPos == null? target.transform.position : targetPos) - transform.position);
    velocity = missileToTargetVec.Value.normalized * maxSteeringSpeed;
  }

  // Update is called once per frame
  void Update()
  {

    if (homing && target != null)
    {



      var missileToTargetVec = (target.transform.position - transform.position);

      var progress = (Time.time - creationTime) >= improveTime ? Mathf.Min((Time.time - creationTime) - improveTime / improveDuration, 1) : 0;
      var desiredVelocity = (missileToTargetVec.normalized) * Time.deltaTime * speed;
      var sterring = Vector3.ClampMagnitude((desiredVelocity - velocity), maxSteeringForce + (increasedSteeringForce * progress)) / mass;

      velocity = Vector2.ClampMagnitude(velocity + sterring, maxSteeringSpeed + (increasedSteeringSpeed * progress));
    }

    transform.position += velocity;
      transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg);
  }

  private void OnTriggerEnter2D(Collider2D collision)
  {
    if (missileLayerMask == (missileLayerMask | (1 << collision.gameObject.layer))) {
      var character = collision.GetComponent<Character>();
      if (character != null)
      {
        character.AddDamage(damage);
      }

      beforeDestroy.Invoke();
      Destroy(this.gameObject);
    }
  }
}

