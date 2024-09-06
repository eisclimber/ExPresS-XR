using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Resetter : MonoBehaviour
{
    //Reference to Rope with the Target
    [SerializeField, Tooltip("Reference to the Rope Gameobject with the Target")]
    private GameObject Target;

    //-------------------------------------------------------
    //alter Movement
    [Header("----------------------------------------------------------------")]
    [Header("Movement alteration")]
    [Space(5)]

    [SerializeField, Tooltip("check to activate the new Movement")]
    private bool alterMovement;

    [SerializeField, Tooltip("Moving Speed of the Target")]
    private float speed = 1;

    [SerializeField, Tooltip("Moving direction of the Target -> change regarding the Placement")]
    private Vector3 direction = new Vector3(-1,0,0);

    [SerializeField, Tooltip("check if you want to change the Movement direction in the other direction !ATTENTION! Movement direction needs to be changed as well! -> changes Colliders accordingly")]
    public bool leftToRight;


  
    //-------------------------------------------------------
    [Header("----------------------------------------------------------------")]
    [Header("Bad Target?")]
    [Space(5)]

    //Bad Targets
    [SerializeField, Tooltip("check to generate a Bad Target, if you want just bad or just good targets, activate/deactivate this bool and do not activate the weightedTargets on the bottom!")]
    private bool badTarget;



    [Header("----------------------------------------------------------------")]
    [Header("Weighted Random for the Images")]
    [Space(5)]

    //weightedrandom for the images
    [SerializeField, Tooltip("check to use weightedRandom for the choice of images on the targets instead of just random(does not change if it is a good or Bad target, only the images on the target) NOTICE: Same amount of probabilities as elements, the probabilities must sum up to 1, they need to be ordered descending, images have to be ordered regarding their probabilities!")]
    private bool weightedImages;

    [Header("----------------------------------------------------------------")]
    [Header("Set images fo good and bad targets")]
    [Space(5)]


    //Good Targets
    [SerializeField, Tooltip("Images for Good Targets")]
    public Sprite[] good_images;
    [SerializeField, Tooltip("Probabilities of the images for the Good Target -> mind the notice at the weightedImages")]
    public float[] good_images_probabilities;

    //Bad Targets
    [SerializeField, Tooltip("Images for Bad Target")]
    public Sprite[] bad_images;
    [SerializeField, Tooltip("Probabilities of the images for Bad Target -> mind the notice at the weightedImages")]
    public float[] bad_images_probabilities;


    //-------------------------------------------------------
    [Header("----------------------------------------------------------------")]
    [Header("Weighted Random for Good/Bad Target")]
    [Space(5)]

    //weightedrandom for the apperance of bad targets
    [SerializeField, Tooltip("check to use weightedRandom to automate and alter the appearance of bad targets. weightedRandom will change the badTarget bool!")]
    private bool weightedTargets;
    [SerializeField, Tooltip("true(check) = badTarget, false(no check) = goodTarget. Remember the notice at weightedrandom, the one with the higher probability needs to be listed first -> change if desired")]
    private bool[] weightedBadGood;
    [SerializeField, Tooltip("probabilities for the good and bad targets -> mind the notice at the weightedrandom")]
    private float[] weightedBadGoodProb;

    //----------------------------------------------------------
    



    //spawnpoint for the rope depending on the direction of the movement
    private GameObject currentSpawnAnchor;
    private Vector3 spawn_position;
    private Vector3 spawn_rotation;
    private Quaternion spawn_Quaternion;


    private GameObject spawnedobject;
    private GameObject RopeAnchor;
    private GameObject Image;
    private GameObject Left_Post;
    private GameObject Right_Post;
    private Collider dummy;
    private bool firstdeclaration = true;


    // Start is called before the first frame update
    void Start()
    {
        //Reference to the Left and Right Posts
        Left_Post = gameObject.transform.parent.Find("Construct/Left_Post").gameObject;
        Right_Post = gameObject.transform.parent.Find("Construct/Right_Post").gameObject;

        //instantiate the first Target with a dummy collider
        SpawnNewRope(dummy);
        firstdeclaration = false;
    }
    
    public void SpawnNewRope( Collider other)
    {
        if (weightedTargets)
        {
            badTarget = weightedBadGood[gameObject.GetComponent<WeightedRandom>().DirectGetRandomObject(weightedBadGood.Length, weightedBadGoodProb)];
        }
        
        //activate/deactivate the invoke scripts on the post regarding the moving direction
        if (alterMovement && leftToRight)
        {
            currentSpawnAnchor = gameObject.transform.Find("Left_Anchor").gameObject;
            Left_Post.GetComponent<invoke>().isactive = false;
            Right_Post.GetComponent<invoke>().isactive = true;

        }
        else
        {
            currentSpawnAnchor = gameObject.transform.Find("Right_Anchor").gameObject;
            Left_Post.GetComponent<invoke>().isactive = true;
            Right_Post.GetComponent<invoke>().isactive = false;
        }

        //Instantiate new Rope
        spawn_position = currentSpawnAnchor.transform.position;
        spawn_rotation = currentSpawnAnchor.transform.eulerAngles;
        spawn_Quaternion.eulerAngles = spawn_rotation;

        if(badTarget)
        {
            spawnedobject = Instantiate(Target, spawn_position, spawn_Quaternion, gameObject.transform.parent.gameObject.transform);
            spawnedobject.transform.Find("ImageContainer").gameObject.tag = "BadTarget";
            Image = spawnedobject.transform.Find("ImageContainer/Canvas/Image").gameObject;
            //change bool regarding bool in inspector
            Image.GetComponent<RandomImage>().weightedRandom = weightedImages;
            Image.GetComponent<RandomImage>().ChangeImages(bad_images,bad_images_probabilities);

        }
        else
        {
            spawnedobject = Instantiate(Target, spawn_position, spawn_Quaternion, gameObject.transform.parent.gameObject.transform);
            spawnedobject.transform.Find("ImageContainer").gameObject.tag = "Target";
            Image = spawnedobject.transform.Find("ImageContainer/Canvas/Image").gameObject;
            Image.GetComponent<RandomImage>().weightedRandom = weightedImages;
            Image.GetComponent<RandomImage>().ChangeImages(good_images, good_images_probabilities);
        }

        //in the beginning there is no object to destroy
        if (!firstdeclaration)
        {
            //Destroy old Rope
            Destroy(other.gameObject.transform.parent.gameObject);
        }


        //change Movement according to the direction Vector
        if (alterMovement)
        {
            RopeAnchor = spawnedobject.transform.Find("RopeAnchor").gameObject;
            if (leftToRight)
            {
                RopeAnchor.GetComponent<MoveObject>().ChangeMovement(speed, direction);
            }
            RopeAnchor.GetComponent<MoveObject>().ChangeMovement(speed, direction);
        }


    }
}
