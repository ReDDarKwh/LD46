using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum ItemType
{
  Battery,
  GunSpeed,
  SuperWeapon,
  RobotFriend
}

public class Item : MonoBehaviour
{

  public ItemType itemType;

  public float battEnergyAmount;

  public float spawnChance;

  public AudioClip pickupSound;

  public AudioClip giveSound;

  public SceneController sceneController;


  // Start is called before the first frame update
  void Start()
  {
    sceneController = GameObject.FindGameObjectWithTag("SceneController").GetComponent<SceneController>();
  }

  // Update is called once per frame
  void Update()
  {

  }

  internal void OnTake(Item lastHeldObject, Transform holdingLocation)
  {
    if (itemType == ItemType.Battery || itemType == ItemType.GunSpeed || itemType == ItemType.SuperWeapon)
    {
      transform.SetParent(holdingLocation);
      transform.localRotation = Quaternion.identity;
      transform.localPosition = Vector3.zero;

      sceneController.PlaySound(pickupSound);
    }
    else if (itemType == ItemType.RobotFriend)
    {

      if (lastHeldObject != null)
      {

        sceneController.PlaySound(lastHeldObject.giveSound);

        var robotFriend = GetComponentInParent<RobotFriend>();
        if (lastHeldObject.itemType == ItemType.Battery)
        {
          robotFriend.GiveEnergy(lastHeldObject.battEnergyAmount);
        } else if (lastHeldObject.itemType == ItemType.GunSpeed) {
          robotFriend.GiveGunSpeed(lastHeldObject.battEnergyAmount);
        } else if (lastHeldObject.itemType == ItemType.SuperWeapon) {
          robotFriend.GiveSuperWeapon(lastHeldObject.battEnergyAmount);
        }
        Destroy(lastHeldObject.gameObject);
      }
    }
  }

  internal void OnDrop()
  {
    if (itemType != ItemType.RobotFriend)
    {
      transform.SetParent(null);
      transform.localRotation = Quaternion.identity;
    }
  }
}
