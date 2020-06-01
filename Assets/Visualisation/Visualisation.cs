using core;
using System.Collections.Generic;
using ui;
using UnityEngine;

namespace game {
    public class Visualisation : MonoBehaviour {

        /// <summary>
        /// Number of points to generate
        /// </summary>
        public int numPoints = 50;

        /// <summary>
        /// Dummy body data
        /// </summary>
        List <Body> bodies = new List<Body>();

        /// <summary>
        /// Quadtree instance
        /// </summary>
        private Quadtree quadtree;

        /// <summary>
        /// Pause/play state of visualisation
        /// </summary>
        private bool paused = false;

        private void Start() {
            InitQuadtree();
            for (int i = 0, counti = numPoints; i < counti; ++i) {
                GenerateBodies();
            }

            RegisterDebug();
        }

        private void Update() {
            if (paused) return;

            // empty the tree
            quadtree.clear();
            // update and re-add all the bodies
            foreach (var x in bodies) {
                x.Update();
                quadtree.insert(x);
            }
        }

        /// <summary>
        /// Create dummy body data
        /// </summary>
        private void GenerateBodies() {            
            float x = UnityEngine.Random.value, y = UnityEngine.Random.value;
            Vector2 pos = Camera.main.ViewportToWorldPoint(new Vector2(x, y));                
            bodies.Add (new Body(new Rectangle(pos, 0.1f, 0.1f), new Vector2(quadtree.rect.width, quadtree.rect.height)));
            quadtree.insert(bodies[bodies.Count-1]);            
        }

        private void InitQuadtree () {
            // top right, bottom left coords of the camera in world space
            Vector2 topRight = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));
            Vector2 bottomLeft = Camera.main.ViewportToWorldPoint(new Vector2(0, 0));
            quadtree = new Quadtree(0, new Rectangle(0, 0, width: topRight.x - bottomLeft.x, height: topRight.y - bottomLeft.y));
            // move the camera such that 0,0 is in the bottom left hand corner
            Camera.main.transform.position = new Vector3(quadtree.rect.width * .5f, quadtree.rect.height * .5f);
        }

        /// <summary>
        /// Set up debug UI, register variables for tweaking
        /// </summary>
        private void RegisterDebug () {
            DebugController dc = DebugController.instance;
            #region Bodies 
            dc.Create(dc.slider).Init(ident: "Bodies", value: numPoints, min: 5, max: 500,
                onValueChanged: (int x) => {
                    int newCount = x;
                    if (newCount > bodies.Count) {
                        for (int i = 0, counti = newCount; i < counti; ++i) {
                            GenerateBodies();
                        }
                    } else if (newCount < bodies.Count) {
                        quadtree.clear();
                        for (int i = bodies.Count-1, counti = newCount; i >= counti; --i) {
                            bodies.RemoveAt(i);
                        }
                    }
                }
            );
            #endregion

            #region Quadtree Recursion
            dc.Create(dc.slider).Init(ident: "Quadtree Depth", value: Quadtree.maxDepth, min: 1, max: 100,
                onValueChanged: (int x) => {
                    Quadtree.maxDepth = x;
                    quadtree.clear();
                }
            );
            #endregion

            #region Quadtree Object Limit
            dc.Create(dc.slider).Init(ident: "Objects Per Quad", value: Quadtree.objectPerQuad, min: 1, max: 100,
                onValueChanged: (int x) => {
                    Quadtree.objectPerQuad = x;
                }
            );
            #endregion

            #region Pause/Play
            dc.Create(dc.button).Init(ident: "Pause", onClick: (DebugButton button) => {
                paused = !paused;
                button.ident.text = paused ? "Play" : "Pause";
            });
            #endregion
        }

        /// <summary>
        /// Swiped from Unity Docs
        /// </summary>
        static Material lineMaterial;
        static void CreateLineMaterial() {
            if (!lineMaterial) {
                // Unity has a built-in shader that is useful for drawing
                // simple colored things.
                Shader shader = Shader.Find("Hidden/Internal-Colored");
                lineMaterial = new Material(shader);
                lineMaterial.hideFlags = HideFlags.HideAndDontSave;
                // Turn on alpha blending
                lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                // Turn backface culling off
                lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
                // Turn off depth writes
                lineMaterial.SetInt("_ZWrite", 0);
            }
        }

        /// <summary>
        /// Draw dummy body data
        /// </summary>
        private void DrawBodies() {
            GL.Begin(GL.QUADS);
            GL.Color(new Color(37 / 255f, 94 / 255f, 50 / 255f, 255 / 255f));
            for (int i = 0, counti = bodies.Count; i < counti; ++i) {
                Vector3[] bb = bodies[i].rect.Bounds();
                Vector3 pos = new Vector3(bodies[i].rect.x, bodies[i].rect.y, 0);
                //Matrix4x4 trans = Matrix4x4.Translate(pos);

                for (int y = 0, county = bb.Length; y < county; ++y) {
                    GL.Vertex(bb[y]);
                }
            }
            GL.End();
        }

        /// <summary>
        /// Draw Quadtree. Warning this is a recursive 
        /// method which will recurse over the Quadtree. 
        /// Rendering each quad stored in the structure
        /// </summary>
        public void DrawTree(Quadtree t) {
            if (t == null) return;
            GL.Begin(GL.LINES);
            GL.Color(new Color(126 / 255f, 145 / 255f, 142 / 255f, 255 / 255f));
            Vector3[] bb = t.rect.Bounds();
            Vector3 pos = new Vector3(0, quadtree.rect.height / 2, 0);

            GL.Vertex(bb[0]);
            GL.Vertex(bb[1]);

            GL.Vertex(bb[1]);
            GL.Vertex(bb[2]);

            GL.Vertex(bb[2]);
            GL.Vertex(bb[3]);

            GL.Vertex(bb[3]);
            GL.Vertex(bb[0]);

            GL.End();

            for (int i = 0, counti = t.nodes.Length; i < counti; ++i) {
                DrawTree(t.nodes[i]);
            }
        }

        public void OnRenderObject() {
            GL.ClearWithSkybox(true, Camera.main);
            
            CreateLineMaterial();            
            lineMaterial.SetPass(0);

            GL.PushMatrix();
            GL.MultMatrix(transform.localToWorldMatrix);
            DrawTree(quadtree);
            DrawBodies();
            GL.PopMatrix();
        }
    }
}