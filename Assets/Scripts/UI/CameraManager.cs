using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SkillIssue.CharacterSpace;

public class CameraManager : MonoBehaviour
{
     Camera cam;
    [SerializeField]
     Character[] characters;
     Vector3 pos = new Vector3(0,0,-10);
     float distance;
    [SerializeField]
     float middle;
    bool isOnScreenEdge = false;
    int screenEdgeFaceDir = 0;

    private void Awake()
    {
        cam = FindFirstObjectByType<Camera>();
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

        CameraMove();
        
    }

    public void SetWallDirection(int faceDirection)
    {
        isOnScreenEdge = true;
        screenEdgeFaceDir = faceDirection;
    }

    public int GetScreenEdgeFaceDir() 
    {
        return screenEdgeFaceDir;
    }

    private void CameraZoom()
    {
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

    private void CameraMove()
    {  
        if (!isOnScreenEdge || IsMovingAwayFromScreenEdge())
        pos.x = middle;
        cam.transform.position = pos;
    }

    private bool IsMovingAwayFromScreenEdge()
    {
        if (screenEdgeFaceDir == -1 && middle > pos.x)
        {
            isOnScreenEdge = false;
            return true;
        }
        if (screenEdgeFaceDir == 1 && middle < pos.x)
        { 
            isOnScreenEdge = false;
        
            return true;
        }
        return false;
    }    

}
