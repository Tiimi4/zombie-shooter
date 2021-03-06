using System;
using System.Collections;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;
using TMPro;

namespace Weapons
{
    public class GunSystem : MonoBehaviour
    {

        // Gun stats
        [field: SerializeField] private int Damage { get; set; }
        [field: SerializeField] public float reloadTime { get; set; }

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
        private bool _isReloading = false;
        private bool previousTriggerState = false;
        

        public GameObject sandHitPrefab;
        public GameObject enemyHitPrefab;
        public GameObject muzzleFlashPrefab;
        
    
        // Reference
        public Camera fpsCam;
        private readonly RaycastHit[] _rayHits = new RaycastHit[15];
        public LayerMask whatIsEnemy;
        private Transform _cameraTransform;
        public Transform muzzlePosition;
        private AudioSource shotSound;
        public Recoil recoilScript;
        private Crosshair _crosshair;

        private TextMeshProUGUI _ammoText;
        private void Awake()
        {
            fpsCam = Camera.main;
            if (fpsCam != null) _cameraTransform = fpsCam.transform;
            
            MagazineBulletsLeft = MaxBullets;
            
            recoilScript = GetComponent<Recoil>();
            shotSound = GetComponent<AudioSource>();


        }

        public void Start()
        {
            _crosshair = GameObject.Find("Crosshair").GetComponent<Crosshair>();
           InitAmmoTextRef();
           CockWeapon();

        }

        public void InitAmmoTextRef()
        {
            _ammoText = GameObject.Find("ammoText").GetComponent<TextMeshProUGUI>();
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

        public void UpdateAmmoText()
        {
            if (MagazineBulletsLeft > 0)
            {
                _ammoText.text = MagazineBulletsLeft + 1 + " / " + MaxBullets;
                return;
            }
            if (_isChambered)
            {
                _ammoText.text = 1 + " / " + MaxBullets;
                return;
            }
            _ammoText.text = 0 + " / " + MaxBullets;
            
           
        }

        public void PullTrigger(bool isHeld)
        {
            _isTriggerHeld = isHeld;
        }

        public void FillAndCockWeapon()
        {
            MagazineBulletsLeft = MaxBullets;
            CockWeapon();
            _isReloading = false;
        }

        public void StartReload()
        {
            _isReloading = true;
            _ammoText.text = "Reloading...";
        }

        private void Fire()
        {
            if (_isReloading) return;
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
                
                 if (firstHit.collider.CompareTag("Enemy"))
                 {
                     
                     EnemyScript enemyRef = firstHit.collider.gameObject.GetComponent<EnemyScript>();
                     if (enemyRef)
                     {
                         enemyRef.HpSystem.Damage(Damage);
                         Instantiate(enemyHitPrefab, firstHit.point,
                             Quaternion.FromToRotation(Vector3.forward, firstHit.normal));
                     }
                     
                 }
                 else
                 {
                     
                     Instantiate(sandHitPrefab, firstHit.point,
                         Quaternion.FromToRotation(Vector3.forward, firstHit.normal));
                 }
             }



            // Recoil
             recoilScript.RecoilFire();
             
             _crosshair.bulletWasShot = true;
            // Muzzle effect
            Instantiate(muzzleFlashPrefab, muzzlePosition);
             
            // Shoot sound effect
            shotSound.Play();
             
            // begin reset shot
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
            UpdateAmmoText();
            
        }


    }
}
