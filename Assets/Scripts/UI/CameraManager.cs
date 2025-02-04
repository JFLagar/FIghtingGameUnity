using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SkillIssue.CharacterSpace;

public class CameraManager : MonoBehaviour
{
    private Camera cam;
    public Character[] characters;
    public Vector3 pos = new Vector3(0,0,-10);
    public float distance;
    public float middle;
    private float center;
    public bool check = false;
    public bool check2 = false;
    public float currentScreenEdge = 5;
    private void Awake()
    {
        cam = GetComponent<Camera>();
    }
    // Start is called before the first frame update
    void Start()
    {
        pos.y = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        middle = characters[0].transform.position.x + (characters[1].transform.position.x - characters[0].transform.position.x) / 2;
        if (Check())
        {
            center = transform.position.x;
        }
        else
        {
            center = middle;
        }     

        CameraMove();
        
    }
    public bool Check()
    {
        if (!check && !check2)
            return false;
        if (check && center < middle)
            return false;
        if (check2 && center > middle)
            return false;
        return true;
    }
    void CameraZoom()
    {
        if (Check())
        {
            return;
        }
        distance = Mathf.Abs(characters[0].transform.position.x - characters[1].transform.position.x);
        cam.orthographicSize = distance / 2;
        if (cam.orthographicSize < 1.25)
        {
            cam.orthographicSize = 1.25f;
        }
        if (cam.orthographicSize > 1.75)
        {
            cam.orthographicSize = 1.75f;
        }
    }
    void CameraMove()
    {  
        pos.x = center;
        gameObject.transform.position = pos;
    }

}
