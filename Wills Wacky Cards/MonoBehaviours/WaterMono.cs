using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using UnityEngine;
using UnboundLib;

namespace WaterMapObjects.MonoBehaviours
{
    public class LavaMono : WaterMono
    {
        public override float forceMult
        {
            get
            {
                return 0.8f;
            }
        }
        public override Color waterColor
        {
            get
            {
                return new Color(1f, 0.15f, 0.15f, 0.05f);
            }
        }
        public override void HandlePlayer(Player player)
        {
            base.HandlePlayer(player);

            player.data.healthHandler.TakeDamageOverTime(Vector2.up * 0.65f, Vector2.zero, 2.5f, 1, new Color(1, 0, 0, 0.7f));
        }

        public override void HandleBox(Rigidbody2D rb)
        {
            base.HandleBox(rb);

            if (rb.GetComponent<DamagableEvent>())
            {
                rb.GetComponent<DamagableEvent>().CallTakeDamage(Vector2.up * 2, Vector2.zero);
            }
            else
            {
                var heatMono = rb.gameObject.GetOrAddComponent<BoxTouchingLava_Mono>();
                heatMono.heatPercent += 0.01f;
                heatMono.heatPercent = Mathf.Min(heatMono.heatPercent, 1f);
            }
        }

        public override void HandleBullet(ProjectileHit projectileHit)
        {
            base.HandleBullet(projectileHit);

            projectileHit.GetComponent<ProjectileCollision>().TakeDamage(10);
            projectileHit.damage -= 10;
            projectileHit.damage = Mathf.Max(projectileHit.damage, 1);
        }
    }

    public class AcidMono : WaterMono
    {
        public override float forceMult
        {
            get
            {
                return 0.725f;
            }
        }
        public override Color waterColor
        {
            get
            {
                return new Color(0.15f, 1f, 0.15f, 0.0275f);
            }
        }

        public override void HandlePlayer(Player player)
        {
            base.HandlePlayer(player);

            player.data.healthHandler.TakeDamageOverTime(Vector2.up * 0.45f, Vector2.zero, 5, 1, new Color(0, 1, 0, 0.7f));
        }

        public override void HandleBox(Rigidbody2D rb)
        {
            base.HandleBox(rb);

            if (rb.GetComponent<DamagableEvent>())
            {
                rb.GetComponent<DamagableEvent>().CallTakeDamage(Vector2.up * 2, Vector2.zero);
            }
            else
            {
                rb.gameObject.transform.localScale = new Vector3(rb.gameObject.transform.localScale.x * 0.9975f, rb.gameObject.transform.localScale.y * 0.9975f, rb.gameObject.transform.localScale.z);

                if (((Vector2)(rb.gameObject.transform.localScale)).magnitude <= (new Vector2(0.1f, 0.1f)).magnitude)
                {
                    UnityEngine.GameObject.Destroy(rb.gameObject);
                }
            }
        }

        public override void HandleBullet(ProjectileHit projectileHit)
        {
            base.HandleBullet(projectileHit);

            projectileHit.GetComponent<ProjectileCollision>().TakeDamage(10);
            projectileHit.damage -= 10;
            projectileHit.damage = Mathf.Max(projectileHit.damage, 1);
        }
    }

    public class WaterMono : MonoBehaviour
    {
        TrailRenderer trail;
        public virtual float forceMult 
        {
            get
            {
                return 0.75f;
            }
        }
        public virtual Color waterColor 
        { 
            get 
            { 
                return new Color(0.215f, 0.215f, 0.8f, 0.025f); 
            }
        }

