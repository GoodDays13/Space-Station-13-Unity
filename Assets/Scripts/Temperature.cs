using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AtmosphericSimulation;

public class Temperature : MonoBehaviour
{
    public AtmosphericSimulation atmosphere;
    public float temperature;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        RoomAtmosphere atmos = atmosphere.GetRoomAtmosphere((Vector2Int)atmosphere.floor.WorldToCell(transform.position));
        float targetTemp = atmos.temperature;
        float pressure = atmos.Pressure;
        temperature += (targetTemp - temperature) * Time.fixedDeltaTime / 60 * pressure;
    }
}
