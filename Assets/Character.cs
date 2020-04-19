using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
  public float hp;
  public float maxHp;
  public bool isDead;
  public abstract void AddDamage(float damage);

  public void Init()
  {
    hp = maxHp;
  }

  public void BaseDie()
  {
    isDead = true;
  }

}
