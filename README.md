# Terra

World of Tanks map off line viewer and battle design tool.

## Gerneral info

Writen in VB.net using Visual Studio 13.

# Sorta working!
There are many things that need to be sorted out.

Atlas maps for the terrain and buildings.

GMM deferred rendering.

Finish decrypting all the data to place itmes on the map including water on all maps.

I am going to have to go to LOD selection of the models. Terra is going to be to slow at rendering once the textures are added to the terrain and models. They have removed the merged models and now only use models split up to trash parts of them.

I am commiting this so Maxim can have a look at the code.



# Terra no longer works!
Do to the massive changes in the game engine and the new tree and terrain formats, Terra can no longer read any of the data.
After 5 years of work on and off, this project is dead.
I am not mad over it.. but a little frustrated.
It was a good learning experince of nothing else.
I learned how to do projected screen space decals (Used in Tank Exporter), Deferred Shading and mnay other things.


### Update: 3/7/17
I have cleaned a lot of the code and also fixed some rendering issues.
Render time is faster now.
In this folder is the MSI installer for the latest version.
I cant fix what I don't know about... let me know so I can fix problems.

SOME MAPS WONT LOAD! There are issues with Paris I can't fix.
Paris is the only map that is not square.. Its a odd size 10x12 or something.
Don't remember but my code can't handle non-square maps.
It was my understanding that the bigworld engine only worked in square maps.
This is proven now to be wrong.
Any missing maps can be added to the map_list.txt file.. Instructions are inside.
They may or may not load. Some maps use so much memory, I'm not sure how BigWorld loads them.
I added code all over the place to recover memory after its use but still some maps are too big!

The code is rough and needs cleaned up and well commented.
There are many unused functions and subs that are lingering
in the code. Entire Modules containing code I started on like
modDeferred.vb that are for setting up a FBO was started and never
used... yet.


I am looking for help with this project to clean it up and find ways
to speed up the rendering. A good start would be a getting all the 
shaders coded in 330 core. as it is now, its a hatbox of mixed versions.

The code is 100% mine other than the referenced DLLs.. Look in the Info.html
for information on what DLLs im using and who wrote them.

Contact me at mailto:thecooltool@hotmail.com if you are interested
in contributing.. or leave a comment.
