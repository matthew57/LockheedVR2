//////////////// READ ME DOCUMENTATION ////////////////////////
/*
 This platform allows multiple users to tap into the same virtual workspace
 and jointly interact with a CAD model. 

 The CAD model of choice must be converted into an .FBX file format and imported
 into unity. Make sure this model is in the scene, and also dragged into the 
 CAD model slot on the ModelManager script which is attached to the Model Manager
 gameobject.

 Multi-user Interaction includes:

 *Grabbing Parts Tool - this is networked over multi-users

 *Cutting Plane Tool - this is networked over multi-users
  
 *Measuring Tool - this is NOT networked over multi-users (i.e. other players can't see your measuring line)
  
 *Orange Axis Rotation Line Tool - this is networked over multi-users 
  
 *Laser Pointer on Player Tool - this is networked over multi-users
  
 *Reset Model Tool - this is networked over multi-users
 
 
 */
