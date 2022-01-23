using System;
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
                return 0.785f;
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

        public override void HandleBox(Rigidbody2D rb, Vector2 centroid)
        {
            base.HandleBox(rb, centroid); ;

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

        public override void HandleBox(Rigidbody2D rb, Vector2 centroid)
        {
            base.HandleBox(rb, centroid);

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
            //if (this.gameObject.GetComponent<Collider2D>())
            //{
            //    UnityEngine.GameObject.Destroy(this.gameObject.GetComponent<Collider2D>());
            //}

            var coll = this.gameObject.GetComponent<Collider2D>();
            var rigid = this.gameObject.GetOrAddComponent<Rigidbody2D>();
            rigid.isKinematic = true;
            //coll.isTrigger = true;

            this.gameObject.layer = LayerMask.NameToLayer("BackgroundObject");
            this.gameObject.GetComponent<SpriteRenderer>().color = waterColor;
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            UnityEngine.Debug.Log($"{collision.collider.gameObject.name} collided with {collision.otherCollider.gameObject.name}");
            var collider = collision.otherCollider;

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
                ContactPoint2D[] contacts = new ContactPoint2D[collision.contactCount];

                collision.GetContacts(contacts);

                var centroid = new Vector2(contacts.Select(contact => contact.point.x).Sum() / contacts.Length, contacts.Select(contact => contact.point.y).Sum() / contacts.Length);

                HandleBox(collider.GetComponent<Rigidbody2D>(), centroid);

                return;
            }
        }

        //private void FixedUpdate()
        //{
        //    if (!this.gameObject.GetComponent<Renderer>())
        //    {
        //        return;
        //    }

        //    var colliders = Physics2D.OverlapBoxAll(this.transform.position, this.gameObject.GetComponent<Renderer>().bounds.size, Vector3.SignedAngle(Vector3.up, this.transform.up.normalized, Vector3.up));

        //    foreach (var collider in colliders)
        //    {
        //        if (collider == this.gameObject.GetComponent<Collider2D>())
        //        {
        //            continue;
        //        }

        //        // If Player
        //        if (collider.GetComponent<Player>())
        //        {
        //            HandlePlayer(collider.GetComponent<Player>());

        //            continue;
        //        }

        //        // If Bullet
        //        if (collider.GetComponentInParent<ProjectileHit>())
        //        {
        //            HandleBullet(collider.GetComponentInParent<ProjectileHit>());

        //            continue;
        //        }

        //        // If Box
        //        if (collider.GetComponent<Rigidbody2D>())
        //        {
        //            HandleBox(collider.GetComponent<Rigidbody2D>());

        //            continue;
        //        }
        //    }
        //}

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

        public virtual void HandleBox(Rigidbody2D rb, Vector2 centroid)
        {
            rb.AddForceAtPosition(Vector2.up * rb.mass * forceMult, centroid, ForceMode2D.Impulse);

            //rb.AddForce(Vector2.up * rb.mass * forceMult, ForceMode2D.Impulse);
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
            var colliders = Physics2D.OverlapBoxAll(this.transform.position, this.gameObject.GetComponent<Renderer>().bounds.size * 1.35f, Vector3.SignedAngle(Vector3.up, this.transform.up.normalized, Vector3.up));

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
