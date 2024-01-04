using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeatScript : MonoBehaviour
{
    public Vector3 seatPosition;

    public SpriteRenderer seat;
    public humanScript sittingHuman;
    void Start()
    {
        sittingHuman = null;
        seatPosition = new Vector3 (seat.transform.position.x, seat.transform.position.y, 0.0f);
    }


    public humanScript GetOccupation()
    {
        return sittingHuman;
    }

    public void ReserveSeat(humanScript newSittingHuman)
    {
        sittingHuman = newSittingHuman;
    }

    public void ReleaseSeat()
    {
        sittingHuman = null;
    }

    public Vector3 GetSeatPosition()
    {
        return seatPosition;
    }
}