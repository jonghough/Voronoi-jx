# Voronoi-jx
Generate Voronoi Diagrams in .NET / WPF

## Application
This is a library and application for drawing voronoi diagrams, using an implementation
of *Fortune's Algorithm*, written in C#. The application is a simple, single-window WPF
application. 
The actual library should be easily extractable from the application to drop into any
other application. The actual logic of the library was mostly copied from my Java app:
[JavaGraph](https://github.com/jonghough/JavaGraph)

## Purpose
Just a demonstration application and library.

## Dependencies
To generate the diagrams the C5 library's priority queue implementation was used.
[c5](http://www.itu.dk/research/c5/)

## Features
1. Draw simple diagrams with mouse click
2. Change background color
3. Change line color
4. Change vertex color
5. Draw Delaunay Edges (edges between all adjacent regions in the voronoi diagram)
6. Save canvas as image


## Samples

### voronoi diagram and corresponding delaunay edges
 ![voronoi1](/images/voronoi1.png)
 
### simple voronoi diagram
 ![voronoi2](/images/voronoi2.png)
 
### lots of points!
 ![voronoi3](/images/voronoi3.png)
 
### a little more complex
 ![voronoi4](/images/voronoi4.png)
 
 
