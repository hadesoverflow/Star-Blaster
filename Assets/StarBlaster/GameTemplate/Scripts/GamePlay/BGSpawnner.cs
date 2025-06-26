using UnityEngine;

namespace StarBlaster.GameTemplate.Scripts.GamePlay
{
    public class BgSpawnner : MonoBehaviour
    {
        public GameObject bgPrefab;
        public float scrollSpeed = 2f;
        public int initialCount = 3;

        private float bgHeight;
        private Transform[] bgs;
        private bool isPaused = false; // trạng thái pause

        void Start()
        {
            bgHeight = bgPrefab.GetComponent<SpriteRenderer>().bounds.size.y;

            bgs = new Transform[initialCount];

            for (int i = 0; i < initialCount; i++)
            {
                GameObject bg = Instantiate(bgPrefab, new Vector3(0, i * bgHeight, 0), Quaternion.identity, transform);
                bgs[i] = bg.transform;
            }
        }

        void Update()
        {
            if (isPaused) return;  // Nếu đang pause thì không update

            for (int i = 0; i < bgs.Length; i++)
            {
                bgs[i].Translate(Vector3.down * (scrollSpeed * Time.deltaTime));

                // Nếu BG đi ra khỏi màn hình -> reset lên trên
                if (bgs[i].position.y < -bgHeight)
                {
                    float highestY = GetHighestY();
                    bgs[i].position = new Vector3(0, highestY + bgHeight, 0);
                }
            }
        }

        float GetHighestY()
        {
            float maxY = float.MinValue;
            foreach (var bg in bgs)
            {
                if (bg.position.y > maxY)
                    maxY = bg.position.y;
            }
            return maxY;
        }

        // Hàm gọi khi pause game
        public void Pause()
        {
            isPaused = true;
        }

        // Hàm gọi khi resume game
        public void Resume()
        {
            isPaused = false;
        }
    }
}