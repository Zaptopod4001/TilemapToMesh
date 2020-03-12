using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

namespace Eses.TilemapToMesh
{

    // Copyright Sami S. 

    // use of any kind without a written permission 
    // from the author is not allowed.

    // DO NOT:
    // Fork, clone, copy or use in any shape or form.


    // NOTE: 
    // There are some defensive measures for path and filename
    // but test this script at your own risk

    public class TilemapToMesh : EditorWindow
    {

        string assetPath = "Assets/";
        string fileName = "mapMesh";
        bool isEnabled = false;
        GameObject targeMapGO;
        Tilemap targetMap;


        // Editor window init

        [MenuItem("Window/TilemapToMesh")]
        static void Init()
        {
            TilemapToMesh window = (TilemapToMesh)EditorWindow.GetWindow(typeof(TilemapToMesh));
            window.Show();
        }


        // Draw editor window

        void OnGUI()
        {
            GUILayout.Label("Source Tilemap:", EditorStyles.boldLabel);

            // Target object
            targeMapGO = (GameObject)EditorGUILayout.ObjectField(targeMapGO, typeof(GameObject));

            // Get tilemap
            if (targeMapGO != null)
            {
                targetMap = targeMapGO.GetComponentInChildren<Tilemap>();
            }
            else
            {
                targetMap = null;
            }


            using (var scope = new EditorGUILayout.VerticalScope("box"))
            {
                // Save path
                GUILayout.Label("Save Path:", EditorStyles.boldLabel);
                assetPath = GUILayout.TextField(assetPath);
                var path = Path.GetDirectoryName(assetPath);

                // File name 
                GUILayout.Label("File name:", EditorStyles.boldLabel);
                fileName = GUILayout.TextField(fileName);


                if (!AssetDatabase.IsValidFolder(path))
                {
                    EditorGUILayout.HelpBox("Not a valid asset folder path: Assets/folder/", MessageType.Warning);
                }

                if (string.IsNullOrEmpty(fileName))
                {
                    EditorGUILayout.HelpBox("Not a valid filename - do not use special characters or leave empty", MessageType.Warning);
                }


                // Enable button if has tilemap

                var disabled = (targetMap == null) || !AssetDatabase.IsValidFolder(path) || string.IsNullOrEmpty(fileName);

                EditorGUI.BeginDisabledGroup(disabled);

                GUILayout.Space(10);

                if (GUILayout.Button("Create Tilemap Mesh", GUILayout.Height(40)))
                {
                    var savePath = string.Concat(assetPath, "/", fileName, ".asset");
                    GetTilemapTiles(targetMap, savePath);
                }

                EditorGUI.EndDisabledGroup();

            }
        }


        static void GetTilemapTiles(Tilemap tilemap, string savePath)
        {
            // bounds of tilemap
            BoundsInt bounds = tilemap.cellBounds;

            // All tiles of map
            TileBase[] allTiles = tilemap.GetTilesBlock(bounds);

            // List Quads of tiles
            List<Vector3[]> quadData = new List<Vector3[]>();


            // Get all tiles, store corner 
            // locations of each tile
            for (int x = 0; x < bounds.size.x; x++)
            {
                for (int y = 0; y < bounds.size.y; y++)
                {
                    TileBase tile = allTiles[x + (y * bounds.size.x)];

                    if (tile != null)
                    {
                        // Create Quad data
                        Vector3 p0 = new Vector3(x + bounds.min.x, bounds.min.y + y);
                        Vector3 p1 = new Vector3(x + bounds.min.x, bounds.min.y + y + 1);
                        Vector3 p2 = new Vector3(x + bounds.min.x + 1, bounds.min.y + y + 1);
                        Vector3 p3 = new Vector3(x + bounds.min.x + 1, bounds.min.y + y);

                        quadData.Add(new Vector3[] { p0, p1, p2, p3 });
                    }
                }
            }

            TilemapShapeToMesh(quadData, savePath);
        }


        static void TilemapShapeToMesh(List<Vector3[]> quadData, string savePath)
        {
            // Create Gameobject
            GameObject gameObj = new GameObject();
            gameObj.AddComponent<MeshFilter>();
            gameObj.AddComponent<MeshRenderer>();


            //  name 
            var name = Path.GetFileNameWithoutExtension(savePath);
            gameObj.name = name;

            // Set material hack
            gameObj.GetComponent<MeshRenderer>().material = new Material(Shader.Find("Diffuse"));


            // Create mesh asset        
            Mesh mesh = new Mesh();

            Debug.Log("Path: " + savePath);
            AssetDatabase.CreateAsset(mesh, savePath);

            // Load mesh back
            Mesh meshAsset = (Mesh)AssetDatabase.LoadAssetAtPath(savePath, typeof(Mesh));



            // Assign mesh to meshfiler
            var mf = gameObj.GetComponent<MeshFilter>();
            mf.mesh = meshAsset;


            // Create triangles of mesh

            // All verts
            Vector3[] vertices = new Vector3[quadData.Count * 4];
            int vert = 0;


            // All tris
            int[] triVerts = new int[quadData.Count * 6];
            int tri = 0;


            // for each quad
            for (int q = 0; q < quadData.Count; q++)
            {
                // quad verts 0,1,2,3
                vertices[vert] = quadData[q][0];
                vertices[vert + 1] = quadData[q][1];
                vertices[vert + 2] = quadData[q][2];
                vertices[vert + 3] = quadData[q][3];

                // quad tri1
                triVerts[tri] = vert;
                triVerts[tri + 1] = vert + 1;
                triVerts[tri + 2] = vert + 2;

                // quad tri2
                triVerts[tri + 3] = vert + 2;
                triVerts[tri + 4] = vert + 3;
                triVerts[tri + 5] = vert + 0;

                // add
                vert = vert + 4;
                tri = tri + 6;
            }


            // Build mesh
            mesh.vertices = vertices;
            mesh.triangles = triVerts;

            // Update assets
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

    }

}

