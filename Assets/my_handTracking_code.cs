using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

using UnityEngine.XR.Hands;
using UnityEngine.XR.Management;
//camera offset 1.1176

using Unity.Robotics.ROSTCPConnector.ROSGeometry;

public class my_handTracking_code : MonoBehaviour
{
    private XRHand hand; // dichiarazione della variabile hand

    private float timeElapsed;
    
    
    public float printConsoleFrequency = 4.0f;
    private float timeElapsedConsole;

    //per trasformare
    public Transform origin;
    /*
    PROVA PER GRIP MANO
    */
    //private XRHandDevice targetDevice; boh forse inizializz  giusta ma non so poi come accedere oltre
    /*
    PROVA PER GRIP MANO
    */
    // Start is called before the first frame update
    float roll,pitch,yaw;

    void Start()  
    {
    XRHandSubsystem m_Subsystem = 
        XRGeneralSettings.Instance?
            .Manager?
            .activeLoader?
            .GetLoadedSubsystem<XRHandSubsystem>();

    if (m_Subsystem != null)
        m_Subsystem.updatedHands += OnHandUpdate;
    }

    public Pose ToWorldPose(XRHandJoint joint, Transform origin)
    {
      Pose xrOriginPose = new Pose(origin.position, origin.rotation);
      if(joint.TryGetPose(out Pose jointPose))
      {
            return jointPose.GetTransformedBy(xrOriginPose);
      }
      else
       return Pose.identity;

    }


