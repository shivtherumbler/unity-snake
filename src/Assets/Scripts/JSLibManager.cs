using System.Runtime.InteropServices;
using UnityEngine;

public class JSLibManager : MonoBehaviour
{

#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void fetchDevice();
#endif

    //public GameObject phoneUI;
    public bool deviceModePhone;
    public void fetchDeviceUnity()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        fetchDevice();
#endif
    }


    public void isPhone(string isphoneBool)
    {
        if(isphoneBool == "true")
        {
            deviceModePhone = true;
            //phoneUI.SetActive(true);
        }
        else
        {
            deviceModePhone = false;
            //phoneUI.SetActive(false);
        }
    }
}
