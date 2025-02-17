using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;


public class InputMapping : MonoBehaviour
{

    private PlayerInput playerInput;
    private InputActions inputActions;
    public TextMeshProUGUI text;

    // Start is called before the first frame update
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        inputActions = new InputActions();
        //INFO
        /// 330+ Controller Inputs; each controller has 20 buttons
        /// 350+ Controller 1
        /// 370+ Controller 2, etc.
        
    }


    // Update is called once per frame
    void Update()
    {

        
    }
    public void MapInput(string excludedControl)
    {

    }

    public void StartMappingInputs(int currentButton =-1)
    {
        
    }
    public void SendData()
    {

    }

}
