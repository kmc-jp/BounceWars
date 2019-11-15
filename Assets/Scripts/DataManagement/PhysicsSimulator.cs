using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsSimulator
{
    public static List<MapPhysicsMaterial> mapPhysicsMaterials;

    public static void SimulateIntegral( Unit u, float dt,MapBehaviour mapBehaviour)
    {
        float v = Mathf.Sqrt(u.vx * u.vx + u.vz * u.vz);
        if (v > 0)
        {
            Tile tile = mapBehaviour.GetTile(new Vector3(u.x, 10, u.z));
            float friction = 1;
//            Debug.Log(tile);
            if (tile != null)
            {
                friction = mapPhysicsMaterials[tile.type].friction;
            }
            float fx = -u.vx / v * friction;
            float fz = -u.vz / v * friction;
            float fxdt = fx * dt;
            float fzdt = fz * dt;
            float vx1 = u.vx + fxdt;
            float vz1 = u.vz + fzdt;
            if (vx1 * u.vx > 0)
            {
                u.vx = vx1;
                u.vz = vz1;
            }
            else
            {
                u.vx = 0;
                u.vz = 0;
            }
        }
        u.x1 += u.vx * dt;
        u.z1 += u.vz * dt;
    }
    public static void ApplyIntegral(Unit u)
    {
        u.x = u.x1;
        u.z = u.z1;
    }
    public static float UnitDistance1(Unit u1, Unit u2)
    {
        //Debug.Log(Mathf.Sqrt((u1.x1 - u2.x1) * (u1.x1 - u2.x1) + (u1.z1 - u2.z1) * (u1.z1 - u2.z1)));
        return Mathf.Sqrt((u1.x1 - u2.x1) * (u1.x1 - u2.x1) + (u1.z1 - u2.z1) * (u1.z1 - u2.z1));
    }
    public static bool CollideMap(Unit u,MapBehaviour mapBehaviour)
    {
        Vector2 pos = new Vector2(u.x1, u.z1);

        Tile t = mapBehaviour.GetTile(new Vector3(u.x1, 0, u.z1));
        if (t == null || t.buildingType <= 1)
        {
            return false;
        }
        Vector2 tPos = new Vector2(t.position.x, t.position.z);
        Vector2 diff = pos - tPos;
        if (diff.sqrMagnitude < 1)
        {
            Vector2 normal = diff.normalized;
            u.vx1 = u.vx1 - u.vx1 * normal.x * normal.x * 2;
            u.vz1 = u.vz1 - u.vz1 * normal.y * normal.y * 2;
            return false;
        }
        return true;
    }
    public static void SimulateCollision(List<Unit> targets,MapBehaviour mapBehaviour,Simulator simulator,List<bool> clean,
        List<CollisionInfo> infos)
    {
        //Debug.Log(targets[0].vx);

        for (int i = 0; i < targets.Count; i++)
        {
            clean.Add(true);
        }
        for (int i = 0; i < targets.Count; i++)
        {
            targets[i].vx1 = targets[i].vx;
            targets[i].vz1 = targets[i].vz;
        }
        for (int i = 0; i < targets.Count; i++)
        {
            for (int j = i; j < targets.Count; j++)
            {
                if (i == j) continue;
                Unit u1 = targets[i];
                Unit u2 = targets[j];
                float d = UnitDistance1(u1, u2);
                float dx = u2.x1 - u1.x1;
                dx /= d;
                float dz = u2.z1 - u1.z1;
                dz /= d;

                if (0 < d && d < 1)
                {
                    CollisionInfo collision = new CollisionInfo();
                    collision.me = u1;
                    collision.other = u2;
                    collision.vx1 = u1.vx;
                    collision.vz1 = u1.vz;
                    collision.vx2 = u2.vx;
                    collision.vz2 = u2.vz;
                    infos.Add(collision);


                    CollisionInfo collision1 = new CollisionInfo();
                    collision1.me = u2;
                    collision1.other = u1;
                    collision1.vx1 = u2.vx;
                    collision1.vz1 = u2.vz;
                    collision1.vx2 = u1.vx;
                    collision1.vz2 = u1.vz;
                    infos.Add(collision1);

                    float rvx = u2.vx - u1.vx;
                    float rvz = u2.vz - u1.vz;
                    float sizeVertical = dx * rvx + dz * rvz;//hiroaki strength of impulse

                    collision.normalVelocity = sizeVertical;
                    collision1.normalVelocity = sizeVertical;
                    if (u1.type == UnitType.TYPE_ARROW || u1.type == UnitType.TYPE_FIREBALL) // tinaxd u1 is arrow or fireball
                    {
                        u1.vx1 = u1.vx;
                        u1.vz1 = u1.vz;
                        u1.HP = 0;
                        u2.vx1 = u2.vx - dx * 0.05f * sizeVertical;
                        u2.vz1 = u2.vz - dz * 0.05f * sizeVertical;
                        clean[i] = false;
                        clean[j] = false;
                    }
                    else if (u2.type == UnitType.TYPE_ARROW || u2.type == UnitType.TYPE_FIREBALL) // tinaxd u2 is arrow or fireball
                    {
                        u1.vx1 = u1.vx + dx * 0.05f * sizeVertical;
                        u1.vz1 = u1.vz + dz * 0.05f * sizeVertical;
                        u2.vx1 = u2.vx;
                        u2.vz1 = u2.vz;
                        u2.HP = 0;
                        clean[i] = false;
                        clean[j] = false;
                    }
                    else if (UnitType.isItem(u2.type))
                    {
                        u1.vx1 = u1.vx;
                        u1.vz1 = u1.vz;
                        u2.vx1 = u2.vx;
                        u2.vz1 = u2.vz;
                        u2.HP = 0;
                    }
                    else if (UnitType.isItem(u1.type))
                    {
                        u1.vx1 = u1.vx;
                        u1.vz1 = u1.vz;
                        u2.vx1 = u2.vx;
                        u2.vz1 = u2.vz;
                        u1.HP = 0;
                    }
                    else // neither u1 nor u2 is arrow or fireball
                    {
                        u1.vx1 = u1.vx + dx * sizeVertical;
                        u1.vz1 = u1.vz + dz * sizeVertical;
                        u2.vx1 = u2.vx - dx * sizeVertical;
                        u2.vz1 = u2.vz - dz * sizeVertical;
                        clean[i] = false;
                        clean[j] = false;
                    }
                    //Debug.Log(string.Format("{0}:({1},{2}),({3},{4})({5})",Time.time,i,j,rvx,rvz,sizeVertical));
                    //Debug.Log(string.Format("{0} i={1}:({2},{3})",Time.time,i, u1.vx1, u1.vz1));


                }
            }
        }

        for (int i = 0; i < targets.Count; i++)
        {
            clean[i] = CollideMap(targets[i], mapBehaviour);
        }
        float E = 0;
        float px = 0;
        float pz = 0;
        for (int i = 0; i < targets.Count; i++)
        {
            Unit curUnit = targets[i];
            curUnit.vx = curUnit.vx1;
            curUnit.vz = curUnit.vz1;
            E += curUnit.vx * curUnit.vx + curUnit.vz * curUnit.vz;
            px += curUnit.vx;
            pz += curUnit.vz;

        }

        //        Debug.Log(targets[0].vx);
        //Debug.Log(string.Format("E={0},p=({1},{2})", E, px, pz));


        //        Debug.Log(targets[0].vx);
    }
}
