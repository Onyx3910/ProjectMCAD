using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    public float amountOfParallax;  // This is amount of parallax scroll. 

    private float _startingPos;     // This is the starting position of the sprites.
    private float _lengthOfSprite;  // This is the length of the sprites.

    private void Start()
    {
        _startingPos = transform.position.x;
        _lengthOfSprite = GetComponent<SpriteRenderer>().bounds.size.x;
    }



    private void Update()
    {
        var camPosition = Camera.main.transform.position;
        var temp = camPosition.x * (1 - amountOfParallax);
        var parallaxOffset = camPosition.x * amountOfParallax;

        transform.position = new Vector3(_startingPos + parallaxOffset, transform.position.y, transform.position.z);

        if (temp > _startingPos + (_lengthOfSprite / 2))
        {
            _startingPos += _lengthOfSprite;
        }
        else if (temp < _startingPos - (_lengthOfSprite / 2))
        {
            _startingPos -= _lengthOfSprite;
        }
    }
}
