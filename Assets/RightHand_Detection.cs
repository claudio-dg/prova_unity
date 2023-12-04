using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

using UnityEngine.XR.Hands;
using UnityEngine.XR.Management;
//camera offset 1.1176

using Unity.Robotics.ROSTCPConnector.ROSGeometry;

public class RightHand_Detection : MonoBehaviour
{
    private XRHand hand; // dichiarazione della variabile hand

    private float timeElapsed;
    
    
    public float printConsoleFrequency = 5.0f;
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
      
    //Debug.Log(" DX tracked = " + subsystem.rightHand.isTracked);
    
                var MytrackingJoint = subsystem.rightHand.GetJoint(XRHandJointIDUtility.FromIndex(1)); //mettere i per stamparli tutti
                
                
                //Con questo stampa tutto : Pos//rot//LinVEl//AngVEl ecc
                if(timeElapsedConsole > printConsoleFrequency) //si aggiorna rapidissimamente, per debug mi faccio stampare solo ogni 2 secondi
                {
                    //Debug.Log(" Joint numero = 1" + MytrackingJoint);
                    

                    Pose prova = ToWorldPose(MytrackingJoint, origin);
                    //Debug.Log(" ***** trasformata world ****** -->" + prova);

                    if (MytrackingJoint.TryGetPose(out Pose pose)) //per estrarre pose del singolo joint
                    {
                       // Debug.Log("unity Joint Position : " + pose.position);
                       // Debug.Log("unity Joint Rotation : " + pose.rotation);

                       
                        // CONVERTO IN FRAME DI ROS
                        Vector3<FLU> toRosPosition = pose.position.To<FLU>();
                        Quaternion<FLU> toRosRotation = pose.rotation.To<FLU>();

                    
                         //Debug.Log(" ################### AAA ##################### ");
                         //Debug.Log(" ++++  SENZA INVERTIRE XYZ Rotation (RPY in GRADI): " + eulerRotation.x + ", " + eulerRotation.y + ", " + eulerRotation.z);
                         Debug.Log(" MANO DESTRA ROSPosition : x =  " + toRosPosition.x + " y = " + toRosPosition.y + " z = " + toRosPosition.z);
                         Debug.Log("  MANO DESTRA ROSrotation : x =  " + toRosRotation.x + " y = " + toRosRotation.y + " z = " + toRosRotation.z + " W = " + toRosRotation.w );
                         
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

