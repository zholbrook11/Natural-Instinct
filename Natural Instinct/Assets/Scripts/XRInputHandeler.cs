using UnityEngine;
using UnityEngine.InputSystem;

public class XRInputHandler : MonoBehaviour
{
    public GrapplingHook grapplingHook;
    private InputAction grappleAction;
    private XRControls actionAsset;

    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
            grapplingHook.StartGrapple();
        if (Keyboard.current.spaceKey.wasReleasedThisFrame)
            grapplingHook.StopGrapple();
    }


    private void OnEnable()
    {
        var actionAsset = new XRControls(); // auto-generated class from Input Actions
        grappleAction = actionAsset.XRActions.Grapple;
        grappleAction.Enable();

        grappleAction.performed += ctx => grapplingHook.StartGrapple();
        grappleAction.canceled += ctx => grapplingHook.StopGrapple();
    }

    private void OnDisable()
    {
        if (grappleAction != null)
        {
            grappleAction.Disable();
        }
    }

    void OnDestroy()
    {
        if (grappleAction != null)
        {
            grappleAction.Disable();
        }
    }

}