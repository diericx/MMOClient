using UnityEngine;
using System.Collections;

public class UpdatePacket
{
    public string Action { get; set; }
    public string Data { get; set; }
    public string ID { get; set; }
    public string Health { get; set; }
    public float X { get; set; }
    public float Y { get; set; }
    public int Rotation { get; set; }
    public int[] Gear { get; set; }
    public float[] BulletIDs { get; set; }
    public float[] BulletXs { get; set; }
    public float[] BulletYs { get; set; }
}