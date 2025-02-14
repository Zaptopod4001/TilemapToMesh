# Tilemap to Mesh (Unity / C#)

![Tilemap to Mesh](/doc/tilemap_to_mesh.PNG)

## What is it?

A Unity Editor tool / Editor Window that can be used to convert a Unity Tilemap into Unity GameObject + Mesh Asset.

How to use:

* Open the window from menu: Window/TilemapToMesh 

* Pick a Tilemap object from the active open Scene

* Input a proper Asset folder path (you'll get a warning for incorrect path)

* Input a proper file name (you'll get a warning for incorrect path)

* "Create Tilemap Mesh" button should activate - press it to create the mesh

The resulting Mesh Asset will be created in the target folder. Also, a new GameObject will be added to current scene, and it will have a Mesh Filter with the created Mesh assigned and also a Mesh Renderer.


# Classes

## TilemapToMesh.cs
Editor script which creates the Tilemap to mesh window, also contains mesh creation code. I won't warn more about using this script as you shouldn't be using it.

# About
I created tilemap to mesh converter for myself, as I needed such functionality once.

# Copyright
Created by Sami S. use of any kind without a written permission from the author is not allowed. But feel free to take a look.
