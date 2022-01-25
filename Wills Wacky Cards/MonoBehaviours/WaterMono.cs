using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using UnityEngine;
using UnboundLib;
using HarmonyLib;

namespace WaterMapObjects.MonoBehaviours
{
    public class LavaMono : WaterMono
    {
        private float _forceMult = 0.75f;
        public override float forceMult
        {
            get
            {
                return _forceMult;
            }
            set
            {
                _forceMult = value;
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
        private float _forceMult = 0.705f;
        public override float forceMult
        {
            get
            {
                return _forceMult;
            }
            set
            {
                _forceMult = value;
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

    public class SpaceMono : WaterMono
    {
        private float _forceMult = 0.7f;
        public override float forceMult
        {
            get
            {
                return _forceMult;
            }
            set
            {
                _forceMult = value;
            }
        }
        public override Color waterColor
        {
            get
            {
                return new Color(0.35f, 0.35f, 0.35f, 0.0275f);
            }
        }

        public override void HandlePlayer(Player player)
        {
            var data = player.data;

            var inSpace = data.gameObject.GetOrAddComponent<PlayerInSpace_Mono>();

            inSpace.inSpace = new bool[] { true, true };
        }

        public override void HandleBox(Rigidbody2D rb)
        {
            base.HandleBox(rb);

        }

        public override void HandleBullet(ProjectileHit projectileHit)
        {
            var inSpace = projectileHit.gameObject.GetOrAddComponent<BulletInSpace_Mono>();
            inSpace.inSpace = new bool[] { true, true };

        }
    }

    public class WaterMono : MonoBehaviour
    {
        //TrailRenderer trail;

        public RaycastHit2D[] hits;

        private float _forceMult = 0.7125f;
        public virtual float forceMult 
        {
            get
            {
                return _forceMult;
            }
            set
            {
                _forceMult = value;
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

            //trail = this.gameObject.AddComponent<TrailRenderer>();
            //trail.startColor = waterColor;
            //trail.endColor = waterColor;
            //trail.time = 0.02f;
            //trail.startWidth = 0.025f;
            //trail.endWidth = 0.025f;
        }

        //private void OnTriggerStay2D(Collider2D collider)
        //{
        //    //if (collider == this.gameObject.GetComponent<Collider2D>())
        //    //{
        //    //    return;
        //    //}

        //    HandleCollision(collider);
        //}

        private void FixedUpdate()
        {
            if (!this.gameObject.GetComponent<Renderer>())
            {
                return;
            }

            var tempHits = new List<RaycastHit2D>();

            var tempCorners = new Vector3[4];
            this.gameObject.GetOrAddComponent<RectTransform>().GetWorldCorners(tempCorners);
            Vector2[] waterCorners = tempCorners.Select(corner => (Vector2)corner).ToArray();

            //trail.AddPositions(tempCorners);
            //trail.AddPosition(tempCorners[0]);

            // Get all collisions going clockwise
            for (int i = 0; i < waterCorners.Length; i++)
            {
                if (i == 0)
                {
                    tempHits.AddRange(Physics2D.LinecastAll(waterCorners[waterCorners.Length-1], waterCorners[0]));
                }
                else
                {
                    tempHits.AddRange(Physics2D.LinecastAll(waterCorners[i - 1], waterCorners[i]));
                }
            }

            // Get all collisions going counterclockwise
            for (int i = 0; i < waterCorners.Length; i++)
            {
                if (i == 0)
                {
                    tempHits.AddRange(Physics2D.LinecastAll(waterCorners[0], waterCorners[waterCorners.Length-1]));
                }
                else
                {
                    tempHits.AddRange(Physics2D.LinecastAll(waterCorners[i], waterCorners[i-1]));
                }
            }

            hits = tempHits.ToArray();

            var colliders = Physics2D.OverlapBoxAll(this.transform.position, this.gameObject.GetComponent<RectTransform>().localScale, Vector2.SignedAngle(Vector2.up, this.transform.up.normalized));

            foreach (var collider in colliders)
            {
                HandleCollision(collider);
            }
        }

        public virtual void HandleCollision(Collider2D collider)
        {

            //trail.AddPositions(new Vector3[] { this.transform.position, collider.transform.position });

            // If our patch of water
            if (collider.gameObject == this.gameObject)
            {
                return;
            }

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
            /*
            1 - 2
            |   |
            0 - 3
            */

            // get the boxes of each object
            List<RaycastHit2D> hits = new List<RaycastHit2D>();
            var tempCorners = new Vector3[4];
            this.gameObject.GetOrAddComponent<RectTransform>().GetWorldCorners(tempCorners);
            Vector2[] waterCorners = tempCorners.Select(corner => (Vector2)corner).ToArray();

            rb.gameObject.GetOrAddComponent<RectTransform>().GetWorldCorners(tempCorners);
            Vector2[] rigidCorners = tempCorners.Select(corner => (Vector2)corner).ToArray();

            // Find out if any corners are in the water
            bool[] inWater = rigidCorners.Select(corner => PointIsInsideShape(waterCorners, corner)).ToArray();

            // If all of the corners are in the object, centroid is the centroid of the object.
            if (!inWater.Contains(false))
            {
                rb.AddForceAtPosition(Vector2.up * rb.mass * forceMult, rb.transform.position, ForceMode2D.Impulse);
                return;
            }

            // Find out if any of the water's corners are in the object
            bool[] inObject = waterCorners.Select(corner => PointIsInsideShape(rigidCorners, corner)).ToArray();

            // If all the corners of the water are in the object, centroid is center of water region
            if (!inObject.Contains(false))
            {
                rb.AddForceAtPosition(Vector2.up * rb.mass * forceMult, this.transform.position, ForceMode2D.Impulse);
                return;
            }

            // Get all contact points from our linecasts
            var contactPoints = hits.Where(hit => hit.collider.gameObject == rb.gameObject).Select(hit => hit.point).ToList();

            // Add any corners inside the water
            contactPoints.AddRange(rigidCorners.Where(corner => PointIsInsideShape(waterCorners, corner)));

            // If contact points for the object don't exist
            if (!(contactPoints.Count() > 0))
            {
                rb.AddForceAtPosition(Vector2.up * rb.mass * forceMult, rb.transform.position, ForceMode2D.Impulse);
                return;
            }

            //trail.AddPositions(contactPoints.Select(point => (Vector3) point).ToArray());

            // Centroid is (sum x / count x, sum y / count y)
            var centroid = new Vector2(contactPoints.Select(point => point.x).Sum() / contactPoints.Count(), contactPoints.Select(point => point.y).Sum() / contactPoints.Count());

            rb.AddForceAtPosition(Vector2.up * rb.mass * forceMult, centroid, ForceMode2D.Impulse);
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
    }

    [DisallowMultipleComponent]
    public class PlayerInWater_Mono : ModdingUtils.MonoBehaviours.ReversibleEffect
    {
        public bool[] hadWater = new bool[] { true, true };
        public override void OnStart()
        {
            this.characterStatModifiersModifier.jump_mult = 0.2f;
            this.characterStatModifiersModifier.movementSpeed_mult = 0.7f;
            this.gravityModifier.gravityForce_mult = 0f;

            player.GetComponent<Gravity>().enabled = false;

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
            player.GetComponent<Gravity>().enabled = true;

            data.currentJumps = data.jumps;
        }
    }

    [DisallowMultipleComponent]
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
            move.drag += 2;
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

    [DisallowMultipleComponent]
    public class PlayerInSpace_Mono : ModdingUtils.MonoBehaviours.ReversibleEffect
    {
        public bool[] inSpace = new bool[] { true, true };
        private float initialDrag;
        private float initialAngularDrag;
        public override void OnStart()
        {
            this.characterStatModifiersModifier.jump_mult = 0.25f;
            this.characterStatModifiersModifier.movementSpeed_mult = 0.1f;
            this.gravityModifier.gravityForce_mult = -0.001f;

            initialAngularDrag = player.data.movement.extraAngularDrag;
            initialDrag = player.data.movement.extraDrag;

            player.data.movement.extraDrag = 1;
            player.data.movement.extraAngularDrag = 1;

            ApplyModifiers();
        }

        public override void OnFixedUpdate()
        {
            if (inSpace[0] == false && inSpace[0] == inSpace[1])
            {
                UnityEngine.GameObject.Destroy(this);
            }
            else
            {
                if (inSpace[0])
                {
                    inSpace[0] = false;
                }
                else
                {
                    inSpace[1] = false;
                }
            }
        }

        public override void OnOnDestroy()
        {
            player.data.movement.extraDrag = initialDrag;
            player.data.movement.extraAngularDrag = initialAngularDrag;

            data.currentJumps = data.jumps;
        }
    }
    [DisallowMultipleComponent]
    public class BulletInSpace_Mono : MonoBehaviour
    {
        public bool[] inSpace = new bool[] { true, true };

        private MoveTransform move;
        private float initialGravity;

        private void Start()
        {
            move = this.gameObject.GetComponent<MoveTransform>();
            move.simulateGravity = 1;
        }

        private void FixedUpdate()
        {
            if (inSpace[0] == false && inSpace[0] == inSpace[1])
            {
                move.simulateGravity = 0;
                UnityEngine.GameObject.Destroy(this);
            }
            else
            {
                if (inSpace[0])
                {
                    inSpace[0] = false;
                }
                else
                {
                    inSpace[1] = false;
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