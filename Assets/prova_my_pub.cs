using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.UnityRoboticsDemo;

public class prova_my_pub : MonoBehaviour
{
    ROSConnection ros;
    public string topicName = "left_arm_frame_topic";

    // The game object 
   // public GameObject cube;
    // Publish the cube's position and rotation every N seconds
    // NOTA, non so perche ma c'e un fattore x10 qua, ossia se metto 2sec qua in realta su ros arrivano ogni 0.2 sec
    //quindi mettendo 13 sec arrivano circa ogni 1,3 sec (in realta varibaile ma cmq varia tra 1.1 e 1.3 circa)
    public float publishMessageFrequency = 13.0f;//0.5f; 

    // Used to determine how much time has elapsed since the last message was published
    private float timeElapsed;
    //anche con x = 0.350f e' reachable ; lasciando tutti altri invariati
    //anche con x = 0.490f e' reachable ;
    // con x =0.55f ---> NON reachable ma gestita!! da mio metodo nuovo restituiendo
    //normal RIGHT Joint number 0 --> POSITION 1.119034
    // normal RIGHT Joint number 1 --> POSITION 1.233352
    //normal RIGHT Joint number 2 --> POSITION 0.737624
    // normal RIGHT Joint number 3 --> POSITION 1.281060
    // normal RIGHT Joint number 4 --> POSITION -1.351053
    //Joint n 5 --> oltre limite MAX, val calcolato: 1.439854 settato a max= 1.390000
    //normal RIGHT Joint number 6 --> POSITION -0.203290

    //POS NOTA RIGHT
    /*
    private float posX = 0.550f; //0.216f; //default 0.216f
    private float posY = -0.228f;
    private float posZ = -0.476f;
    */
    //rotazione right
    /*
    private float orientX = 0.537f;
    private float orientY = -0.436f;
    private float orientZ = 0.480f;
    private float orientW = 0.539f;
    */
    
    //POS NOTA left
    /*
    private float posX = 0.216f; //0.216f; //default 0.216f
    private float posY = 0.228f;
    private float posZ = -0.476f;
    */
    //prima pos left estratta da visore e convertita con converter..a cui ho cambiato i segni di x e y
    //private float posX = 0.398f; 
    //private float posY = 0.250f;
    //private float posZ = -0.476f;

    //seconda pos left estratta da visore e convertita con converter...non cambio nulla (ma nel converter ho invertito assi!!!)
    private float posX = 0.466f; 
    private float posY = -0.031f;
    private float posZ = 0.0f;//quella vera convertita sarebbe = 0.219f;
    //rotazione left nota iniziale
    /*
    private float orientX = 0.539f;
    private float orientY = -0.481f;
    private float orientZ = 0.436f;
    private float orientW = 0.537f;
    */ //rotazione SX 26Giugno, Palmo verso la faccia     (0.0972, -0.5619, -0.8156, -0.0980)
    private float orientX = 0.0972f;
    private float orientY = -0.5619f;
    private float orientZ = -0.8156f;
    private float orientW = -0.0980f;
    float roll,pitch,yaw;
    
    void Start()
    {
        // start the ROS connection
        ros = ROSConnection.GetOrCreateInstance();
        // inizializza publisher su topic pos_rot che accetta msg di tipo PosRotMsg
        //ros.RegisterPublisher<MyPosRotMsg>(topicName);
        ros.RegisterPublisher<PosRotMsg>(topicName);
    }

    private void Update()
    {
        timeElapsed += Time.deltaTime;
        
        Quaternion prova = new Quaternion(orientX,orientY,orientZ,orientW);
    Quaternion invertedRotation = Quaternion.Inverse(prova);
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
        
        if (timeElapsed > publishMessageFrequency)
        {
            


            //MyPosRotMsg myPos = new MyPosRotMsg(posX,posY,posZ, roll,pitch,yaw );
            PosRotMsg myPos = new PosRotMsg(posX,posY,posZ, orientX,orientY,orientZ,orientW );

            // Finally send the message to server_endpoint.py running in ROS
            ros.Publish(topicName, myPos);
            //Debug.Log("pubblicato pos?" + cubePos);
            Debug.Log("pubblicato pos?" + myPos);
            timeElapsed = 0;
        }
    }
}
