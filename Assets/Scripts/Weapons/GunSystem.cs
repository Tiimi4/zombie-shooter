using System;
using System.Collections;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;
namespace Weapons
{
    public class GunSystem : MonoBehaviour
    {

        // Gun stats
        [field: SerializeField] private int Damage { get; set; }
        [field: SerializeField] private int MagazineBulletsLeft { get; set; }
        [field: SerializeField] private int MaxBullets { get; set; }
        [field: SerializeField] private float Range { get; set; }
        [field: SerializeField] private bool isFullAuto { get; set; }

        private float ShotCooldown { get; set; }
        [field: SerializeField] private int Rpm { get; set; }
        public Action OnEmptyClick;

        private bool IsShotOnCooldown => ShotCooldown > 0.001; 
        private bool HasAmmoLeft => MagazineBulletsLeft > 0;
        private bool _isChambered = false;
        private bool _isTriggerHeld = false;
        private bool _isCocked = false;
        private bool previousTriggerState = false;
        

        public GameObject sandHitPrefab;
        public GameObject muzzleFlashPrefab;

    
        // Reference
        public Camera fpsCam;
        private readonly RaycastHit[] _rayHits = new RaycastHit[15];
        public LayerMask whatIsEnemy;
        private Transform _cameraTransform;
        public Transform muzzlePosition;


        private void Awake()
        {
            fpsCam = Camera.main;
            if (fpsCam != null) _cameraTransform = fpsCam.transform;

            MagazineBulletsLeft = MaxBullets;
            CockWeapon();
        }

        public void Update()
        {
            if (_isTriggerHeld && isFullAuto)
            {
                Fire();
            }

            if (!previousTriggerState && _isTriggerHeld && !isFullAuto)
            {
                Fire();
            }

            previousTriggerState = _isTriggerHeld;

        }

        public void PullTrigger(bool isHeld)
        {
            _isTriggerHeld = isHeld;
        }

        public void FillAndCockWeapon()
        {
            MagazineBulletsLeft = MaxBullets;
            CockWeapon();
        }

        private void Fire()
        {
            if (!_isCocked) return;
            if (!_isChambered)
            {
                EmptyClick();
                return;
            }
          

             int hitCount =  Physics.RaycastNonAlloc(_cameraTransform.position, _cameraTransform.forward, _rayHits, Range, whatIsEnemy);
             if (hitCount > 0)
             {
                 // render hit effect
                 var firstHit = _rayHits.Take(hitCount).OrderBy(hit => hit.distance).First();
                 Instantiate(sandHitPrefab, firstHit.point,
                     Quaternion.FromToRotation(Vector3.forward, firstHit.normal));
             }



            Instantiate(muzzleFlashPrefab, muzzlePosition);
            _isChambered = false;
            _isCocked = false;
            StartCoroutine(nameof(HandleCocking));

        }

        private IEnumerator HandleCocking()
        {
            yield return new WaitForSeconds(60f / Rpm);
            CockWeapon();
        }
        public void EmptyClick()
        {
            OnEmptyClick?.Invoke();
        }

        public void CockWeapon()
        {
            _isCocked = true;

            if (HasAmmoLeft)
            {
                _isChambered = true;
                MagazineBulletsLeft--;
            }
            
        }


    }
}
