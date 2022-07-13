# SpotifyCacheClean
Clean spotify local files cache


# Problem
If you trying to edit, add/remove/rename some songs from your local songs folder (which is spotify tracking on) 
spotify will not apply the changes till you remove the cache (which is in user appdata) and re-fill songs folder.
All this actions is done by this program, have fun =D


# How to use
Find your own local files cache path of spotify (below), insert the path into 'SPOTIFY_CACHE_PATH' field
If your music doesn't in 'Music' folder (C:\Users\Music\) change 'USER_MUSIC_PATH' accroding your local music path.


# How to find spotify local files cache path
## Official version (from web)
C:\Users\*USERNAME*\AppData\Local\Spotify\ 
OR
C:\Users\*USERNAME*\AppData\Roaming\Spotify\Users\username-user\
## Microsoft Store version
press WIN+R, search for %appdata% -> \Local\Packages\Spotify..\LocalState\Spotify\Users\username-user\
for example: C:\Users\yonka\AppData\Local\Packages\SpotifyAB.SpotifyMusic_zpdnekdrzrea0\LocalState\Spotify\Users\d792kbzaa1z84viefr149injq-user\

