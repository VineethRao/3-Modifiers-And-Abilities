﻿using System.Collections;
using UnityEngine;

using RPG.CameraUI; // for mouse events

namespace RPG.Characters
{
    [RequireComponent(typeof(Character))]
    [RequireComponent(typeof(SpecialAbilities))]
    [RequireComponent(typeof(WeaponSystem))]
    public class PlayerControl : MonoBehaviour
    {
        float lastHitTime = 0f;
        Character character = null;
        SpecialAbilities abilities = null;
        WeaponSystem weaponSystem;

        void Start()
        {
            character = GetComponent<Character>();
            abilities = GetComponent<SpecialAbilities>();
            weaponSystem = GetComponent<WeaponSystem>();

            RegisterForMouseEvents();
        }

        void Update()
        {
            ScanForAbilityKeyDown();
        }

        private void ScanForAbilityKeyDown()
        {
            for (int keyIndex = 1; keyIndex < abilities.GetNumberOfAbilities(); keyIndex++)
            {
                if (Input.GetKeyDown(keyIndex.ToString()))
                {
                    abilities.AttemptSpecialAbility(keyIndex);
                }
            }
        }

        private void RegisterForMouseEvents()
        {
            var cameraRaycaster = FindObjectOfType<CameraUI.CameraRaycaster>();
            cameraRaycaster.onMouseOverEnemy += OnMouseOverEnemy;
            cameraRaycaster.onMouseOverPotentiallyWalkable += OnMouseOverPotentiallyWalkable;
        }

        void OnMouseOverPotentiallyWalkable(Vector3 destination)
        {
            if (Input.GetMouseButton(0))
            {
                character.SetDestination(destination);
            }
        }

        void OnMouseOverEnemy(EnemyAI enemy)
        {
            float weaponHitPeriod = weaponSystem.GetCurrentWeapon().GetMinTimeBetweenHits();
            bool timeToHitAgain = Time.time - lastHitTime > weaponHitPeriod;
            if (Input.GetMouseButton(0) && IsTargetInRange(enemy.gameObject) && timeToHitAgain)
            {
                weaponSystem.AttackTarget(enemy.gameObject);
                lastHitTime = Time.time;
            }
            else if (Input.GetMouseButton(0) && !IsTargetInRange(enemy.gameObject))
            {
                character.SetDestination(enemy.transform.position);
            }
            else if (Input.GetMouseButtonDown(1))
            {
                abilities.AttemptSpecialAbility(0, enemy.gameObject);
            }
        }

        bool IsTargetInRange(GameObject target)
        {
            float distanceToTarget = (target.transform.position - transform.position).magnitude;
            return distanceToTarget <= weaponSystem.GetCurrentWeapon().GetMaxAttackRange();
        }
    }
}