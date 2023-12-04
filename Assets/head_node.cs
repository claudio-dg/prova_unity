using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.UnityRoboticsDemo;

public class head_node : MonoBehaviour
{
    public XRNode inputSource; //a cosa serve?
    private InputDevice targetDevice;
    
    //per connessione ROS
    ROSConnection ros;
    public string topicName = "head_frame_topic";
    public float publishMessageFrequency = 0.3f;//0.4f;//1.0f;//6.0f;//10.0f;//0.5f; //corrisponde a 1sec in teoria
    // Used to determine how much time has elapsed since the last message was published
    private float timeElapsed;
    
    
    public float printConsoleFrequency = 0.4f;
    private float timeElapsedConsole;

    float roll,pitch,yaw;
    void Start()
    {
        // start the ROS connection
        ros = ROSConnection.GetOrCreateInstance();
        // inizializza publisher su topic pos_rot che accetta msg di tipo MyPosRotMsg
        ros.RegisterPublisher<MyPosRotMsg>(topicName);
    }
    // COSI FUNZIONA E STRAMPA I NOMI CORRETTAMENTTE
    void Update()
    {
        timeElapsed += Time.deltaTime;

        //solo per debug stampa
        timeElapsedConsole += Time.deltaTime;

//  **posizione & Rotazione visore !!!!!***
        var headMountedDisplays = new List<UnityEngine.XR.InputDevice>();
        var headDesiredCharacteristics = UnityEngine.XR.InputDeviceCharacteristics.HeadMounted | UnityEngine.XR.InputDeviceCharacteristics.TrackedDevice;
        UnityEngine.XR.InputDevices.GetDevicesWithCharacteristics(headDesiredCharacteristics, headMountedDisplays);

        foreach (var device in headMountedDisplays)
        {
            //Debug.Log(string.Format("Device name '{0}' has characteristics '{1}'", device.name, device.characteristics.ToString()));
            if (device != null)
            {
                /* POSIZIONE DEL VISORE NON MI SERVE; SERVE SOLO ROTAZIONE
                if (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.devicePosition, out Vector3 devicePos))
                {
                    Debug.Log("Head Mounted Display Position : " + devicePos); //COMMENTO LA SCRITTA PER PROVARE ALTRI SCRIPTS
                }
                */
                //estrai rotazione dal visore
                if (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.deviceRotation, out Quaternion deviceRot))
                {
                    if(timeElapsedConsole > printConsoleFrequency) //si aggiorna rapidissimamente, per debug mi faccio stampare solo ogni 2 secondi
                    {
                        Debug.Log("Head Mounted Display Rotation : " + deviceRot); //COMMENTO LA SCRITTA PER PROVARE ALTRI SCRIPTS
                        timeElapsedConsole = 0;
                        Quaternion invertedRotation = Quaternion.Inverse(deviceRot);
                        Vector3 eulerRotation = invertedRotation.eulerAngles;

                        if(eulerRotation.z > 180)
                        {
                            eulerRotation.z -=360;
                        }
                        if(eulerRotation.y > 180)
                        {
                            eulerRotation.y -=360;
                        }
                        if(eulerRotation.x > 180)
                        {
                            eulerRotation.x -=360;
                        }
                        roll = eulerRotation.z * Mathf.Deg2Rad;
                        pitch = eulerRotation.x * Mathf.Deg2Rad;
                        yaw = eulerRotation.y * Mathf.Deg2Rad;

                        Debug.Log(" ++++ Rotation (RPY in radians): " + roll + ", " + pitch + ", " + yaw);
                    }
                    
                //e pubblicala (dovro aggiungere controllo: se pos uguale o simile a prec evita di ripubblicarla)
                //il check "se visore esiste ed e montato in teoria e' gia fatto con ddevice != null credo"

                 //COMMENTO UN ATTIMO IL PUBLISHER DELLA TESTA
                if (timeElapsed > publishMessageFrequency)
                {
            


                     MyPosRotMsg headRotation = new MyPosRotMsg(0,0,0, roll,pitch,yaw );

                    // Finally send the message to server_endpoint.py running in ROS
                     ros.Publish(topicName, headRotation);
                     
                     Debug.Log("\n ############### pubblicato pos? " + headRotation +  " ###############\n");
                     timeElapsed = 0;
                }
                
                
                }
            }
        }
        //  **posizione visore***

    }


}
