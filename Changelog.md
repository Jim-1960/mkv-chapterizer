
---

**2.4.0.0** (2012-05-06)

---

  * NEW:
    * Chapterize using an external chapter file
    * Use another "base" chapter when adding chapters from ChapterDB
    * Ability to only select a part of the chapters of a set from ChapterDB
    * Setting to remember Overwrite option between program starts
    * Setting to set default mode on program start-up

  * IMPROVED:
    * Updated SharpDate to 1.0.5.0
    * Updated mkvmerge and mkvextract to 5.5.0.0

  * CHANGED:
    * Renamed "Advanced" tab to "Queue"

  * FIXED:
    * When adding folder and selecting recurse scan, the base folder now gets scanned too


---

**2.3.1.0** (2012-04-01)

---

  * CHANGED:
    * Reimplemented so you can shift-click to only output generated chapters
    * Updated SharpDate to 1.0.3.0


---

**2.3.0.0** (2012-03-31)

---

  * NEW:
    * Now using SharpDate to update the application
    * Import/export list of mkvs
    * Scan full folders of mkvs to add
    * Extract chapters from mkvs in batch

  * IMPROVED:
    * A lot of code refactoring
    * More obvious that you can switch to Advanced mode (no more right-clicking)

  * CHANGED:
    * Logging system moved to separate project/dll

  * FIXED:
    * Status label no longer overlays other UI when it holds a long file name
    * Mkvs with capitalized extension are now droppable


---

**2.2.0.0**

---


  * Fixed bug where the auto updater crashed when unable to connect
  * Fixed some bugs with the chapter file mode
  * Changed the choosable chapter interval to 1 second up to 60 hours
  * Changed the way the program handles the UI (no visual change)


---

**2.1.1.0**

---


  * Added progressviewing when copying the finished file
  * Added possiblity to only output chapter files (Shift-click Chapterize in single mode)
  * Solved bug where the numbers of chapters would show 1 on first drop
  * More intense cleaning of the workfolder
  * Fixed portable mode detection (Auto update didn't download file)


---

**2.1.0.0**

---


  * Added support for chapterizing with chapters from ChapterDB
  * Added setting for custom chapter name
  * Added setting for custom mkvmerge
  * Added logging for easier problem-solving
  * Added function for automatically checking and downloading updates (no downloading in portable version)
  * Added option to turn off automatic updates
  * Updated mkvmerge to latest (v5.0.1)


---

**2.0.0.0**

---


  * Completely rewritten chapterizing code
  * New UI for queuing movies
  * Compression bug fixed
  * Moved work directory to temporary directory
  * Ability to Remove and Replace chapters


---

**1.2.0.0**

---


  * Settings dialog for more customizations


---

**1.1.0.0**

---


  * Complete rewrite of Application

---
