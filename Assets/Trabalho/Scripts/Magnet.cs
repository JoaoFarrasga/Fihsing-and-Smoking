using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using System.Collections.Generic;
using UnityEngine;

public class Magnet : MonoBehaviour
{
    public GameObject magnet;
    public float pressCooldown = 1.0f;
    public float pullSpeed = 150f;

    public Transform pullPosition;

    public List<Point> points = new();

    public void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Peixe") && points.Count < 1)
        {
            var point = other.gameObject.GetComponent<Point>();
            point.GetPickedUP(pullPosition, pullSpeed);
            points.Add(point);
            other.gameObject.GetComponentInChildren<DistanceHandGrabInteractable>().MaxInteractors = -1;
        }
    }
}