Some work I would like to have completed here includes :

moving url settings to the config file and fetching these where required
a set of unit tests for the method GetBestStoriesFromIds 

some improvements for performance :

cache best story detail records for the top scoring ones based on a max threshold which users would retrieve
determine when they are likely to be stale and fetch these at the required frequency
this would ideally be done by a job which runs overnight to push this data into the database
which would immensely speed up the retrieval 


The solution has been created using VS 2022
you can run as  dev  build and a swagger page will launch where you can enter the number of best stories required to fetch those

the Get method to run this is:
/HackerNews/{NumberOfStories}
