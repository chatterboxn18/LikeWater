using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServiceManager : MonoBehaviour
{
    public enum Services
    {
        FileService
    }
    
    public static Dictionary<Services, Service> ServiceCollection = new Dictionary<Services, Service>();
    private void Awake()
    {
        for (var i = 0; i < transform.childCount; i++)
        {
            ServiceCollection[Services.FileService] = transform.GetChild(0).GetComponent<Service>();
        }
    }
    
    

}
