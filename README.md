# DynastyParser

Manga parser and "gluer" for Dynasty Scans

## Purpose

The initial purpose of this tool was to grab a series of images that often are a result of a
single-strip manga/manhwa chapter being cut up into pieces and glue them back together.
*Ouji-sama Nante Iranai* made me do it.

Eventually, I decided to also include a parser, since forementioned manga had well over 50
chapters cut up that way, and I'd rather automate the process.

## Features

* [x] Download all chapters of a selected manga
* [x] Glue selected images from a chapter into a single image

## Todo
* [ ] Change download location
* [ ] Change output file format
* [ ] Open multiple chapters at once in some built-in browser

## Usage

### Parse

Paste a link to the manga, press the button, wait. The application will become unresponsive,
just give it time.

### Glue

Drag&drop any number of images onto the window. The big button glues them all together,
the small button glues only the selected ones.

Images are glued one below another, in alphabetical (and numerical) order, so make sure the
files are named appropriately.

Glued image will appear in the source directory as `glued.png`

## Disclaimer

Remember to always read the TOS of a website you're scraping and adhere to it.
Be respectful of the server too, don't DOS it with massive amounts of requests.