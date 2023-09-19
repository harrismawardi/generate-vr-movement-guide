# Rhino Plugin for generating VR movement guide

About:
This code base is an extension of a concept project I worked on in 2021. If you are interested in more details, the full design document can be found [here](https://github.com/harrismawardi/generate-vr-movement-guide/blob/0474afcdc798257eb945cdbee4f0d151da4fc2dc/A%20System%20For%20Mapping%20Yoga.pdf). 

For context, a demo of an earlier version of a visual guide for yoga can be found on my [vimeo](https://vimeo.com/866003907?share=copy)!
This plugin is the start of implementing a developed version of a 'visual guide' for VR users to complete yoga movements with good form. Instead of a wire being the guide as in the video, the virtual environment itself is the guide. By this I mean the environment is only visible at the point in space where the user should be looking when completing a movement with good form.

So far the plugin is able to take the point of the user and cut a slice of geometry - The first part of the guide. The next points of development are to do this for many points and join these together into a 'guide trail' that will scroll past the user.

Features:
- User prompt for selecting the start point of the movement
- User prompt for selecting a solid as the source geometry
- Cutting planes are generated at the point selected
- Source geometry is 'split' with cutting planes to generate an element of the guide geometry
- Cutting planes and guide geometry are organised by being assigned to new layers.


Upcoming Development:
- Read JSON file to supply mocked movement data
- Iterate through multiple data points to generate a guide geometry for each data point
- Offset the generated guide element tangential to camera orientation
- Boolean join on generated guide geometries
- Add data smoothing to enable checking if movement position/orientation is unchanged
- Reuse previously generated guide element if movement is unchanged 

Blockers:
- The version of .Net (4.8) does not include the System library for reading JSON files. Currently, the cutting planes are generated with hardcoded values.
