using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class AvatarSelectionUI : MonoBehaviour
{
    public static AvatarSelectionUI Singleton;

    public Button nextHead;
    public Button previousHead;
    public Button nextBody;
    public Button previousBody;
    public Slider skinSlider;
    public TMPro.TMP_InputField nameInputField;

    private NetworkAvatar currentNetworkAvatar;

    public void UpdateHeadIndex(int newIndex)
    {
        if (!currentNetworkAvatar)
            return;

        if (newIndex >= currentNetworkAvatar.headParts.Length)
            newIndex = 0;
        if (newIndex < 0)
            newIndex = currentNetworkAvatar.headParts.Length - 1;

        NetworkAvatar.NetworkAvatarData newdata = currentNetworkAvatar.networkAvatarData.Value;
        newdata.headIndex = newIndex;
        currentNetworkAvatar.networkAvatarData.Value = newdata;
    }

    public void UpdateBodyIndex(int newIndex)
    {
        if (!currentNetworkAvatar)
            return;

        if (newIndex >= currentNetworkAvatar.bodyParts.Length)
            newIndex = 0;
        if (newIndex < 0)
            newIndex = currentNetworkAvatar.bodyParts.Length - 1;

        NetworkAvatar.NetworkAvatarData newdata = currentNetworkAvatar.networkAvatarData.Value;
        newdata.bodyIndex = newIndex;
        currentNetworkAvatar.networkAvatarData.Value = newdata;
    }

    public void UpdateSkinValue(float value)
    {
        if (!currentNetworkAvatar)
            return;

        NetworkAvatar.NetworkAvatarData newdata = currentNetworkAvatar.networkAvatarData.Value;
        newdata.skinColor = value;
        currentNetworkAvatar.networkAvatarData.Value = newdata;
    }

    public void UpdateNameValue(string value)
    {
        if (!currentNetworkAvatar)
            return;

        NetworkAvatar.NetworkAvatarData newdata = currentNetworkAvatar.networkAvatarData.Value;
        newdata.avatarName = value;
        currentNetworkAvatar.networkAvatarData.Value = newdata;
    }

    // Start is called before the first frame update
    void Start()
    {
        Singleton = this;

        nextHead.onClick.AddListener(() => UpdateHeadIndex(currentNetworkAvatar.networkAvatarData.Value.headIndex + 1));
        previousHead.onClick.AddListener(() => UpdateHeadIndex(currentNetworkAvatar.networkAvatarData.Value.headIndex - 1));

        nextBody.onClick.AddListener(() => UpdateBodyIndex(currentNetworkAvatar.networkAvatarData.Value.bodyIndex + 1));
        previousBody.onClick.AddListener(() => UpdateBodyIndex(currentNetworkAvatar.networkAvatarData.Value.bodyIndex - 1));

        skinSlider.onValueChanged.AddListener(UpdateSkinValue);

        nameInputField.onEndEdit.AddListener(UpdateNameValue);
    }

    public void Initialize(NetworkAvatar networkAvatar)
    {
        currentNetworkAvatar = networkAvatar;

        nameInputField.SetTextWithoutNotify(currentNetworkAvatar.networkAvatarData.Value.avatarName.ToString());
        skinSlider.SetValueWithoutNotify(currentNetworkAvatar.networkAvatarData.Value.skinColor);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