        public void Start()
        {
            if (this.gameObject.GetComponent<Collider2D>())
            {
                UnityEngine.GameObject.Destroy(this.gameObject.GetComponent<Collider2D>());
            }

            //this.gameObject.layer = LayerMask.NameToLayer("BackgroundObject");
            //var coll = gameObject.GetOrAddComponent<BoxCollider2D>();
            //coll.isTrigger = true;

            //var rigid = gameObject.GetOrAddComponent<Rigidbody2D>();
            //rigid.isKinematic = true;
            //rigid.useFullKinematicContacts = true;

            this.gameObject.GetComponent<SpriteRenderer>().color = waterColor;

            this.gameObject.GetOrAddComponent<RectTransform>();

            trail = this.gameObject.AddComponent<TrailRenderer>();
            trail.startColor = waterColor;
            trail.endColor = waterColor;
            trail.time = 0.02f;
            trail.startWidth = 0.1f;
            trail.endWidth = 0.1f;
        }

        //private void OnTriggerStay2D(Collider2D collider)
        //{
        //    //if (collider == this.gameObject.GetComponent<Collider2D>())
        //    //{
        //    //    return;
        //    //}

        //    trail.AddPositions(new Vector3[] { this.transform.position, collider.transform.position});

        //    // If Player
        //    if (collider.GetComponent<Player>())
        //    {
        //        HandlePlayer(collider.GetComponent<Player>());

        //        return;
        //    }

        //    // If Bullet
        //    if (collider.GetComponentInParent<ProjectileHit>())
        //    {
        //        HandleBullet(collider.GetComponentInParent<ProjectileHit>());

        //        return;
        //    }

        //    // If Box
        //    if (collider.GetComponent<Rigidbody2D>())
        //    {
        //        HandleBox(collider.GetComponent<Rigidbody2D>());

        //        return;
        //    }
        //}

        private void FixedUpdate()
        {
            if (!this.gameObject.GetComponent<Renderer>())
            {
                return;
            }

            // Stupid raycast method to do a terrible job of brute forcing the intersecting area.
            if (true)
            {
                List<RaycastHit2D> hits = new List<RaycastHit2D>();
                var tempCorners = new Vector3[4];
                this.gameObject.GetOrAddComponent<RectTransform>().GetWorldCorners(tempCorners);
                Vector2[] waterCorners = tempCorners.Select(corner => (Vector2)corner).ToArray().OrderBy(corner => corner.x).ToArray();

                // the first corner will always be the leftmost
                var raySpacing = 0.05f < (waterCorners[3].x - waterCorners[0].x) / 100f ? 0.05f : (waterCorners[3].x - waterCorners[0].x) / 100f;
                for (float x = (waterCorners[0].x + raySpacing); x < waterCorners[3].x; x += raySpacing)
                {
                    Vector2[] line1 = new Vector2[2];
                    // line 1
                    if (x < waterCorners[1].x)
                    {
                        line1[0] = waterCorners[0];
                        line1[1] = waterCorners[1];
                    }
                    else
                    {
                        line1[0] = waterCorners[1];
                        line1[1] = waterCorners[3];
                    }

                    Vector2[] line2 = new Vector2[2];
                    // line 2
                    if (x < waterCorners[2].x)
                    {
                        line1[0] = waterCorners[0];
                        line1[1] = waterCorners[2];
                    }
                    else
                    {
                        line1[0] = waterCorners[2];
                        line1[1] = waterCorners[3];
                    }

                    var dir1 = (line1[1] - line1[0]).normalized;
                    var dir2 = (line2[1] - line2[0]).normalized;

                    var m1 = dir1.y / dir1.x;
                    var m2 = dir2.y / dir2.x;

                    var b1 = line1[0].y - m1 * line1[0].x;
                    var b2 = line2[0].y - m2 * line2[0].x;

                    var p1 = m1 * x + b1;
                    var p2 = m2 * x + b2;

                    hits.AddRange(Physics2D.RaycastAll((p1.y < p2.y ? p1 : p2), Vector2.up, Mathf.Abs(p1.y - p2.y)).ToList());
                    hits.AddRange(Physics2D.RaycastAll((p1.y > p2.y ? p1 : p2), Vector2.down, Mathf.Abs(p1.y - p2.y)).ToList());

                    trail.AddPositions(new Vector3[] { p1, p2 });
                }

                UnityEngine.Debug.Log($"{hits.Count()} hits found.");

                Dictionary<Collider2D, List<RaycastHit2D>> colliderHits = new Dictionary<Collider2D, List<RaycastHit2D>>();

                foreach (var hit in hits)
                {
                    if (colliderHits.ContainsKey(hit.collider))
                    {
                        colliderHits[hit.collider].Add(hit);
                    }
                    else
                    {
                        colliderHits.Add(hit.collider, new List<RaycastHit2D> { hit });
                    }
                }

                //colliderHits = hits.Select(hit => hit.collider).Distinct().ToDictionary(collider => collider, collider => hits.Where(hit => hit.collider == collider).ToList());

                foreach (var kvp in colliderHits)
                {
                    HandleRaycastCollision(kvp.Key, kvp.Value.ToArray());
                }
            }

            var colliders = Physics2D.OverlapBoxAll(this.transform.position, this.gameObject.GetComponent<RectTransform>().localScale, Vector2.SignedAngle(Vector2.up, this.transform.up.normalized));

            foreach (var collider in colliders)
            {
                HandleCollision(collider);
            }
        }

