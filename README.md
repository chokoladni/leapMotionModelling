# leapMotionModelling
VR-enabled 3D modelling application using [Leap Motion](https://en.wikipedia.org/wiki/Leap_Motion) (now called Ultraleap).

## Modelling controls
### Vertex creation
A vertex can be created by bringing the index finger and thumb of any one hand together. Vertices are represented by spheres.
![vertex creation](https://github.com/chokoladni/leapMotionModelling/assets/19283862/d262c6fc-23d1-415a-8988-3f8b248f3112)

### Vertex selection
A vertex can be selected using the pointing gesture, as shown on the image. Selected vertices are colored in glowing yellow, while unselected vertices are colored white. Vertices can only be selected with the right hand, while the left hand is used to deselect them.
![vertex selection](https://github.com/chokoladni/leapMotionModelling/assets/19283862/11f6c1cd-1ee1-40e1-87bc-505b32378ef2)

### Triangle creation
Triangles can be created from selected vertices by pointing the open palm of any hand upwards. The order in which the vertices were selected plays an important role in the triangles' creation - behaviour imitates OpenGL's primitive TRIANGLE_STRIP, so the triangles are created from the following vertex indices: (1, 2, 3), (2, 3, 4), (3, 4, 5) ... (n-2, n-1, n), where n is the number of selected vertices. The operation results creates n-2 triangles (in the example below, n=5 and 3 triangles are created). 
![triangle creation](https://github.com/chokoladni/leapMotionModelling/assets/19283862/6ba5907b-34c0-4d7b-b13e-bb729d5aff6c)

### Triangle normal inversion
The normal of the triangle, i.e. its facing side, can be inverted using the "thumbs up" gesture of any hand. The front-facing side of the triangle is shown using the white color, while the backside of a triangle is indicated with darker gray color.
![image](https://github.com/chokoladni/leapMotionModelling/assets/19283862/baa04d04-a8c9-4d4a-8993-d18d523a0410)

### Vertex translation
Vertices can be translated by using the "grab and drag" gesture, as shown on the picture below. When the "grab" gesture is detected, all selected vertices will enter translation mode. The vertices will move with the hand for as long as the "grab" gesture is detected - once detection stops, translation will stop as well. If the moved vertices form any triangles, the triangles will be altered as well.
![image](https://github.com/chokoladni/leapMotionModelling/assets/19283862/c864a80c-ce35-4c45-9872-5b2ba19a9d39)

### Vertex deletion
Selected vertices can be deleted by using the "thumbs down" gesture of the left hand. All triangles which contained any of the deleted vertices are deleted as well.
![image](https://github.com/chokoladni/leapMotionModelling/assets/19283862/7600dc11-2ecf-4fef-b973-6c01c85be1dd)

### Triangle deletion
Triangles can be deleted by using the "thumbs down" gesture of the right hand. Only triangles whose all three vertices have been selected are deleted. After deletion, vertices remain and are automatically deselected.
![image](https://github.com/chokoladni/leapMotionModelling/assets/19283862/40dfe6be-a45d-49e5-9286-7d74e1108f93)

### Camera translation
In addition to VR headset movement, the camera can be translated in space by using the "grab" gesture of the left hand as well. Translation starts as soon as the gesture is detected, and ends once detection stops. The translation can be intuitively explained as if the user has grabbed a point in space and moving it around moves the whole world - bringing the hand closer to the VR headset will then move the camera closer to the grabbed point, while extending the arm will result in the camera being moved away from the grabbed point.
![image](https://github.com/chokoladni/leapMotionModelling/assets/19283862/98c34569-d611-4908-a446-45d968de45ee)
