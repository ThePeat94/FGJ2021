﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Scriptables;
using UI;
using Unity.VisualScripting;
using UnityEngine;
using UnityTemplateProjects;

public class PlayerMovementController : MonoBehaviour
{
    private static PlayerMovementController s_instance;

    [SerializeField] private PlayerData m_playerData;
    [SerializeField] private GameObject m_funnySphere;

    private Vector3 m_moveDirection;
    private CharacterController m_characterController;
    private InputProcessor m_inputProcessor;
    private Animator m_animator;
    private GameObject m_currentInteractable;
    private PlayerDashController m_playerDashController;
    private Vector3 m_mousePlayerPosition;
    
    private static readonly int s_isWalkingHash = Animator.StringToHash("IsWalking");

    public static PlayerMovementController Instance => s_instance;


    private void Awake()
    {
        if (s_instance == null)
        {
            s_instance = this;
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }
        
        this.m_inputProcessor = this.GetComponent<InputProcessor>();
        this.m_playerDashController = this.GetComponent<PlayerDashController>();
        this.m_characterController = this.GetComponent<CharacterController>();
        this.m_animator = this.GetComponent<Animator>();
    }
    
    // Update is called once per frame
    void Update()
    {
        if (this.m_playerDashController.IsDashing) return;
        
        var mouseRay = Camera.main.ScreenPointToRay(this.m_inputProcessor.MouseScreenPosition);
        var p = new Plane(Vector3.up, this.transform.position);
        if( p.Raycast( mouseRay, out float hitDist) ){
            Vector3 hitPoint = mouseRay.GetPoint(hitDist);
            this.m_mousePlayerPosition = hitPoint;
        }
        this.Move();
        this.Rotate();
    }

    private void LateUpdate()
    {
        // this.UpdateAnimator();
    }
    
    protected void Move()
    {
        var forwardMovement = this.transform.forward * this.m_inputProcessor.Movement.y;
        var sideMovement = this.transform.right * this.m_inputProcessor.Movement.x;

        this.m_moveDirection = Vector3.Lerp(forwardMovement, sideMovement, 0.5f).normalized;
        this.m_moveDirection.y = Physics.gravity.y;
        this.m_characterController.Move(this.m_moveDirection * Time.deltaTime * this.m_playerData.MovementSpeed);
    }
        
    private void Rotate()
    {
        this.transform.LookAt(this.m_mousePlayerPosition);
    }

    private void UpdateAnimator()
    {
        this.m_animator.SetBool(s_isWalkingHash, this.m_moveDirection != Physics.gravity);
    }


}
