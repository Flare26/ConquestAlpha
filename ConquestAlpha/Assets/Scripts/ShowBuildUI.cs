using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowBuildUI : MonoBehaviour
{
    public GameObject baseUI;
    // Start is called before the first frame update
    void Start()
    {
        baseUI.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        baseUI.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        baseUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        TurretBuilder m_builder = GetComponent<TurretBuilder>();
        baseUI.GetComponent<Text>().text = Mathf.RoundToInt(m_builder.buildPercent).ToString() + "% built";
    }
}