        public virtual void HandleCollision(Collider2D collider)
        {

            //trail.AddPositions(new Vector3[] { this.transform.position, collider.transform.position });

            // If Player
            if (collider.GetComponent<Player>())
            {
                HandlePlayer(collider.GetComponent<Player>());

                return;
            }

            // If Bullet
            if (collider.GetComponentInParent<ProjectileHit>())
            {
                HandleBullet(collider.GetComponentInParent<ProjectileHit>());

                return;
            }

            // If Box
            if (collider.GetComponent<Rigidbody2D>())
            {
                HandleBox(collider.GetComponent<Rigidbody2D>());

                return;
            }
        }

        public virtual void HandleRaycastCollision(Collider2D collider, RaycastHit2D[] hits)
        {

            trail.AddPositions(new Vector3[] { this.transform.position, collider.transform.position });

            // If Player
            if (collider.GetComponent<Player>())
            {
                HandlePlayer(collider.GetComponent<Player>());

                return;
            }

            // If Bullet
            if (collider.GetComponentInParent<ProjectileHit>())
            {
                HandleBullet(collider.GetComponentInParent<ProjectileHit>());

                return;
            }

            // If Box
            if (collider.GetComponent<Rigidbody2D>())
            {
                HandleBox(collider.GetComponent<Rigidbody2D>(), hits);

                return;
            }
        }

        public virtual void HandlePlayer(Player player)
        {
            var data = player.data;

            data.currentJumps = data.jumps;
            var inWater = data.gameObject.GetOrAddComponent<PlayerInWater_Mono>();

            inWater.hadWater = new bool[] { true, true };
        }

        public virtual void HandleBullet(ProjectileHit projectileHit)
        {
            var inWater = projectileHit.gameObject.GetOrAddComponent<BulletInWater_Mono>();
            inWater.hadWater = new bool[] { true, true };
        }

        public virtual void HandleBox(Rigidbody2D rb, RaycastHit2D[] hits)
        {
            if (hits.Length > 0)
            {
                Vector2 centroid = new Vector2(hits.Select(hit => hit.point.x).Sum() / hits.Length, hits.Select(hit => hit.point.y).Sum() / hits.Length);

                rb.AddForceAtPosition(Vector2.up * rb.mass * forceMult, centroid, ForceMode2D.Impulse);
            }
            else
            {
                rb.AddForce(Vector2.up * rb.mass * forceMult, ForceMode2D.Impulse);
            }
        }

