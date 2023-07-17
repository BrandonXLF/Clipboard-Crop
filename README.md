![Farmer in a field next to crops with a clipboard](images/farmer.jpg)

# Clipboard Crop

**Clipboard Crop** is a Windows application written with .NET that allows you to easily crop images in your clipboard and to save them back to your clipboard.

## Main Features

* Loading an image from the clipboard or a file
* Cropping, rotating, and flipping the image
* Changing the brightness, contrast, and saturation
* Saving the image to the clipboard or a file

## Image

[<img src="images/demo.png" width="500">](images/demo.png)

## Building

Clipboard Crop is built using Visual Studio with .NET as well as NSIS. To produce a publishable version of the app, publish the main project in Visual Studio. Note this will automatically run `makensis NSIS.nsi` to create the installer.
