using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class XIDScan : MonoBehaviour
{
    public int num;
    public InputField inputField;
    public Button button;

    private void Start()
    {
        button.onClick.AddListener(GetXidOnBtnClick);
    }
    public void GetXidOnBtnClick()
    {
        Debug.Log("Xid from inputfield is: " + inputField.text);
        if (inputField.text=="")
        {
            Debug.Log("Entered xid field is empty");
        }
        else
        {
            Debug.Log(" Go to ArLoad Scene");
            inputField.text = "";
            SceneManager.LoadScene("ARLoadScene");
        }
    }

    void XidFound()
    {
      //  if (num.ToString() != null)
        {

        }
        

    }
    void XidNotFound()
    {
        
    }
}