        public virtual void HandleBox(Rigidbody2D rb)
        {
            ///*
            //1 - 2
            //|   |
            //0 - 3
            //*/

            //var tempCorners = new Vector3[4];
            //this.gameObject.GetOrAddComponent<RectTransform>().GetWorldCorners(tempCorners);
            //Vector2[] waterCorners = tempCorners.Select(corner => (Vector2) corner).ToArray();

            //tempCorners = new Vector3[4];
            //rb.gameObject.GetOrAddComponent<RectTransform>().GetWorldCorners(tempCorners);
            //Vector2[] rigidCorners = tempCorners.Select(corner => (Vector2)corner).ToArray();

            //bool[] inWater = rigidCorners.Select(corner => PointIsInsideShape(waterCorners, corner)).ToArray();

            //List<Vector2> inWaterShape = new List<Vector2>();

            //// Go through each corner
            //for (int i = 0; i < rigidCorners.Length; i++)
            //{
            //    // if the corner is in water
            //    if (inWater[i])
            //    {
            //        // Add to list
            //        inWaterShape.Add(rigidCorners[i]);

            //        // If the previous corner wasn't in water, get the intersection
            //        if ((i > 0 && !inWater[i - 0]) || (i == 0 && !inWater[inWater.Length]))
            //        {

            //        }
            //    }
            //    else if ((i > 0 && inWater[i - 1]) || (i == 0 && inWater[inWater.Length]))
            //    {
            //        var intersection = Vector2.zero;

            //        var x = 0;
            //        while (!LineSegmentsIntersectionWithPrecisonControl())
            //    }
            //}

            //// Stupid raycast method to do a terrible job of brute forcing the intersecting area.
            //{
            //    List<RaycastHit2D> hits = new List<RaycastHit2D>();
            //    var tempCorners = new Vector3[4];
            //    this.gameObject.GetOrAddComponent<RectTransform>().GetWorldCorners(tempCorners);
            //    Vector2[] waterCorners = tempCorners.Select(corner => (Vector2)corner).ToArray().OrderBy(corner => corner.x).ToArray();

            //    tempCorners = new Vector3[4];
            //    rb.gameObject.GetOrAddComponent<RectTransform>().GetWorldCorners(tempCorners);
            //    Vector2[] rigidCorners = tempCorners.Select(corner => (Vector2)corner).ToArray().OrderBy(corner => corner.x).ToArray();
            //    var rbMinX = rigidCorners.Select(corner => corner.x).Min();
            //    var rbMaxX = rigidCorners.Select(corner => corner.x).Max();

            //    // the first corner will always be the leftmost

            //}

            // Fallback method
            rb.AddForce(Vector2.up * rb.mass * forceMult, ForceMode2D.Impulse);
        }

        private bool PointIsInsideShape(Vector2[] outline, Vector2 point)
        {
            var result = true;
            List<int> results = new List<int>();
            for (int i = 0; i < outline.Length - 1; i++)
            {
                results.Add(isLeft(outline[i], outline[i+1], point));
            }

            if (results.Contains(1) && results.Contains(2))
            {
                result = false;
            }

            return result;
        }

        private int isLeft(Vector3 linePoint1, Vector3 linePoint2, Vector3 point){

            Vector3 lineVec = linePoint2 - linePoint1;
            Vector3 pointVec = point - linePoint1;

            float dot = Vector3.Dot(pointVec, lineVec);

            //point is on side of linePoint2, compared to linePoint1
            if (dot > 0)
            {

                //point is on the line segment
                if (pointVec.magnitude <= lineVec.magnitude)
                {

                    return 0;
                }

                //point is not on the line segment and it is on the side of linePoint2
                else
                {

                    return 2;
                }
            }

            //Point is not on side of linePoint2, compared to linePoint1.
            //Point is not on the line segment and it is on the side of linePoint1.
            else
            {

                return 1;
            }
        }

