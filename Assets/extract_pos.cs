using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class extract_pos : MonoBehaviour
{
     // The game object 
    public GameObject cube;
    public Transform origin;

    private float printMessageFrequency = 4.0f;

    // Used to determine how much time has elapsed since the last message was published
    private float timeElapsed;
    Vector3 pos;
    Quaternion rot, prova;
    Pose TransformedPose, InTransformedPose;
    // Start is called before the first frame update
     float roll,pitch,yaw;

     float rollA,pitchA,yawA;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timeElapsed += Time.deltaTime;
     
        if (timeElapsed > printMessageFrequency)
        {
            //cube.transform.rotation = Random.rotation;

            //qui e' dove crea il messaggio PosRot da pubblicare (CREDO)
            //tra l altro lo crea come stesse usando il costruttore di una classe..
            Debug.Log("Cube position; x = "
               + cube.transform.position.x
               + " y =" + cube.transform.position.y
                + " z =" +cube.transform.position.z
                + " rotX =" +cube.transform.rotation.x
                + " rotY = " +cube.transform.rotation.y
                + " rotZ = " +cube.transform.rotation.z
                + " rotW = " + cube.transform.rotation.w
            );

            pos = new Vector3(cube.transform.position.x,cube.transform.position.y,cube.transform.position.z);
            rot = new Quaternion(cube.transform.rotation.x,cube.transform.rotation.y,cube.transform.rotation.z,cube.transform.rotation.w);
            Pose CubePos = new Pose(pos, rot);
            Pose OriginPose = new Pose(origin.position, origin.rotation);
            //Debug.Log("ORIGin pose " + OriginPose);
            TransformedPose = CubePos.GetTransformedBy(OriginPose);
            Debug.Log("TRASFormata rispetto a frame sel:  " + TransformedPose);

            //prova = new Quaternion(cube.transform.localRotation.x,cube.transform.localRotation.y,cube.transform.localRotation.z,cube.transform.localRotation.w);
            //Debug.Log("provaRot:  " + prova);

             //Quaternion prova = new Quaternion(orientX,orientY,orientZ,orientW);
                        Quaternion invertedRotation = Quaternion.Inverse(TransformedPose.rotation);
                        Vector3 eulerRotation = invertedRotation.eulerAngles;
                        roll = eulerRotation.z * Mathf.Deg2Rad;
                        pitch = eulerRotation.x * Mathf.Deg2Rad;
                        yaw = eulerRotation.y * Mathf.Deg2Rad;

                        Debug.Log(" ++++  Rotation (RPY in radians): " + roll + ", " + pitch + ", " + yaw);
                         Debug.Log(" ++++  Rotation (RPY in GRADI): " + eulerRotation.z + ", " + eulerRotation.x + ", " + eulerRotation.y);

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
                         
                         Debug.Log(" +++++++++++++++++++++++++++ ");
                         Debug.Log(" ++++  Rotation (RPY in radians): " + roll + ", " + pitch + ", " + yaw);
                         Debug.Log(" ++++  Rotation (RPY in GRADI): " + eulerRotation.z + ", " + eulerRotation.x + ", " + eulerRotation.y);

                         rollA = eulerRotation.x * Mathf.Deg2Rad;
                        pitchA = eulerRotation.y * Mathf.Deg2Rad;
                        yawA = eulerRotation.z * Mathf.Deg2Rad;

                        Debug.Log(" ############################# ");
                         //Debug.Log(" ++++ NON INVERTENDO XYZ (RPY in radians): " + rollA + ", " + pitchA + ", " + yawA);
                         Debug.Log(" ++++ NON INVERTENDO XYZ (RPY in GRADI): " + eulerRotation.x + ", " + eulerRotation.y + ", " + eulerRotation.z);
            //InTransformedPose = OriginPose.GetTransformedBy(CubePos);
            //Debug.Log("INVERSA TRASF " + InTransformedPose);
            // Finally send the message to server_endpoint.py running in ROS
            //ros.Publish(topicName, cubePos);
            //Debug.Log("pubblicato pos?" + cubePos);
            timeElapsed = 0;
        }
    }
}