    void OnHandUpdate(XRHandSubsystem subsystem,
                  XRHandSubsystem.UpdateSuccessFlags updateSuccessFlags,
                  XRHandSubsystem.UpdateType updateType) // ultima riga hands aggiunta dopo
    {

        //solo per debug stampa
        timeElapsedConsole += Time.deltaTime;
      //////////////Debug.Log(" SX trackata = " + subsystem.leftHand.isTracked);
    // Debug.Log(" DX tracked = " + subsystem.rightHand.isTracked);
     ////////////// Debug.Log(" LEFT POSITION = " + subsystem.leftHand.rootPose);
    
    /* for(var i = XRHandJointID.BeginMarker.ToIndex();
                i < XRHandJointID.EndMarker.ToIndex(); 
                i++)
                {
//subsystem.leftHand.TryGetFeatureValue(CommonUsages.gripPosition); sbagliato
                // forse il migliore è l'1 : palm
        
        
                var MytrackingJoint = subsystem.leftHand.GetJoint(XRHandJointIDUtility.FromIndex(i)); //mettere i per stamparli tutti
                 Debug.Log(" Joint numero = "+ i + MytrackingJoint);//Con questo stampa tutto : Pos//rot//LinVEl//AngVEl ecc
                
                if (MytrackingJoint.TryGetPose(out Pose pose)) //per estrarre pose del singolo joint
                 {
                 //////////////Debug.Log("Joint Position : " + pose.position);
                 //////////////Debug.Log("Joint Rotation : " + pose.rotation);
                }

                */
                


                /*Remarks
                Joint poses are relative to the real-world point chosen by the user's device.
                To transform to world space so that the joint appears in the correct location relative to the user, transform the pose based on the XROrigin.
            Examples ----- https://docs.unity3d.com/Packages/com.unity.xr.hands@1.1/api/UnityEngine.XR.Hands.XRHandJoint.html#UnityEngine_XR_Hands_XRHandJoint_TryGetLinearVelocity_*/
/*
                if (MytrackingJoint.TryGetLinearVelocity(out Vector3 linVel)) //estrarre lin vel di singolo joint funziona!
                 {
                 //////////////Debug.Log("Joint Linear Velocity : " + linVel);
                
                }
                if (MytrackingJoint.TryGetAngularVelocity(out Vector3 AngVel)) //estrarre Angular vel di singolo joint funziona!
                 {
                 //////////////Debug.Log("Joint Angular Velocity : " + AngVel);
                
                }
                  
                } */
     var MytrackingJoint = subsystem.leftHand.GetJoint(XRHandJointIDUtility.FromIndex(1)); //mettere i per stamparli tutti
                
                
                //Con questo stampa tutto : Pos//rot//LinVEl//AngVEl ecc
                if(timeElapsedConsole > printConsoleFrequency) //si aggiorna rapidissimamente, per debug mi faccio stampare solo ogni 2 secondi
                {
                    //Debug.Log(" Joint numero = 1" + MytrackingJoint);
                    

                    Pose prova = ToWorldPose(MytrackingJoint, origin);
                    //Debug.Log(" ***** trasformata world ****** -->" + prova);

                    if (MytrackingJoint.TryGetPose(out Pose pose)) //per estrarre pose del singolo joint
                    {
                        //Debug.Log("unity Joint Position : " + pose.position);
                       // Debug.Log("unity Joint Rotation : " + pose.rotation);

                        //Quaternion prova = new Quaternion(orientX,orientY,orientZ,orientW);
                        //Quaternion invertedRotation = Quaternion.Inverse(pose.rotation);
                       // Vector3 eulerRotation = invertedRotation.eulerAngles;

                        // CONVERTO IN FRAME DI ROS
                        Vector3<FLU> toRosPosition = pose.position.To<FLU>();
                        Quaternion<FLU> toRosRotation = pose.rotation.To<FLU>();

                    /*
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
                    */
                         // Debug.Log(" ++++ Rotation (RPY in radians): " + roll + ", " + pitch + ", " + yaw); // PROBLEMA GIMBAL LOCK
                         // Debug.Log(" ++++  Rotation (RPY in GRADI): " + eulerRotation.z + ", " + eulerRotation.x + ", " + eulerRotation.y);

                        // Debug.Log(" ################### AAA ##################### ");
                         //Debug.Log(" ++++  SENZA INVERTIRE XYZ Rotation (RPY in GRADI): " + eulerRotation.x + ", " + eulerRotation.y + ", " + eulerRotation.z);
                         Debug.Log(" SX SINSTRA  ROSPosition : x =  " + toRosPosition.x + " y = " + toRosPosition.y + " z = " + toRosPosition.z);
                         Debug.Log(" SX SINSTRA  ROSrotation : x =  " + toRosRotation.x + " y = " + toRosRotation.y + " z = " + toRosRotation.z + " W = " + toRosRotation.w );
                         
                         //Debug.Log(" #################### BBB ################# ");
                    }

                    timeElapsedConsole = 0;
                }
                /*
                if (MytrackingJoint.TryGetPose(out Pose pose)) //per estrarre pose del singolo joint
                 {
                 Debug.Log("Joint Position : " + pose.position);
                 Debug.Log("Joint Rotation : " + pose.rotation);
                }
                */
     
    switch (updateType)
      {
        case XRHandSubsystem.UpdateType.Dynamic:
            // Update game logic that uses hand data
            //hand = subsystem.TryGetHand(0); // KKK
            //subsystem.TryGetHand(0, out XRHand xrHand);
            /*for(var i = XRHandJointID.BeginMarker.ToIndex();
                i < XRHandJointID.EndMarker.ToIndex(); 
                i++)
            {
                var trackingData = hand.GetJoint(XRHandJointIDUtility.FromIndex(i));
                Debug.Log(" ------- " + trackingData);
                
                //Debug.Log(" trackato =  " + trackingData.isTracked());
                
                if (trackingData.TryGetPose(out Pose pose))
                 {
        // displayTransform is some GameObject's Transform component
                //displayTransform.localPosition = pose.position;
                //displayTransform.localRotation = pose.rotation;
                Debug.Log("Hand Position : " + pose.position);
                Debug.Log("Hand Rotation : " + pose.rotation);
                }
                else{Debug.Log("AAAAAAAAAAAAAAAAAAA");}

            }*/
            break;
        case XRHandSubsystem.UpdateType.BeforeRender: 
            // Update visual objects that use hand data
            //Debug.Log("BBBBBBBBBBB");
            break;
      }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

