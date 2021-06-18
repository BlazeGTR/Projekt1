using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Viewbob : MonoBehaviour
{
    public float walkingBobbingSpeed = 11f;
    public float bobbingAmount = 0.07f;
    public PlayerMovement controller;

    float defaultPosY = 0;
    float DefaultPosViewModelY = 0;
    float timer = 0;
    GameObject viewModel;

    // Start is called before the first frame update
    void Start()
    {
        defaultPosY = transform.localPosition.y;
        controller = GetComponentInParent<PlayerMovement>();
        viewModel = GameObject.FindGameObjectWithTag("ViewModel");
        DefaultPosViewModelY = viewModel.transform.localPosition.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (Mathf.Abs(controller.move.x) > 0.1f || Mathf.Abs(controller.move.z) > 0.1f)
        {
            //Player is moving
            timer += Time.deltaTime * walkingBobbingSpeed;
            transform.localPosition = new Vector3(transform.localPosition.x, defaultPosY + Mathf.Sin(timer) * bobbingAmount, transform.localPosition.z);
            viewModel.transform.localPosition = new Vector3(0, (defaultPosY - Mathf.Sin(timer) * bobbingAmount/3)-2.125f,1);

        }
        else
        {
            //Idle
            timer = 0;
            transform.localPosition = new Vector3(transform.localPosition.x, Mathf.Lerp(transform.localPosition.y, defaultPosY, Time.deltaTime * walkingBobbingSpeed), transform.localPosition.z);
           viewModel.transform.localPosition = new Vector3(viewModel.transform.localPosition.x, Mathf.Lerp(viewModel.transform.localPosition.y, DefaultPosViewModelY, Time.deltaTime * walkingBobbingSpeed), viewModel.transform.localPosition.z);
        }
    }
}