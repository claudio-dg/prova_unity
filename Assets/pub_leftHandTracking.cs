using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

using UnityEngine.XR.Hands;
using UnityEngine.XR.Management;
//camera offset 1.1176

using Unity.Robotics.ROSTCPConnector.ROSGeometry;

//To publish to ROS
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.UnityRoboticsDemo;

public class pub_leftHandTracking : MonoBehaviour
{
    private XRHand hand; // dichiarazione della variabile hand

    //per connessione ROS
    ROSConnection ros;
    public string topicName = "left_arm_frame_topic";
    public float publishMessageFrequency = 0.5f;//0.4f;//1.0f;//6.0f;//10.0f;//0.5f; //corrisponde a 1sec in teoria
    // Used to determine how much time has elapsed since the last message was published
    private float timeElapsed;

    //Pose previousPose; //NON usata Adesso
    Vector3<FLU> previousPosition = new Vector3<FLU>(0,0,0);
    //bool  newPosition = false; //NON usata Adesso
   // float threshold =  0.07f; //0.10f; con 0.10f funziona bene ..ma provo ad alzare threshold per aumentare rate di publishing e vedere se ottengo comportamento migliore

     
    // prove decremento threshold del 3 NOVEMBRE --> prima dei cambiamenti era 0.07f

    float threshold =  0.05f; 

    //public float printConsoleFrequency = 4.0f;
    //private float timeElapsedConsole;

    //per trasformare
    public Transform origin;
    
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

        // DECOMMENTA PER INIZIALIZZARE CONNECTION ROS
        //   /*
        // start the ROS connection
        ros = ROSConnection.GetOrCreateInstance();
        // inizializza publisher su topic left_arm_frame_topic che accetta msg di tipo  ...
        ros.RegisterPublisher<PosRotMsg>(topicName);
        //   */
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
        //per publishing rate
        timeElapsed += Time.deltaTime;

        //solo per debug stampa
       // timeElapsedConsole += Time.deltaTime;
      
     var MytrackingJoint = subsystem.leftHand.GetJoint(XRHandJointIDUtility.FromIndex(1)); //mettere i per stamparli tutti
                
                
                //Con questo stampa tutto : Pos//rot//LinVEl//AngVEl ecc
                if(timeElapsed > publishMessageFrequency)
                 //devo pubblicare se: 
                // 1) e' passato tempo predef da msg prec
                // 2) Pos nuova e' diversa da quella precedente
                // 3) altro?
                {
                    if (MytrackingJoint.TryGetPose(out Pose pose)) //per estrarre pose del singolo joint
                    {
                    
                        // CONVERTO IN FRAME DI ROS
                        Vector3<FLU> toRosPosition = pose.position.To<FLU>();
                        // dopo aver trasformato in coordinate rispetto a frame ros, converto rispetto al frame torso_lift_link
                        //dato che pos iniziali sono relative ad un frame posizionato circa sulla testa ma io le voglio relative al torso appunto
                        // quindi aggiungo offset su x e z 
                        float corrected_x = toRosPosition.x + 0.2f;
                        float corrected_z = toRosPosition.z + 0.1f;
                       
                        Quaternion<FLU> toRosRotation = pose.rotation.To<FLU>();

                        //verifica sia posizione nuova

                         PosRotMsg left_hand_frame = new PosRotMsg(corrected_x, toRosPosition.y, corrected_z, 
                                                                    toRosRotation.x, toRosRotation.y, toRosRotation.z, toRosRotation.w ); //nota qui inserisco corrected_x e corrected_z
                   
                        Vector3 tempNew = new Vector3(corrected_x, toRosPosition.y, corrected_z);
                        Vector3 tempOld = new Vector3(previousPosition.x, previousPosition.y, previousPosition.z);
                        float pos_offset = Mathf.Abs(Vector3.Distance(tempNew, tempOld));
                        Debug.Log(" DIFFERENZA POSIZIONI =   " + pos_offset );
                        
                        Debug.Log(" previous pos =   " + tempOld );

                        Debug.Log(" new pos =   " + tempNew );

                        

                        // Finally send the message to server_endpoint.py running in ROS

                        //qui metto check pos diversa da precedente --- nota...posso provare a considerare solo position e non pos e ROtation..
                        //in questo cASO escluderei i campi di pos della persona che ruota SOLAmente la mano mantenenendola nella stessa poszione
                        if( pos_offset > threshold )
                        {

                            //cancello momentaneamente connesione ROS --> decommenta riga successiva per pubblicare su ros
                            ros.Publish(topicName, left_hand_frame); 

                            previousPosition.x = corrected_x;
                            previousPosition.y = toRosPosition.y;
                            previousPosition.z = corrected_z;
                            Debug.Log("\n ## pubblicata pos NUOVA " + left_hand_frame +  " #####\n");
                            Debug.Log("\n ## pos originale pre offset =  " + toRosPosition );
                       

                        }
                        

                         timeElapsed = 0;
                    }

                    //timeElapsed = 0;
                }
               
     
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


