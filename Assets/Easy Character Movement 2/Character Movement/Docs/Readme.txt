Character Movement
------------------

VERSION 1.0.6

	- Added CharacterMovement State data. This is the data needed to sync across the network ensuring proper simulation continuity.

	- Added networking examples for Major Unity networking libraries like Photon Fusion, FishNet, Mirror, etc. Where both, the Photon Fusion and FishNet examples implement server authoritative movement with client side prediction and reconciliation.

	- Minor bug fixes.



VERSION 1.0.5

	- Greatly improved the 'Collide and Slide' algorithm. This is capable of correctly detecting and resolving overlaps found during the movement loop.

	- Added Use Flat Top option, when enabled will treat head collisions as if the character is using a shape with a flat top.

	- Exposed moving platform info through the movingPlatform property.

	- Minor bug fixes and improvements.



VERSION 1.0.4

	- Fixed a bug with SetDimensions and SetHeight functions not updating Collider size in editor.



VERSION 1.0.3

	- Minor bug fix.



VERSION 1.0.2

	- Added Assembly Definition.
	
	- Minor bug fixes.
 
	- Fixed. CharacterMovement.detectCollisions not re-enabling collider.



VERSION 1.0.1

	- Added a 'Getting Started with SimpleMove' guide and example.

	- Minor bugs fixed.



VERSION 1.0

	- Initial release.



DISCLAIMER & LEGAL INFORMATION

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
PARTICULAR PURPOSE.

YOU MAY NOT REDISTRIBUTE THIS SOURCE CODE IN WHOLE OR IN PART
WITHOUT WRITTEN CONSENT FROM THE CONTENT AUTHOR OR COPYRIGHT HOLDER.