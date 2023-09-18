# Rhino Plugin for generating VR movement guide

About:
This code base is an extension of a concept project I worked on in 2021. If you are interested in more details go here.

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
