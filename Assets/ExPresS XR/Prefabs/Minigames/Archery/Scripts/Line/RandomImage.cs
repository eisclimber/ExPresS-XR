using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomImage : MonoBehaviour
{
    [SerializeField]
    public Sprite[] images;

    [SerializeField, Tooltip("Give probabilities if you want to use the weightedRandom -> Same amount of probabilities as elements, they have to sum up to 1, need to be ordered descending, images have to be ordered regarding their probabilities!")]
    public float[] probabilites;

    [SerializeField,Tooltip("Reference to the Image on the Canvas")]
    private Image image_on_Canvas;
    [SerializeField, Tooltip("check for 'WeightedRandom'")]
    public bool weightedRandom;

    
    //Put random image on Canvas at start
    void Start()
    {
        if (weightedRandom)
        {
            image_on_Canvas.sprite = images[gameObject.GetComponent<WeightedRandom>().DirectGetRandomObject(images.Length, probabilites)];
            Debug.Log("weightedRandom");
        }
        else
        {
            image_on_Canvas.sprite = RandomElement();
        }
    }
    


    //Get a random Image of the Image Array
    private Sprite RandomElement()
    {
        int num = images.Length;
        int randomNumber = Random.Range(0, num);
        Debug.Log(randomNumber);
        Sprite img = images[randomNumber];
        return img;
    }

    //change images to the one given in the array
    public void ChangeImages(Sprite[] newimages, float[] newprobabilities)
    {
        images = newimages;
        probabilites = newprobabilities;
    }

}
