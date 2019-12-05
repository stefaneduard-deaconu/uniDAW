# University course for Developing a Web Application (DAW)

## The course revolves around..

Using .NET and Visual Studio 2017 for creating a website

## Modelling and versioning

These are the rules used throughout the App Model:

1. An User has many Versions
2. Every Version is for a unique Article
3. Every Article has 1 or more Categories associated with it
4. Every Article has 0 or more chapters
5. Every Article is associated with a 'latest Version' that contains the introContent (otherwise, we can add  and include introContent in the ARticle, too)
6. Every chapter has a Gallery of images
7. Every image must either be associated with a Gallery, or with a version.
Every Version must have an associated Article - we don't create Versions on their own

    **Will a CachedChapter just give up on the Gallery? That means that if we go back some versions (The Moderator ca n do so), we'll need to remake the Gallery, but it will be empty. But we can keep the galleryId inside every Image, and when we move back a version the backend will fin the gallery and "bring back to life" the Gallery, as a Phoenix :D.**

    **For versioning:**
8. Each and every Image must have a unique imageId, and be associated either with a Version (in which case it will be displaced inside the introContent, and introImageId = imageId), either with a Gallery
9. As a continuation and clarification of point 9, every Gallery keeps all the images used from the creation of it's associated Chapter until the present.
10. If we will delete a chapter, the Gallery goes, but the images remain as to satisfy the versioning system.

    **About deleting Articles, Versions, and Galleries:**
11. When we delete an article, we don't keep neither the images, nor the CachedChapters, nor the Versions
12. IN the same way, when we are deleting a Version (like the most recent one), we don't keep the unnecessary Images, nor the associated CachedChapters
13. We only delete Galleries when their associated Chapter is deleted. So the deletion of a Chapted eliminates a Gallery and possibly Images.

## As a user... what can I do?
### 1. Unregistered User
- able to read any article
- able to edit any _unfrozen unprotected_ article

### 2. Registered User
- able to do anything an **unregistered user** does
- able to create an article
- able to edit any _unfrozen protected_ article
- able to delete own articles
- able to _protect/unprotect_ own _unfrozen_ articles (so that unregistered users cannot edit the article)

### 3. Moderator
- able to do anything a **registered user** does
- able to revert any _unfrozen_ article to older versions
- able to _protect/unprotect_ any _unfrozen_ article (so that unregistered users cannot edit the article)

### 4. Admin
- able to do anything a **moderator** does
- able to delete any article
- able to CRUD categories
- able to read users information
- able to edit users information
- able to delete users
- able to _freeze_ any article (so that only the admin can edit and revert changes)
