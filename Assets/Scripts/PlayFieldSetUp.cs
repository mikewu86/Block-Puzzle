﻿using ScriptableObjectArchitecture;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayFieldSetUp : MonoBehaviour
{
    [SerializeField] private float xDist;
    [SerializeField] private float yDist;
    [SerializeField] private GameObject squareField;
    private List<TileObject> tileList = new List<TileObject>();
    public float xSquareField;
    public float ySquareField;
    public GameObject greenArea;
    [SerializeField] private Vector2 position;
    [SerializeField] private Vector2 basePosition;
    [SerializeField] private float pieceDistScale;

    public BlockDragGameEvent dragListen;
    public BlockDragGameEvent placeListen;
    [SerializeField] private List<TileObject> tileToCheck = new List<TileObject>();
    
    
    private void OnEnable()
    {
        dragListen.AddListener(Check);
        placeListen.AddListener(Place);
    }

    private void OnDisable()
    {
        dragListen.RemoveListener(Check);
        placeListen.RemoveListener(Place);  
    }

    public TileObject tim(Vector2 pos)
    {
        for(int i = 0; i < tileList.Count; ++i)
        {
            if(tileList[i].position == pos)
            {
                return tileList[i];
            }
        }
        return null;
    }

    public void Check(BlockDrag drag)
    {
        GameObject basePiece = drag.gameObject.transform.GetChild(0).gameObject;
        for (int i = 0; i < tileList.Count; i++)
        {
            GameObject ground = tileList[i].gameObject;
            if (drag.lastPos != drag.curPos)
            {
                foreach (TileObject tile in tileToCheck)
                {
                    if(tile.isEmpty())
                    {
                        tile.UnHover();
                    }
                }
                tileToCheck.Clear();
                drag.lastPos = drag.curPos;
                drag.gameObject.GetComponent<BlockDrag>().check = false;
                drag.gameObject.GetComponent<BlockDrag>().hovering = false;
            }
            if (ground.GetComponent<BoxCollider2D>().bounds.Contains(basePiece.transform.position))
            {
                if(!tileToCheck.Contains(tileList[i]))
                {
                    tileToCheck.Add(tileList[i]);
                }
                for (int j = 1; j < drag.gameObject.GetComponent<BlockDisplay>().points.Count; j++)
                {
                    Vector2 tempPoint = drag.gameObject.GetComponent<BlockDisplay>().points[j];
                    Vector2 pointToCheck = tileList[i].position + tempPoint;
                    TileObject tile = tim(pointToCheck);
                    if (tile != null && !tileToCheck.Contains(tile))
                    {
                        tileToCheck.Add(tile);
                    }
                }
                if (tileToCheck.Count > 0)
                {
                    int checkAmount = 0;
                    foreach (TileObject tile in tileToCheck)
                    {
                        if (tile.isEmpty())
                        {
                            checkAmount++;
                        }
                        if (checkAmount >= drag.gameObject.transform.GetComponent<BlockDisplay>().activeChild)
                        {
                            drag.gameObject.GetComponent<BlockDrag>().check = true;
                            drag.gameObject.GetComponent<BlockDrag>().hovering = true;
                        }
                    }
                    if (drag.gameObject.GetComponent<BlockDrag>().hovering)
                    {
                        foreach (TileObject tile in tileToCheck)
                        {
                            tile.Hovering(basePiece.GetComponent<PieceDisplay>().data);
                        }
                    }
                }
            }
        }
    }

    public void Place(BlockDrag drag)
    {
        if (drag.gameObject.GetComponent<BlockDrag>().check)
        {
            foreach (TileObject tile in tileToCheck)
            {
                tile.addPieceData(drag.transform.GetChild(0).GetComponent<PieceDisplay>().data);
            }
        }
        tileToCheck.Clear();
    }   

    void Start()
    {
        Vector2 topLeft = new Vector2(greenArea.transform.position.x - greenArea.GetComponent<SpriteRenderer>().size.x/2,
            greenArea.transform.position.y + greenArea.GetComponent<SpriteRenderer>().size.y/2);
        xSquareField = squareField.GetComponent<BoxCollider2D>().size.x / 2;
        ySquareField = squareField.GetComponent<BoxCollider2D>().size.y / 2;
        basePosition = topLeft + new Vector2(xSquareField + 0.1f, -ySquareField - 0.1f);
        position = basePosition;
        int square_number = 0;
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                GameObject newSquare = Instantiate(squareField, transform);
                square_number++;
                newSquare.name = "Square" + square_number;
                TileObject temp = newSquare.GetComponent<TileObject>();
                tileList.Add(temp);                
                newSquare.transform.position = position;
                temp.position = new Vector2(j % 8, -i);
                position.x += xDist;
            }
            position.y -= yDist;
            position.x = basePosition.x;
        }
    }

    void Update()
    {
        
    }
}
