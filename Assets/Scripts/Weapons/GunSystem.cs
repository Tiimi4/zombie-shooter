using System;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;
namespace Weapons
{
    public class GunSystem : MonoBehaviour
    {

        // Gun stats
        [field: SerializeField] private int damage { get; set; }
        [field: SerializeField] private int bulletsLeft { get; set; }
        [field: SerializeField] private int maxBullets { get; set; }
        [field: SerializeField] private float range { get; set; }

        private float shotCooldown { get; set; }
        [field: SerializeField] private int rpm { get; set; }

        private bool _isShotOnCooldown => shotCooldown > 0.001;

    
        // Reference
        public Camera fpsCam;
        public RaycastHit[] RayHits = new RaycastHit[15];
        public LayerMask whatIsEnemy;
        private Transform cameraTransform;


        private void Awake()
        {
            fpsCam = Camera.main;
            cameraTransform = fpsCam.transform;

        }

        public void FixedUpdate()
        {
            if (shotCooldown > 0)
            {
                shotCooldown = Mathf.MoveTowards(shotCooldown, 0, Time.fixedDeltaTime);
            }
        }

        public void Shoot()
        {
            if (_isShotOnCooldown) return;
            if (bulletsLeft == 0)
            {
                EmptyClick();
                //return;
            }

             int hitCount =  Physics.RaycastNonAlloc(cameraTransform.position, cameraTransform.forward, RayHits, range, whatIsEnemy);
             if (hitCount > 0)
             {
                 var firstHit = RayHits.Take(hitCount).OrderBy(hit => hit.distance).First();
                 Debug.Log($"Distanse: {firstHit.distance} object: {firstHit.collider.gameObject.name} ");
                 Debug.DrawLine(cameraTransform.position, firstHit.point, Color.magenta, 5f, false);
             }  
           
             
            
            bulletsLeft -= 1;
            shotCooldown = 60f / rpm;
           
           

        }

        public void EmptyClick()
        {
            
        }

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
