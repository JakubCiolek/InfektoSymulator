using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeatScript : MonoBehaviour
{
    public Vector3 seatPosition;

    public SpriteRenderer seat;
    public bool isOccupied;
    void Start()
    {
        isOccupied = false;
        seatPosition = new Vector3 (seat.transform.position.x, seat.transform.position.y, 0.0f);
    }


    public bool GetOccupation()
    {
        return isOccupied;
    }

    public void SetOccupation(bool newOccupation)
    {
        isOccupied = newOccupation;
    }

    public Vector3 GetSeatPosition()
    {
        isOccupied = true;
        return seatPosition;
    }
}