        /// <summary>
        /// Calculate the intersection point of two line segments
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <param name="p4"></param>
        /// <param name="intersection"></param>
        /// <param name="fSelfDefinedEpsilon">Epsilon in pixels</param>
        /// <returns></returns>
        public static bool LineSegmentsIntersectionWithPrecisonControl(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, ref Vector2 intersection, float fSelfDefinedEpsilon = 1.0f)
        {
            //Debug.Log (string.Format("LineSegmentsIntersection2 p1 {0} p2 {1} p3 {2} p4{3}", p1, p2, p3, p4)); // the float value precision in the log is just 0.0f
            UnityEngine.Assertions.Assert.IsTrue(fSelfDefinedEpsilon > 0);

            float Ax, Bx, Cx, Ay, By, Cy, d, e, f, num/*,offset*/;
            float x1lo, x1hi, y1lo, y1hi;
            Ax = p2.x - p1.x;
            Bx = p3.x - p4.x;

            // X bound box test/
            if (Ax < 0)
            {
                x1lo = p2.x; x1hi = p1.x;
            }
            else
            {
                x1hi = p2.x; x1lo = p1.x;
            }

            if (Bx > 0)
            {
                if ((x1hi < p4.x && Mathf.Abs(x1hi - p4.x) > fSelfDefinedEpsilon)
                    || (p3.x < x1lo && Mathf.Abs(p3.x - x1lo) > fSelfDefinedEpsilon))
                    return false;
            }
            else
            {
                if ((x1hi < p3.x && Mathf.Abs(x1hi - p3.x) > fSelfDefinedEpsilon)
                    || (p4.x < x1lo && Mathf.Abs(p4.x - x1lo) > fSelfDefinedEpsilon))
                    return false;
            }

            Ay = p2.y - p1.y;
            By = p3.y - p4.y;

            // Y bound box test//
            if (Ay < 0)
            {
                y1lo = p2.y; y1hi = p1.y;
            }
            else
            {
                y1hi = p2.y; y1lo = p1.y;
            }

            if (By > 0)
            {
                if ((y1hi < p4.y && Mathf.Abs(y1hi - p4.y) > fSelfDefinedEpsilon)
                    || (p3.y < y1lo && Mathf.Abs(p3.y - y1lo) > fSelfDefinedEpsilon))
                    return false;
            }
            else
            {
                if ((y1hi < p3.y && Mathf.Abs(y1hi - p3.y) > fSelfDefinedEpsilon)
                    || (p4.y < y1lo && Mathf.Abs(p4.y - y1lo) > fSelfDefinedEpsilon))
                    return false;
            }

            Cx = p1.x - p3.x;
            Cy = p1.y - p3.y;
            d = By * Cx - Bx * Cy;  // alpha numerator//
            f = Ay * Bx - Ax * By;  // both denominator//

            // alpha tests//

            if (f > 0)
            {
                if ((d < 0 && Mathf.Abs(d) > fSelfDefinedEpsilon)
                    || (d > f && Mathf.Abs(d - f) > fSelfDefinedEpsilon))
                    return false;
            }
            else
            {
                if ((d > 0 && Mathf.Abs(d) > fSelfDefinedEpsilon)
                    || (d < f && Mathf.Abs(d - f) > fSelfDefinedEpsilon))
                    return false;
            }
            e = Ax * Cy - Ay * Cx;  // beta numerator//

            // beta tests //

            if (f > 0)
            {
                if ((e < 0 && Mathf.Abs(e) > fSelfDefinedEpsilon)
                    || (e > f) && Mathf.Abs(e - f) > fSelfDefinedEpsilon)
                    return false;
            }
            else
            {
                if ((e > 0 && Mathf.Abs(e) > fSelfDefinedEpsilon)
                    || (e < f && Mathf.Abs(e - f) > fSelfDefinedEpsilon))
                    return false;
            }

            // check if they are parallel
            if (f == 0 && Mathf.Abs(f) > fSelfDefinedEpsilon)
                return false;

            // compute intersection coordinates //
            num = d * Ax; // numerator //

            //    offset = same_sign(num,f) ? f*0.5f : -f*0.5f;   // round direction //

            //    intersection.x = p1.x + (num+offset) / f;
            intersection.x = p1.x + num / f;
            num = d * Ay;

            //    offset = same_sign(num,f) ? f*0.5f : -f*0.5f;

            //    intersection.y = p1.y + (num+offset) / f;
            intersection.y = p1.y + num / f;
            return true;
        }
    }

