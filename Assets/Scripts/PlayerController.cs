using UnityEngine;
using UnityEngine.UI;

// Ensure the component is present on the gameobject the script is attached to
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : Character
{
  // Local rigidbody variable to hold a reference to the attached Rigidbody2D component
  new Rigidbody2D rigidbody2D;

  public float movementSpeed = 1000.0f;
  public Animator animator;
  private Item itemInHand;

  public Transform holdingLocation;

  public CircleCollider2D playerSeparationCircle;
  public CircleCollider2D playerPickupCircle;

  public float jumpForce;
  public float jumpCoolDown;
  public float lastJumpTime;

  public Image healthBar;

  public float jumpDuration;
  public LayerMask itemLayerMask;


  private void Start() 
  {
    Init();
  }

  void Awake()
  {
    // Setup Rigidbody for frictionless top down movement and dynamic collision
    rigidbody2D = GetComponent<Rigidbody2D>();

    rigidbody2D.isKinematic = false;
    rigidbody2D.angularDrag = 0.0f;
    rigidbody2D.gravityScale = 0.0f;
  }

  void Update()
  {
    if (isDead) {
      return;
    }

    // Handle user input
    Vector2 targetVelocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

    if (Input.GetMouseButtonDown(0)) {

      var contactFilter = new ContactFilter2D();
      contactFilter.SetLayerMask(itemLayerMask);
      var results = new Collider2D[10];

      var overlap = playerPickupCircle.OverlapCollider(contactFilter, results);
      if (overlap > 0)
      {
        foreach (var r in results)
        {
          if (r == null)
          {
            continue;
          }
          var item = r.GetComponent<Item>();
          if (item != itemInHand)
          {

            if (itemInHand != null) { 
              itemInHand.OnDrop();
            }

            var hoverItem = item;
            if (hoverItem != null)
            {
              hoverItem.OnTake(itemInHand, holdingLocation);
            }
            itemInHand = hoverItem;
            break;
          }
          else if(overlap == 1)
          {
            if (itemInHand != null)
            { 
              itemInHand.OnDrop();
            }
            itemInHand = null;
          }
        }
      }
     
    }

    if (Input.GetKeyDown(KeyCode.Space)) {

      if (Time.time - lastJumpTime > jumpCoolDown) {
        lastJumpTime = Time.time;
        animator.SetTrigger("jump");
      }
    }

    hp = Mathf.Min(hp + Time.deltaTime * 2, maxHp);
    healthBar.fillAmount = hp / maxHp;

    if (hp < maxHp)
    {
      healthBar.transform.parent.gameObject.SetActive(true);
    }
    else {
      healthBar.transform.parent.gameObject.SetActive(false);
    }

    Move(targetVelocity);
  }


  //void OnTriggerEnter2D(Collider2D other)
  //{
  //  var item = other.GetComponent<Item>();
  //  if (item != null) {
  //    hoverItem = item;
  //    Debug.Log(hoverItem);
  //  }
  //}

  //void OnTriggerExit2D(Collider2D other)
  //{
  //  hoverItem = null;
  //}

  void Move(Vector2 targetVelocity)
  {

    var dir = Vector2.Dot(targetVelocity, Vector2.right);

    if (dir == 1)
    {
      healthBar.transform.localEulerAngles = new Vector3(0, 0, 0);
      transform.eulerAngles = new Vector3(0, 0, 0);
    }
    else if (dir == -1)
    {
      healthBar.transform.localEulerAngles = new Vector3(0, 180, 0);
      transform.eulerAngles = new Vector3(0, 180, 0);
    };

    if (targetVelocity != Vector2.zero)
    {
      animator.SetBool("running", true);
    }
    else {
      animator.SetBool("running", false);
    }
    
    // Set rigidbody velocity
    rigidbody2D.velocity = (targetVelocity * movementSpeed * ((Time.time - lastJumpTime < jumpDuration) ? jumpForce : 1)) * Time.deltaTime; // Multiply the target by deltaTime to make movement speed consistent across different framerates
  }

  public override void AddDamage(float damage)
  {
    hp -= damage;
    healthBar.fillAmount = hp / maxHp;
    if (hp <= 0) {
      Die();
    }
  }
  private void Die()
  {
    if (!isDead) {
      animator.enabled = false;
      transform.Rotate(new Vector3(0, 0, -90));
      BaseDie();
    }
  }
}
