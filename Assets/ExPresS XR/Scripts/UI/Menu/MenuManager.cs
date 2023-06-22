using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    private const float LERP_STOP_ANGLE = 1.0f;

    [Tooltip("Container for all menus that should be handled.")]
    [SerializeField]
    private GameObject menuContainer;


    [Tooltip("Menu that is shown initially.")]
    [SerializeField]
    private Transform startMenu;

    [Tooltip("If enabled, the menu will follow the player. The menu will stay at its initial height and will only move along the horizontal plane.")]
    [SerializeField]
    private bool followPlayer;

    [Tooltip("The transform providing the direction and position where the menu is shown. Should usually be the rig's Main Camera.")]
    [SerializeField]
    private Transform followTransform;

    [Tooltip("The distance to the followTransform the menu is rendered.")]
    [SerializeField]
    private float followDistance = 3.0f;

    [Tooltip("The speed at which the transform follows the player. A value of 1.0f moves instantaneously.")]
    [SerializeField]
    private float followSpeed = 1.0f;


    [Tooltip("The angle threshold for following. Slight movements will not trigger movement to keep it more stable ")]
    [SerializeField]
    private float menuMoveAngleThreshold = 20.0f;


    private bool currentlyLerping = false;


    // Start is called before the first frame update
    void Start()
    {
        if (followPlayer && followTransform == null)
        {
            Debug.LogError("The menu should follow the player but no transform was provided to follow.");
        }
        else if (followPlayer)
        {
            // Move it into position initially

            Vector3 targetPos = followTransform.position + new Vector3(followTransform.forward.x, 0, followTransform.forward.z).normalized * followDistance;
            targetPos.y = transform.position.y;

            transform.position = targetPos;
            transform.forward = new Vector3(followTransform.forward.x, 0, followTransform.forward.z).normalized;
        }

        ActivateStartMenu();
    }

    // Update is called once per frame
    void Update() => FollowPlayer();

    private void FollowPlayer()
    {
        if (!followPlayer || followTransform == null)
        {
            return;
        }

        Vector3 targetPos = followTransform.position + new Vector3(followTransform.forward.x, 0, followTransform.forward.z).normalized * followDistance;
        targetPos.y = transform.position.y;
        
        Vector3 targetFwd = new Vector3(followTransform.forward.x, 0, followTransform.forward.z).normalized;

        // Continue lerping it was already triggered or if the threshold for a new one was reached
        if (currentlyLerping || Vector3.Angle(transform.forward, targetFwd) > menuMoveAngleThreshold)
        {
            currentlyLerping = true;

            transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);
            transform.forward = Vector3.Lerp(transform.forward, targetFwd, followSpeed * Time.deltaTime);

            // Stop lerp if the target was (almost) reached
            currentlyLerping = Vector3.Angle(transform.forward, targetFwd) > LERP_STOP_ANGLE;
        }
    }




    public void ActivateStartMenu()
    {
        if (menuContainer == null || startMenu == null)
        {
            Debug.LogError("No menuContainer or startMenu provided, cannot activate the first menu.");
            return;
        }

        // Disable all but the start menu
        foreach (Transform menu in menuContainer.transform)
        {
            menu.gameObject.SetActive(menu == startMenu);
        }
    }
}