    public class PlayerInWater_Mono : ModdingUtils.MonoBehaviours.ReversibleEffect
    {
        public bool[] hadWater = new bool[] { true, true };
        public override void OnStart()
        {
            this.characterStatModifiersModifier.jump_mult = 0.2f;
            this.characterStatModifiersModifier.movementSpeed_mult = 0.7f;
            this.gravityModifier.gravityForce_mult = 0f;

            ApplyModifiers();
        }

        public override void OnFixedUpdate()
        {
            if (hadWater[0] == false && hadWater[0] == hadWater[1])
            {
                UnityEngine.GameObject.Destroy(this);
            }
            else
            {
                if (hadWater[0])
                {
                    hadWater[0] = false;
                }
                else
                {
                    hadWater[1] = false;
                }
            }

            if (player.data.playerActions.Jump.IsPressed)
            {
                data.jump.Jump();
            }
        }

        public override void OnOnDestroy()
        {
            data.currentJumps = data.jumps;
        }
    }

    public class BulletInWater_Mono : MonoBehaviour
    {
        public bool[] hadWater = new bool[] { true, true };

        private MoveTransform move;
        private float initialDrag;
        private float initialMinDragSpeed;
        private float prevVel;
        private float timeEntered;
        private float maxTimeInWater = 1f;
        private void Start()
        {
            move = this.gameObject.GetComponent<MoveTransform>();
            prevVel = move.velocity.magnitude;
            timeEntered = Time.time;
            initialDrag = move.drag;
            initialMinDragSpeed = move.dragMinSpeed;
        }

        private void FixedUpdate()
        {
            move.drag += 1;
            move.dragMinSpeed = 0f;
            if (move.velocity.magnitude < prevVel)
            {
                prevVel = move.velocity.magnitude;
            }
            if (move.velocity.magnitude > prevVel)
            {
                move.velocity = Vector3.ClampMagnitude(move.velocity, prevVel);
            }

            move.velocity += Vector3.up * 0.5f;

            if (Time.time >= timeEntered + maxTimeInWater)
            {
                UnityEngine.GameObject.Destroy(this.gameObject);
            }

            if (hadWater[0] == false && hadWater[0] == hadWater[1])
            {
                move.drag = initialDrag;
                move.dragMinSpeed = initialMinDragSpeed;
                UnityEngine.GameObject.Destroy(this);
            }
            else
            {
                if (hadWater[0])
                {
                    hadWater[0] = false;
                }
                else
                {
                    hadWater[1] = false;
                }
            }
        }
    }

    public class BoxTouchingLava_Mono : MonoBehaviour
    {
        SpriteRenderer boxSprite;
        float heatDuration = 5f;
        Color initialColor;
        Color heatedColor = new Color(1f, 0f, 0f, 0.7f);
        public float heatPercent;

        private void Start()
        {
            boxSprite = GetComponent<SpriteRenderer>();
            initialColor = boxSprite.color;
        }

        private void Update()
        {
            boxSprite.color = new Color(initialColor.r + ((heatedColor.r - initialColor.r)* heatPercent), initialColor.g + ((heatedColor.g - initialColor.g) * heatPercent), initialColor.b + ((heatedColor.b - initialColor.b) * heatPercent), initialColor.a + ((heatedColor.a - initialColor.a) * heatPercent));

            if (heatPercent <= 0f)
            {
                UnityEngine.GameObject.Destroy(this);
            }
        }

        private void FixedUpdate()
        {
            var colliders = Physics2D.OverlapBoxAll(this.transform.position, this.gameObject.GetOrAddComponent<RectTransform>().localScale * 1f, Vector3.SignedAngle(Vector3.up, this.transform.up.normalized, Vector3.up));

            foreach (var collider in colliders)
            {
                if (collider.GetComponent<Player>())
                {
                    var player = collider.GetComponent<Player>();

                    player.data.healthHandler.TakeDamageOverTime(Vector2.up * 0.25f * heatPercent, Vector2.zero, 5f, 0.1f, new Color(1f, 0f, 0f, 0.7f));
                }
            }

            heatPercent -= Time.deltaTime / heatDuration;
        }

        private void OnDestroy()
        {
            boxSprite.color = initialColor;
        }
    }
}
