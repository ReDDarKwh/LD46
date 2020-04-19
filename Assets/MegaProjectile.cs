using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MegaProjectile : MonoBehaviour
{

  public GameObject energyPrefab;

  public int explodeNum;

  // Start is called before the first frame update
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {

  }

  public void Explode()
  {
    var div = 360 / explodeNum;
    for (var i = 0; i < explodeNum; i++) {
      var m = Instantiate(energyPrefab, transform.position, Quaternion.identity).GetComponent<Projectile>();
      m.targetPos = transform.position + Quaternion.AngleAxis(div * i, Vector3.forward) * Vector3.right;
    }
    
  }
}
