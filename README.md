# Rhino Plugin for generating VR environment


Features:
- each data point is equivalent to a slice
- slice is generated
- guide is shifted one time-length to the left (perpendicular to view)
- slice is copied to guide layer
- boolean join (?) is performed on the guide and slice
- repeat
- (check if next data point is the same as the previous - if true reuse the same slice instead of generate)


Under development:

- smoothing data on movements (cuts out wobbles) - how would this work?
- reusing the same slices for repeated movements 
- or have incoming data a list of movements
