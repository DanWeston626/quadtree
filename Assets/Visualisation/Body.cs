using core;
using UnityEngine;

namespace game {
    public class Body : QuadObject {
        /// <summary>
        /// The game bounds where the body can traverse
        /// </summary>
        Vector2 bounds;

        /// <summary>
        /// Current direction of travel
        /// </summary>
        Vector2 direction;

        /// <summary>
        /// Current speed of travel
        /// </summary>
        float speed;

        public Body(Rectangle rect, Vector2 bounds) {
            this.rect = rect;
            this.bounds = bounds;
            // assign radom direction
            direction = new Vector2(Random.value > .5f ? 1.0f : -1.0f, Random.value > .5f ? 1 : -1);
            // assign radom speed
            speed = Random.Range(2.0f, 2.5f);
        }

        public void Update() {
            // translate
            Vector2 pos = new Vector2(rect.x, rect.y);
            pos += direction * (speed * Time.deltaTime);
            rect.x = pos.x; rect.y = pos.y;

            // bounds check x-axis
            if (pos.x <= 0) {
                direction.x = 1f;
            }
            if (pos.x >= (bounds.x)) {
                direction.x = -1f;
            }

            // bounds check y-axis
            if (pos.y <= 0) {
                direction.y = 1f;
            }
            if (pos.y >= (bounds.y)) {
                direction.y = -1f;
            }
        }

    }
}