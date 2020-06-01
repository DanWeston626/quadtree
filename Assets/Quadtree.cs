using System.Collections.Generic;
using UnityEngine;

namespace core {

    /// <summary>
    /// Rectangle used for representing Quadtree data
    /// </summary>
    public class Rectangle {

        /// <summary>
        /// Height of the rectangle 
        /// </summary>
        public readonly float height;

        /// <summary>
        /// Width of the rectangle
        /// </summary>
        public readonly float width;

        /// <summary>
        /// The x pos of the upper left corner
        /// </summary>
        public float x;

        /// <summary>
        /// The y pos of the upper left corner
        /// </summary>
        public float y;

        public Rectangle(Vector2 position, float width, float height) : this(position.x, position.y, width, height) { }

        public Rectangle(float x, float y, float width, float height) {
            this.width = width;
            this.height = height;
            this.x = x;
            this.y = y;
        }

        /// <summary>
        ///  Create a bounding box which 
        ///  represents this Rectangle instance. 
        ///  Coords start at top left and go clockwise
        /// </summary>
        /// <returns>Bounding box representing Rectangle instance</returns>
        public Vector3[] Bounds() {
            float hw = width * .5f, hh = height * .5f;
            return new Vector3[4] {
            new Vector3(x, y), // bottom left
            new Vector3(x, y+height, 1), // top left
            new Vector3(x + width, y+height, 1), // top right
            new Vector3(x + width, y, 1), // bottom right
        };
        }

    }

    /// <summary>
    /// Abstract class for objects used in Quadtree.
    /// See objects field in Quadtree
    /// </summary>
    public abstract class QuadObject {
        /// <summary>
        /// Rectangle representation for this object. 
        /// Used for bounds checking in Quadtree algorithm.
        /// </summary>
        public Rectangle rect;
    }

    /// <summary>
    /// Quadtree data structure
    /// Reference: https://gamedevelopment.tutsplus.com/tutorials/quick-tip-use-quadtrees-to-detect-likely-collisions-in-2d-space--gamedev-374
    /// </summary>
    public class Quadtree {
        /// <summary>
        /// How many object each node will contain
        /// </summary>
        public static int objectPerQuad = 1;

        /// <summary>
        /// The deepest level recursed
        /// </summary>
        public static int maxDepth = 10;

        /// <summary>
        /// The current level of this quad
        /// </summary>
        private int level;

        /// <summary>
        /// Objects currently contained in tree
        /// </summary>
        private List<QuadObject> objects;

        /// <summary>
        /// The this.rect/rect area of this quad
        /// </summary>
        public readonly Rectangle rect;

        /// <summary>
        /// Nodes/quads of this tree instance
        /// </summary>
        public readonly Quadtree[] nodes;

        public Quadtree(int level, Rectangle rect) {
            this.level = level;
            this.objects = new List<QuadObject>();
            this.rect = rect;
            this.nodes = new Quadtree[4];
        }

        /// <summary>
        /// Clear the Quadtree
        /// </summary>
        public void clear() {
            objects.Clear();
            for (int i = 0; i < nodes.Length; ++i) {
                if (nodes[i] != null) {
                    nodes[i].clear();
                    nodes[i] = null;
                }
            }
        }

        /// <summary>
        /// Splits a node into 4 subnodes, 
        /// dividing the node into 4 equal parts
        /// </summary>
        public void split() {
            float subWidth = rect.width * .5f;
            float subHeight = rect.height * .5f;
            float x = rect.x;
            float y = rect.y;

            nodes[0] = new Quadtree(level + 1, new Rectangle(x + subWidth, y, subWidth, subHeight)); // top right 
            nodes[1] = new Quadtree(level + 1, new Rectangle(x, y, subWidth, subHeight)); // top left
            nodes[2] = new Quadtree(level + 1, new Rectangle(x, y + subHeight, subWidth, subHeight)); // bottom left
            nodes[3] = new Quadtree(level + 1, new Rectangle(x + subWidth, y + subHeight, subWidth, subHeight)); // bottom right
        }

        /// <summary>
        /// Returns the index of the passed rect 
        /// -1 means it's not found in the tree
        /// </summary>
        private int getIndex(QuadObject obj) {
            int index = -1;
            float verticalMidpoint = this.rect.x + (this.rect.width / 2);
            float horizontalMidpoint = this.rect.y + (this.rect.height / 2);

            // Object can completely fit within the top quadrants
            bool topQuadrant = (obj.rect.y < horizontalMidpoint && obj.rect.y + obj.rect.height < horizontalMidpoint);
            // Object can completely fit within the bottom quadrants
            bool bottomQuadrant = (obj.rect.y > horizontalMidpoint);

            // Object can completely fit within the left quadrants
            if (obj.rect.x < verticalMidpoint && obj.rect.x + obj.rect.width < verticalMidpoint) {
                if (topQuadrant) {

                    index = 1;
                } else if (bottomQuadrant) {
                    index = 2;
                }
            }
             // Object can completely fit within the right quadrants
             else if (obj.rect.x > verticalMidpoint) {
                if (topQuadrant) {
                    index = 0;
                } else if (bottomQuadrant) {
                    index = 3;
                }
            }

            return index;
        }

        /// <summary>
        /// Insert the object into the quadtree. If the node
        /// exceeds the capacity, it will split and add all
        /// objects to their corresponding nodes.
        /// </summary>        
        public void insert(QuadObject obj) {
            if (nodes[0] != null) {
                int index = getIndex(obj);

                if (index != -1) {
                    nodes[index].insert(obj);

                    return;
                }
            }

            objects.Add(obj);

            if (objects.Count > objectPerQuad && level < maxDepth) {
                if (nodes[0] == null) {
                    split();
                }

                int i = 0;
                while (i < objects.Count) {
                    int index = getIndex(objects[i]);
                    if (index != -1) {
                        nodes[index].insert(objects[i]);
                        objects.RemoveAt(i);
                    } else {
                        i++;
                    }
                }
            }
        }
        
        /// <summary>
        /// Return all objects that could collide with the given object 
        /// </summary>        
        public List<QuadObject> retrieve(List<QuadObject> returnObjects, QuadObject pRect) {
            int index = getIndex(pRect);
            if (index != -1 && nodes[0] != null) {
                nodes[index].retrieve(returnObjects, pRect);
            }

            returnObjects.AddRange(objects);

            return returnObjects;
        }
    }
}