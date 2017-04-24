using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public enum Action
{
    None, 
    Rotating,
    Zooming,
    Moving
}

public class cameraRotate : MonoBehaviour
{
    #region Properties

    public Transform target;

    public Slider slider; 

    public bool userControl;

    public bool rotateAtStart;

    private bool targetDefined = false;

    [SerializeField]
    private float speed = new float();

    private float maxSpeed = 100f;

    private Action currAction;

    private Vector3 targetPos;
    private Vector3 cameraPos;

    private float zoomMin = 1f;
    private float zoomMax = 50f;
    private float currZoomDistance;

    private readonly Dictionary<Action, float> coeffByAction = new Dictionary<Action, float>()
    {
        {Action.None, 0f},
        {Action.Rotating, 5f},
        {Action.Moving, 0.05f},
        {Action.Zooming, 1f}
    };

    #endregion

    #region Unity Callbacks

	void Start () 
    {
        if (target != null)
            affectTarget(target);

        manageSliderSpeed();

        if (rotateAtStart)
            transform.Rotate(Vector3.up, Random.Range(0f, 360f));
	}
	
	void Update () 
    {
        if (userControl)
            inputParam();
        
        if (currAction.Equals(Action.None))
        {
            if (targetDefined)
                transform.RotateAround(targetPos, Vector3.up, speed * Time.deltaTime);
            else
                transform.Rotate(Vector3.up, speed * Time.deltaTime);
        }
        else if (userControl)
        {
            actionParam();
        }

        if (userControl)
        {
            if (Input.GetKeyDown(KeyCode.R))
                reset();
        }
	}

    #endregion
   
    #region Methods

    public void reset()
    {
        target.position = targetPos;

        transform.position = cameraPos;
        transform.LookAt(target);

        currZoomDistance = Vector3.Distance(transform.position, target.position);
    }

    public void affectTarget(Transform tar)
    {
        target = tar;

        targetDefined = true;

        targetPos = tar.position;
        cameraPos = transform.position;
        currZoomDistance = Vector3.Distance(transform.position, target.position);

        reset();
    }

    private void manageSliderSpeed()
    {
        if (slider != null)
        {
            float normalizedValue = Mathf.Abs(speed) / maxSpeed;

            if (normalizedValue > 1f)
            {
                speed = 0f;
                normalizedValue = Mathf.Abs(speed) / maxSpeed;
            }

            slider.normalizedValue = normalizedValue;

            slider.minValue = -maxSpeed;
            slider.maxValue = maxSpeed;

            slider.onValueChanged.AddListener((float sliderValue) =>
                {
                    speed = sliderValue;
                });
        }
    }

    private void inputParam()
    {
        #if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL

        if (currAction.Equals(Action.None))
        {
            if (Input.GetMouseButtonDown(0))
                currAction = Action.Rotating;
            else if (Input.GetMouseButtonDown(1))
                currAction = Action.Moving;
            else if (Input.GetMouseButtonDown(2) || !Input.GetAxis("Mouse ScrollWheel").Equals(0))
                currAction = Action.Zooming;
        }
        else
        {
            if (currAction.Equals(Action.Rotating))
            {
                if (Input.GetMouseButtonUp(0))
                    currAction = Action.None;
            }
            else if (currAction.Equals(Action.Moving))
            {
                if (Input.GetMouseButtonUp(1))
                    currAction = Action.None;
            }
            else if (currAction.Equals(Action.Zooming))
            {
                if (!Input.GetMouseButton(2) && Input.GetAxis("Mouse ScrollWheel").Equals(0))
                    currAction = Action.None;
            }
        } 

        #elif UNITY_ANDROID || UNITY_IOS

        switch (Input.touchCount)
        {
            case 3:
                currAction = Action.Moving;
                break;
            case 2:
                currAction = Action.Zooming;
                break;
            case 1:
                currAction = Action.Rotating;
                break;
            case 0:
            default:
                currAction = Action.None;
                break;
        }

        #endif
    }

    private void actionParam()
    {
        #if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL

        switch (currAction)
        {
            case Action.Rotating:
                transform.RotateAround(targetPos, transform.up, Input.GetAxis("Mouse X") * coeffByAction[currAction]);
                transform.RotateAround(targetPos, transform.right, -Input.GetAxis("Mouse Y") * coeffByAction[currAction]);            
                break;
            case Action.Moving:
                target.Translate(Input.GetAxis("Mouse X") * coeffByAction[currAction], -Input.GetAxis("Mouse Y") * coeffByAction[currAction], 0f);
                break;
            case Action.Zooming:

                float scrollAxis;

                if (Input.GetMouseButton(2))
                    scrollAxis = Input.GetAxis("Mouse Y");
                else 
                    scrollAxis = Input.GetAxis("Mouse ScrollWheel");

                scrollAxis *= coeffByAction[currAction] * (currZoomDistance / zoomMax);

                currZoomDistance -= scrollAxis;
                if (currZoomDistance < zoomMax && currZoomDistance > zoomMin)
                    transform.Translate(0f, 0f, scrollAxis);
                else 
                    currZoomDistance += scrollAxis;
                break;
            case Action.None:
            default:
                break;
        }

        #elif UNITY_ANDROID || UNITY_IOS



        #endif
    }

    #endregion
}
