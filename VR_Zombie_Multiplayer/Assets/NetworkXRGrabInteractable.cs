using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class NetworkXRGrabInteractable : XRGrabInteractable
{

    private SetOwnershipOnSelect selectOwner;

    // Start is called before the first frame update
    void Start()
    {
        selectOwner = GetComponent<SetOwnershipOnSelect>();
    }

    public override bool IsSelectableBy(IXRSelectInteractor interactor)
    {
        bool isNetworkGrabbable = (!selectOwner.isNetworkGrabbed.Value) || (selectOwner.isNetworkGrabbed.Value && selectOwner.IsOwner);
        return base.IsSelectableBy(interactor) && isNetworkGrabbable;
    }
}
